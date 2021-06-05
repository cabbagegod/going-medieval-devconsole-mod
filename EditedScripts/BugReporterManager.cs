using System;
using NSEipix;
using NSEipix.Base;
using NSMedieval.DevConsole;
using NSMedieval.Sound;
using NSMedieval.Tools.BugReporting;
using UnityEngine;

namespace NSMedieval.Tools.Debug
{
	// Token: 0x02000609 RID: 1545
	public class BugReporterManager : MonoSingleton<BugReporterManager>, IObserver
	{
		// Token: 0x06003ABF RID: 15039
		public void ShowReporter()
		{
			MonoSingleton<AudioManager>.Instance.PlaySound("AttackStart");
			MonoSingleton<DeveloperToolsView>.Instance.Open();
		}

		// Token: 0x06003AC0 RID: 15040
		private void Start()
		{
			Application.logMessageReceived += this.LogCallback;
		}

		// Token: 0x06003AC1 RID: 15041
		private void OnDisable()
		{
			Application.logMessageReceived -= this.LogCallback;
		}

		// Token: 0x06003AC2 RID: 15042
		private void LogCallback(string condition, string stackTrace, LogType type)
		{
		}

		// Token: 0x0600565F RID: 22111
		public void OriginalCode()
		{
			this.bugReporter.Show(BugReporter.WindowType.MainWindow);
		}

		// Token: 0x06005720 RID: 22304
		private void OriginalLogCallback(string condition, string stackTrace, LogType type)
		{
			if (type != LogType.Exception || this.exceptionCaught)
			{
				return;
			}
			this.bugReporter.Show(BugReporter.WindowType.Quitting);
			MonoSingleton<BugReporterJiraAPI>.Instance.SubmitReport(BugReporterJiraAPI.ReportPriority.High, BugReporterJiraAPI.ReportType.Exception, TextFormatting.GetErrorFilename(stackTrace), condition + "\n" + stackTrace, delegate(BugReporterJiraAPI.ReportStatus status)
			{
				if (status == BugReporterJiraAPI.ReportStatus.Error)
				{
					this.bugReporter.Show(BugReporter.WindowType.ErrorOccured);
					return;
				}
				MonoSingleton<TaskController>.Instance.WaitForUnscaled(3f).Then(delegate
				{
					this.bugReporter.Close();
				});
			});
			this.exceptionCaught = true;
		}

		// Token: 0x04001D6B RID: 7531
		[SerializeField]
		private BugReporter bugReporter;

		// Token: 0x04001D6C RID: 7532
		private bool exceptionCaught;
	}
}
