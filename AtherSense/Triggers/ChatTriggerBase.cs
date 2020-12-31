using System.Text.RegularExpressions;
using Dalamud.Game.Chat;
using Dalamud.Game.Chat.SeStringHandling;
using ImGuiNET;

namespace AetherSense.Triggers
{
	public class ChatTriggerBase : TriggerBase
	{
		public int Duration { get; set; }

		public string RegexPattern { get; set; }
		public XivChatType ChatType { get; set; } = XivChatType.SystemMessage;

		public override void Initialize()
		{
			Plugin.DalamudPluginInterface.Framework.Gui.Chat.OnChatMessage += this.OnChatMessage;
		}

		public override void OnEditorGui()
		{
			ImGui.Text("Hello!");
		}

		private void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
		{
			if (type != this.ChatType)
				return;

			if (this.Pattern == null)
				return;

			if (!Regex.IsMatch(message.TextValue, this.RegexPattern))
				return;

			this.Pattern.RunFor(this.Duration);
		}
	}
}
