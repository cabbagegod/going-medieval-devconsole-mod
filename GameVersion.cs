using System;
using NSEipix.Base;
using NSMedieval.Controllers;
using NSMedieval.UI;
using NSMedieval.UI.PhotoMode;
using TMPro;
using UnityEngine;

namespace NSMedieval.Tools
{
	// Token: 0x020005F7 RID: 1527
	[RequireComponent(typeof(TMP_Text))]
	public class GameVersion : MonoBehaviour, IObserver
	{
		// Token: 0x06003A3A RID: 14906
		private void Start()
		{
			this.label = base.GetComponent<TMP_Text>();
			this.label.SetText(string.Concat(new string[]
			{
				this.prefix,
				Application.version,
				" CONSOLE MOD 1.0",
				this.suffix,
				" "
			}), true);
			MonoSingleton<LoadingController>.Instance.Attach(this);
		}

		// Token: 0x06003A3B RID: 14907
		private void OnMainSceneSceneLoaded()
		{
			MonoSingleton<PhotoModeController>.Instance.PhotoModeVisibleEvent += this.HideText;
			MonoSingleton<UIController>.Instance.HideUIToggleevent += this.HideText;
		}

		// Token: 0x06003A3C RID: 14908
		private void HideText(bool textHidden)
		{
			this.label.enabled = !textHidden;
		}

		// Token: 0x04001CE0 RID: 7392
		[SerializeField]
		private string prefix;

		// Token: 0x04001CE1 RID: 7393
		[SerializeField]
		private string suffix;

		// Token: 0x04001CE2 RID: 7394
		private TMP_Text label;
	}
}
