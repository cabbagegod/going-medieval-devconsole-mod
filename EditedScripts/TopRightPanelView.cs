using System;
using System.Collections.Generic;
using NSEipix.Base;
using NSEipix.View.UI;
using NSMedieval.Controllers;
using NSMedieval.DevConsole;
using NSMedieval.Enums;
using NSMedieval.GameEventSystem;
using NSMedieval.Manager;
using NSMedieval.RoomDetection;
using NSMedieval.Sound;
using NSMedieval.State;
using NSMedieval.Tools.BugReporting;
using NSMedieval.Tools.Debug;
using NSMedieval.UI.PhotoMode;
using NSMedieval.UI.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace NSMedieval.UI
{
	// Token: 0x020006AB RID: 1707
	public class TopRightPanelView : UIView, IObserver
	{
		// Token: 0x060041AC RID: 16812
		private void Start()
		{
			MonoSingleton<GameEventSystemController>.Instance.Attach(this);
			Singleton<GameSpeedManager>.Instance.UpdateTimeScaleUIEvent += this.OnChangeTimeScale;
			MonoSingleton<WorldTimeManager>.Instance.DateTimeInitalizeEvent += this.OnDateTimeInitalize;
			MonoSingleton<WorldTimeManager>.Instance.TimeUpdateEvent += this.OnTimeUpdate;
			MonoSingleton<WorldTimeManager>.Instance.DateUpdateEvent += this.OnDateUpdate;
			MonoSingleton<WeatherManager>.Instance.TemperatureUpdateEvent += this.OnTemperatureUpdate;
			PlayerVoxelInfo instance = MonoSingleton<PlayerVoxelInfo>.Instance;
			instance.OnHoverChange = (Action<Vector3Int>)Delegate.Combine(instance.OnHoverChange, new Action<Vector3Int>(this.OnVoxelHoverChange));
			MonoSingleton<RaidController>.Instance.RaidSpawnedEvent += this.OnRaidStarted;
			MonoSingleton<RaidController>.Instance.RaidEndedEvent += this.OnRaidEnded;
			for (int i = 0; i < this.gameSpeedButtons.Length; i++)
			{
				int index = i;
				this.gameSpeedButtons[index].onValueChanged.AddListener(delegate(bool call)
				{
					if (call)
					{
						Singleton<GameSpeedManager>.Instance.ChangeTimeScaleByIndex(index);
					}
				});
				if (index == 1)
				{
					this.SelectSpeedButton(index);
				}
			}
			this.almanacButton.onClick.AddListener(new UnityAction(this.ToggleAlmanac));
			MonoSingleton<KeybindingManager>.Instance.SubscribeToEvent(KeyInputEvent.ShowHideAlmanac, new Action(this.ToggleAlmanac), false);
			this.optionsButton.onClick.AddListener(delegate()
			{
				base.SceneUIManager.ShowNewView("InGameMenuView");
			});
			this.statsButton.onClick.AddListener(delegate()
			{
				base.SceneUIManager.TogglePanel("StatisticsPanelManager");
			});
			this.bugReportButton.onClick.AddListener(new UnityAction(this.ShowBugReporter));
			MonoSingleton<KeybindingManager>.Instance.SubscribeToEvent(KeyInputEvent.Report, new Action(this.ShowBugReporter), false);
			this.photoModeButton.onClick.AddListener(new UnityAction(this.ShowPhotoMode));
			MonoSingleton<DeveloperToolsView>.Instance.SendConsoleMessage("<color=blue>Successfully loaded console mod v1.0!</color>");
		}

		// Token: 0x060041AD RID: 16813
		protected override void OnDestroy()
		{
			if (this == null || base.gameObject == null)
			{
				return;
			}
			Singleton<GameSpeedManager>.Instance.UpdateTimeScaleUIEvent -= this.OnChangeTimeScale;
			base.OnDestroy();
		}

		// Token: 0x060041AE RID: 16814
		private void OnChangeTimeScale(int scale, int speedIndex)
		{
			this.SelectSpeedButton(speedIndex);
			if (!MonoSingleton<UIController>.Instance.GameStarted)
			{
				return;
			}
			MonoSingleton<AudioManager>.Instance.OnGameplayPaused(speedIndex);
		}

		// Token: 0x060041AF RID: 16815
		private void OnTimeUpdate()
		{
			int hoursSinceDay = MonoSingleton<GlobalSaveController>.Instance.CurrentVillage.DateAndTime.HoursSinceDay;
			this.timeText.text = ((hoursSinceDay < 10) ? string.Format("0{0}", hoursSinceDay) : hoursSinceDay.ToString()) + base.Localize.GetText("general_hour_short");
			this.UpdateBugReportButtonState();
		}

		// Token: 0x060041B0 RID: 16816
		private void OnDateUpdate()
		{
			this.dateText.text = string.Format("{0}. {1}\n{2}", MonoSingleton<GlobalSaveController>.Instance.CurrentVillage.DateAndTime.Year, Singleton<LocalizationController>.Instance.GetText("general_" + MonoSingleton<GlobalSaveController>.Instance.CurrentVillage.DateAndTime.Season.Name), UiUtils.GetLocalizedDay());
		}

		// Token: 0x060041B1 RID: 16817
		private void OnTemperatureUpdate()
		{
			this.UpdateWeatherText();
		}

		// Token: 0x060041B2 RID: 16818
		private void UpdateWeatherText()
		{
			string text = MonoSingleton<GameEventSystem>.Instance.RunningEventsWeatherTextKey();
			if (text != null)
			{
				this.weather.text = this.GetTemperatureLocalized() + "\n" + text;
				return;
			}
			this.weather.text = this.GetTemperatureLocalized() + "\n" + MonoSingleton<WeatherManager>.Instance.EventNamesLocalized;
		}

		// Token: 0x060041B3 RID: 16819
		private string GetTemperatureLocalized()
		{
			if (!MonoSingleton<PlayerVoxelInfo>.IsInstantiated())
			{
				return string.Empty;
			}
			Vector3Int hoverGridPosition = MonoSingleton<PlayerVoxelInfo>.Instance.HoverGridPosition;
			string text = Singleton<LocalizationController>.Instance.GetText((MonoSingleton<RoomDetection>.Instance.GetRoomAt(hoverGridPosition) != null) ? "inside" : "outside");
			return MonoSingleton<GlobalSaveController>.Instance.CurrentVillage.DateAndTime.GetLocalizedTemperature(MonoSingleton<RoomTemperatureManager>.Instance.GetTemperature(hoverGridPosition)) + " " + text;
		}

		// Token: 0x060041B4 RID: 16820
		private void OnDateTimeInitalize()
		{
			this.OnTimeUpdate();
			this.OnDateUpdate();
			this.OnTemperatureUpdate();
		}

		// Token: 0x060041B5 RID: 16821
		private void OnRaidStarted(ActiveRaidInfo info, List<EnemyInstance> enemies)
		{
			this.gameSpeedButtons[3].gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.2f);
			this.isFastSpeedBlocked = true;
		}

		// Token: 0x060041B6 RID: 16822
		private void OnRaidEnded(ActiveRaidInfo info)
		{
			this.gameSpeedButtons[3].gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			this.isFastSpeedBlocked = false;
		}

		// Token: 0x060041B7 RID: 16823
		private void UpdateBugReportButtonState()
		{
			if (BugReporterJiraAPI.IsReportUploading)
			{
				if (this.bugReportButton.interactable)
				{
					this.bugReportButton.interactable = false;
					return;
				}
			}
			else if (!this.bugReportButton.interactable)
			{
				MonoSingleton<BlackBarMessageController>.Instance.ShowBlackBarMessage(Singleton<LocalizationController>.Instance.GetText("report_sent"));
				this.bugReportButton.interactable = true;
			}
		}

		// Token: 0x060041B8 RID: 16824
		private void SelectSpeedButton(int speedIndex)
		{
			for (int i = 0; i < this.gameSpeedButtons.Length; i++)
			{
				this.gameSpeedButtons[i].interactable = (i != speedIndex);
				if (this.isFastSpeedBlocked && i >= 3)
				{
					return;
				}
				if (i == GameSpeedManager.SpeedIndexFaster)
				{
					this.gameSpeedButtons[i].GetComponent<Image>().color = ((Singleton<GameSpeedManager>.Instance.GetCurrentTimescaleIndex() == GameSpeedManager.SpeedIndexSleeping) ? Color.cyan : Color.white);
				}
			}
		}

		// Token: 0x060041B9 RID: 16825
		private void OnVoxelHoverChange(Vector3Int obj)
		{
			this.UpdateWeatherText();
		}

		// Token: 0x060041BA RID: 16826
		private void ShowPhotoMode()
		{
			MonoSingleton<PhotoMode>.Instance.Show();
		}

		// Token: 0x060041BB RID: 16827
		private void ToggleAlmanac()
		{
			base.SceneUIManager.TogglePanel("AlmanacPanelManager");
		}

		// Token: 0x060041BC RID: 16828
		private void ShowBugReporter()
		{
			MonoSingleton<BugReporterManager>.Instance.ShowReporter();
		}

		// Token: 0x060041BD RID: 16829
		private void OnStartEvent(GameEventInstance gameEventInstance)
		{
			if (gameEventInstance == null || gameEventInstance.Blueprint == null || gameEventInstance.ReplaceWeatherText == null)
			{
				return;
			}
			this.UpdateWeatherText();
		}

		// Token: 0x060041BE RID: 16830
		private void OnEndEvent(GameEventInstance gameEventInstance)
		{
			if (gameEventInstance == null || gameEventInstance.Blueprint == null || gameEventInstance.ReplaceWeatherText == null)
			{
				return;
			}
			this.UpdateWeatherText();
		}

		// Token: 0x0400219D RID: 8605
		[SerializeField]
		private TMP_Text dateText;

		// Token: 0x0400219E RID: 8606
		[SerializeField]
		private TMP_Text timeText;

		// Token: 0x0400219F RID: 8607
		[SerializeField]
		private TMP_Text weather;

		// Token: 0x040021A0 RID: 8608
		[SerializeField]
		private SoundButton almanacButton;

		// Token: 0x040021A1 RID: 8609
		[SerializeField]
		private SoundButton optionsButton;

		// Token: 0x040021A2 RID: 8610
		[SerializeField]
		private SoundButton statsButton;

		// Token: 0x040021A3 RID: 8611
		[SerializeField]
		private SoundButton bugReportButton;

		// Token: 0x040021A4 RID: 8612
		[SerializeField]
		private SoundButton photoModeButton;

		// Token: 0x040021A5 RID: 8613
		[SerializeField]
		private Toggle[] gameSpeedButtons;

		// Token: 0x040021A6 RID: 8614
		private bool isFastSpeedBlocked;
	}
}
