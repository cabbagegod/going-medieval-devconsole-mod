using System;
using System.Collections;
using System.Collections.Generic;
using NSEipix.Base;
using NSEipix.View.UI;
using UnityEngine;

namespace NSMedieval.DevConsole
{
	// Token: 0x020004D4 RID: 1236
	public class DeveloperToolsView : MonoSingleton<DeveloperToolsView>, IObserver
	{
		// Token: 0x140000C4 RID: 196
		// (add) Token: 0x06002C5C RID: 11356
		// (remove) Token: 0x06002C5D RID: 11357
		public event Action SpawnResourceReadyEvent;

		// Token: 0x06002C5E RID: 11358
		public void SpawnResourceReady()
		{
			Action spawnResourceReadyEvent = this.SpawnResourceReadyEvent;
			if (spawnResourceReadyEvent == null)
			{
				return;
			}
			spawnResourceReadyEvent();
		}

		// Token: 0x06002C5F RID: 11359
		public void Open()
		{
			this.devToolsActive = true;
			this.SetActive();
			this.mainContainer.SetActive(true);
		}

		// Token: 0x06002C60 RID: 11360
		public void SetBackButton(Action callback)
		{
			if (callback == null)
			{
				this.buttonBack.gameObject.SetActive(false);
				return;
			}
			this.buttonBack.gameObject.SetActive(true);
			this.buttonBack.onClick.RemoveAllListeners();
			this.buttonBack.onClick.AddListener(delegate()
			{
				callback();
			});
		}

		// Token: 0x06002C61 RID: 11361
		private void Close()
		{
			this.devToolsActive = false;
			this.SetActive();
			this.mainContainer.SetActive(false);
		}

		// Token: 0x06002C62 RID: 11362
		private void Start()
		{
			MonoSingleton<SceneController>.Instance.Tick += this.OnTick;
			this.SpawnResourceReadyEvent += this.Close;
			this.buttonClose.onClick.AddListener(delegate()
			{
				this.Close();
			});
			this.SetBackButton(null);
			this.SetTabs();
		}

		// Token: 0x06002C63 RID: 11363
		private void OnDestroy()
		{
			this.SpawnResourceReadyEvent = null;
		}

		// Token: 0x06002C64 RID: 11364
		private void SetTabs()
		{
			for (int i = 0; i < this.categories.Count; i++)
			{
				int num = i;
				SoundButtonToggle key = UnityEngine.Object.Instantiate<GameObject>(this.tabButtonPrefab, this.tabsParent).GetComponent<SoundButtonToggle>();
				DeveloperView component;
				if (this.categories[num].Category == DeveloperPanelCategory.Console)
				{
					component = this.consolePanel;
				}
				else
				{
					component = UnityEngine.Object.Instantiate<GameObject>(this.panelPrefab, this.panelsParent).GetComponent<DeveloperPanelView>();
					component.SetupPanel((DeveloperPanelCategory)num);
				}
				this.panels.Add(key, component);
				key.SetLabels(this.categories[num].Name);
				key.IsOn(false);
				key.Button.onClick.AddListener(delegate()
				{
					this.EnableTabs(key);
				});
				if (num == 0)
				{
					key.IsOn(true);
					this.EnableTabs(key);
				}
			}
			this.SetActive();
		}

		// Token: 0x06002C65 RID: 11365
		private void EnableTabs(SoundButtonToggle panelToEnable)
		{
			foreach (KeyValuePair<SoundButtonToggle, DeveloperView> keyValuePair in this.panels)
			{
				bool flag = keyValuePair.Key == panelToEnable;
				keyValuePair.Key.Button.interactable = !flag;
				keyValuePair.Key.IsOn(flag);
				keyValuePair.Value.SetActive(flag);
			}
		}

		// Token: 0x06002C66 RID: 11366
		private void OnTick(float obj)
		{
		}

		// Token: 0x06002C67 RID: 11367
		private void SetActive()
		{
		}

		// Token: 0x06002C68 RID: 11368
		private IEnumerator WaitForClose()
		{
			yield return new WaitForEndOfFrame();
			MonoSingleton<DeveloperConsoleController>.Instance.MouseInputBlocked = this.devToolsActive;
			yield break;
		}

		// Token: 0x060057EB RID: 22507
		public void SendConsoleMessage(string message)
		{
			this.consolePanel.GetComponentInChildren<DeveloperConsole>().AddMessageToConsole(message, ConsoleMessageType.Standard);
		}

		// Token: 0x0400172D RID: 5933
		[SerializeField]
		private GameObject mainContainer;

		// Token: 0x0400172E RID: 5934
		[SerializeField]
		private SoundButton buttonClose;

		// Token: 0x0400172F RID: 5935
		[SerializeField]
		private SoundButton buttonBack;

		// Token: 0x04001730 RID: 5936
		[SerializeField]
		private Transform tabsParent;

		// Token: 0x04001731 RID: 5937
		[SerializeField]
		private Transform panelsParent;

		// Token: 0x04001732 RID: 5938
		[SerializeField]
		private DeveloperView consolePanel;

		// Token: 0x04001733 RID: 5939
		[SerializeField]
		private GameObject panelPrefab;

		// Token: 0x04001734 RID: 5940
		[SerializeField]
		private GameObject tabButtonPrefab;

		// Token: 0x04001735 RID: 5941
		private bool devToolsActive;

		// Token: 0x04001736 RID: 5942
		private List<DeveloperToolsCategory> categories = new DeveloperToolsCategories().Categories;

		// Token: 0x04001737 RID: 5943
		private Dictionary<SoundButtonToggle, DeveloperView> panels = new Dictionary<SoundButtonToggle, DeveloperView>();
	}
}
