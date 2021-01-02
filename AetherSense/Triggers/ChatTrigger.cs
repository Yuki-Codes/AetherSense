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
		private bool wasPreviousMessageYou = false;
		private bool onlyYou = true;

		public int Duration
		{
			get => this.duration;
			set => this.duration = value;
		}

		public bool OnlyYou
		{
			get => this.onlyYou;
			set => onlyYou = value;
		}

		public string RegexPattern
		{
			get => this.regexPattern;
			set => this.regexPattern = value;
		}

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
			ImGui.SameLine();
			ImGui.Checkbox("Only Self", ref this.onlyYou);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("This trigger will only work if the proceding line in the log begins with 'You', eg. 'You cast' or 'You use'");

			ImGui.InputText("Regex", ref this.regexPattern, 32);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("A regex-format string to check each chat message with.");

			ImGui.InputInt("Duration", ref this.duration);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("The duration to run the selected pattern for when this trigger runs.");
		}

		private void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
		{
			if (this.Pattern == null)
				return;

			this.OnChatMessage(message.TextValue);
		}

		public void OnChatMessage(string message)
		{
			// Don't process any chat messages if the plugin is globally disabled
			if (!Plugin.Configuration.Enabled)
				return;

			if (message.StartsWith("You"))
			{
				this.wasPreviousMessageYou = true;
				return;
			}

			if (this.OnlyYou && !this.wasPreviousMessageYou)
				return;

			this.wasPreviousMessageYou = false;

			if (!Regex.IsMatch(message, this.RegexPattern))
				return;

			PluginLog.Information($"Triggered: {this.Name} with chat message: \"{message}\"");
			this.Pattern.RunFor(this.Duration);
		}
	}
}
