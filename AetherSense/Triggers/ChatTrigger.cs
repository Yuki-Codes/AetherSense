using System;
using System.Text.RegularExpressions;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;

namespace AetherSense.Triggers
{
	public class ChatTrigger : TriggerBase
	{
		private string regexPattern = "";
		private int duration = 1000;
		private int cooldown = 250;
		private bool wasPreviousMessageYou = false;
		private bool onlyYou = true;

		private DateTime cooldownUntil = DateTime.MinValue;

		public int Duration
		{
			get => this.duration;
			set => this.duration = value;
		}

		public int Cooldown
		{
			get => this.cooldown;
			set => this.cooldown = value;
		}

		public override int CooldownLeft
		{
			get
			{
				if (DateTime.Now > cooldownUntil)
					return 0;

				return (int)((cooldownUntil - DateTime.Now).TotalMilliseconds);
			}
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
			base.Attach();
			Plugin.ChatGui.ChatMessage += this.OnChatMessage;
		}

		public override void Detach()
		{
			base.Detach();
			Plugin.ChatGui.ChatMessage -= this.OnChatMessage;
		}

		public override void OnEditorGui()
		{
			ImGui.SameLine();
			ImGui.Checkbox("Only Self", ref this.onlyYou);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("This trigger will only work if the proceding line in the log begins with 'You', eg. 'You cast' or 'You use'");

			ImGui.InputText("Regex", ref this.regexPattern, 100);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("A regex-format string to check each chat message with.");

			ImGui.InputInt("Duration", ref this.duration);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("The duration to run the selected pattern for when this trigger runs.");

			ImGui.InputInt("Cooldown", ref this.cooldown);
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("The cooldown before this trigger can be run again.");
		}

		private void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
		{
			if (this.Pattern == null)
				return;

			this.OnChatMessage(sender.TextValue, message.TextValue);
		}

		public void OnChatMessage(string sender, string message)
		{
			// Don't process any chat messages if the plugin is globally disabled
			if (!Plugin.Configuration.Enabled)
				return;

			bool isSystem = string.IsNullOrEmpty(sender);

			if (this.OnlyYou)
			{
				bool currentMessageWasYou = (isSystem && (message.StartsWith("You") || message.StartsWith("Vous") || message.StartsWith("Du")));

				if (!this.wasPreviousMessageYou)
				{
					this.wasPreviousMessageYou = currentMessageWasYou;
					return;
				}
			}

			// Add the sender back into the message before regex.
			if (!isSystem)
				message = sender + ": " + message;

			if (!Regex.IsMatch(message, this.RegexPattern))
				return;

			if (DateTime.Now < cooldownUntil)
				return;

			cooldownUntil = DateTime.Now.AddMilliseconds(this.Cooldown);

			PluginLog.Information($"Triggered: {this.Name} with chat message: \"{message}\"");
			this.Pattern.RunFor(this.Duration);
		}
	}
}
