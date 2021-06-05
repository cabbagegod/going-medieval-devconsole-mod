using System;
using System.Collections.Generic;
using System.Linq;
using NSEipix.Base;
using UnityEngine;
using UnityEngine.UI;

namespace NSMedieval.DevConsole
{
	// Token: 0x020004C9 RID: 1225
	public class DeveloperConsole : DeveloperView, IObserver
	{
		// Token: 0x06002B88 RID: 11144 RVA: 0x000BC623 File Offset: 0x000BA823
		public override void SetActive(bool active)
		{
			base.gameObject.SetActive(active);
			if (!active)
			{
				this.commandIterator = 0;
				return;
			}
			this.ClearInputField();
		}

		// Token: 0x06002B89 RID: 11145 RVA: 0x000BC642 File Offset: 0x000BA842
		public override void SetupPanel(DeveloperPanelCategory category)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002B8A RID: 11146 RVA: 0x000BC649 File Offset: 0x000BA849
		public override void Reset()
		{
			this.ClearInputField();
		}

		// Token: 0x06002B8B RID: 11147 RVA: 0x000BC651 File Offset: 0x000BA851
		private void Start()
		{
			MonoSingleton<SceneController>.Instance.Tick += this.OnTick;
			MonoSingleton<DeveloperConsoleController>.Instance.Attach(this);
		}

		// Token: 0x06002B8C RID: 11148 RVA: 0x000BC674 File Offset: 0x000BA874
		private void OnTick(float deltaTime)
		{
			if (!base.gameObject.activeInHierarchy)
			{
				return;
			}
			if (Input.GetKeyDown(KeyCode.Return) && this.inputField.text != string.Empty)
			{
				this.AddMessageToConsole(string.Format(">> {0}", this.inputField.text), ConsoleMessageType.Command);
				this.SendInput(this.inputField.text);
				if (!this.usedCommands.DefaultIfEmpty(string.Empty).Last<string>().Equals(this.inputField.text))
				{
					this.usedCommands.Add(this.inputField.text);
				}
				this.ClearInputField();
				this.commandIterator = 0;
				return;
			}
			if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				this.inputField.text = this.IterateUsedCommands(-1);
				return;
			}
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				this.inputField.text = this.IterateUsedCommands(1);
			}
		}

		// Token: 0x06002B8D RID: 11149 RVA: 0x000BC768 File Offset: 0x000BA968
		private void ClearInputField()
		{
			this.inputField.Select();
			this.inputField.ActivateInputField();
			this.inputField.text = string.Empty;
		}

		// Token: 0x06002B8E RID: 11150 RVA: 0x000BC790 File Offset: 0x000BA990
		private string IterateUsedCommands(int step)
		{
			if (this.usedCommands.Count == 0)
			{
				return null;
			}
			this.ClearInputField();
			if (this.commandIterator == 0)
			{
				this.commandIterator = this.usedCommands.Count;
			}
			else
			{
				this.commandIterator = (((this.commandIterator == 1 && step < 0) || (this.commandIterator == this.usedCommands.Count && step > 0)) ? this.commandIterator : (this.commandIterator + step));
			}
			return this.usedCommands.ElementAt(this.commandIterator - 1);
		}

		// Token: 0x06002B8F RID: 11151
		public ConsumeFlag AddMessageToConsole(string message, ConsoleMessageType messageType = ConsoleMessageType.Standard)
		{
			switch (messageType)
			{
			case ConsoleMessageType.Warrning:
				message = this.MessageAsWarrning(message);
				break;
			case ConsoleMessageType.Error:
				message = this.MessageAsError(message);
				break;
			case ConsoleMessageType.Command:
				message = this.MessageAsCommand(message);
				break;
			}
			Text text = this.consoleText;
			text.text += string.Format("{0}\n", message);
			return ConsumeFlag.CONSUME;
		}

		// Token: 0x06002B90 RID: 11152 RVA: 0x000BC880 File Offset: 0x000BAA80
		private string MessageAsError(string message)
		{
			return string.Format("<color=red>{0}</color>", message);
		}

		// Token: 0x06002B91 RID: 11153 RVA: 0x000BC88D File Offset: 0x000BAA8D
		private string MessageAsWarrning(string message)
		{
			return string.Format("<color=yellow>{0}</color>", message);
		}

		// Token: 0x06002B92 RID: 11154 RVA: 0x000BC89A File Offset: 0x000BAA9A
		private string MessageAsCommand(string message)
		{
			return string.Format("<color=green>{0}</color>", message);
		}

		// Token: 0x06002B93 RID: 11155 RVA: 0x000BC8A8 File Offset: 0x000BAAA8
		private void SendInput(string input)
		{
			string[] array = input.Split(null);
			MonoSingleton<DeveloperConsoleController>.Instance.ParseCommand(array[0], array.Skip(1).ToArray<string>());
		}

		// Token: 0x040016F6 RID: 5878
		[Header("UI Components")]
		[SerializeField]
		private Text consoleText;

		// Token: 0x040016F7 RID: 5879
		[SerializeField]
		private InputField inputField;

		// Token: 0x040016F8 RID: 5880
		private List<string> usedCommands = new List<string>();

		// Token: 0x040016F9 RID: 5881
		private int commandIterator;
	}
}
