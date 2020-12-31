using System.Text.RegularExpressions;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using Dalamud.Plugin;
using ImGuiNET;

namespace AetherSense.Triggers
{
	public class ChatTrigger : TriggerBase
	{
		private string regexPattern = "";
		private int duration = 1000;

		public int Duration
		{
			get => this.duration;
			set => this.duration = value;
		}

		public string RegexPattern
		{
			get => this.regexPattern;
			set => this.regexPattern = value;
		}

		public XivChatType ChatType { get; set; } = XivChatType.SystemMessage;

		public override void Attach()
		{
			Plugin.DalamudPluginInterface.Framework.Gui.Chat.OnChatMessage += this.OnChatMessage;
		}

		public override void Detach()
		{
			Plugin.DalamudPluginInterface.Framework.Gui.Chat.OnChatMessage -= this.OnChatMessage;
		}

		public override void OnEditorGui()
		{
			ImGui.InputText("Regex", ref this.regexPattern, 32);
			ImGui.InputInt("Duration", ref this.duration);
		}

		private void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
		{
			PluginLog.Information("Recieved chat: " + type + " - " + message.TextValue);

			if (type != this.ChatType)
				return;

			if (this.Pattern == null)
				return;

			PluginLog.Information("Match: " + Regex.IsMatch(message.TextValue, this.RegexPattern));
			if (!Regex.IsMatch(message.TextValue, this.RegexPattern))
				return;

			this.Pattern.RunFor(this.Duration);
		}
	}
}
