using AetherSense.Patterns;
using ImGuiNET;
using AetherSense.Triggers;
using System;
using System.Collections.Generic;

namespace AetherSense
{
	public static class ConfigurationEditor
	{
		private static Dictionary<string, Type> triggerTypes = new Dictionary<string, Type>()
		{
			{ "Chat Trigger", typeof(ChatTrigger) },
		};

		public static void OnGui()
		{
			ImGui.Text("Aether sense is disabled while this window is visible.");

			bool enabled = Plugin.Configuration.Enabled;
			ImGui.Checkbox("Enabled", ref enabled);
			Plugin.Configuration.Enabled = enabled;

			if (ImGui.BeginTabBar("##ConfigTabBar", ImGuiTabBarFlags.None))
			{
				if (ImGui.BeginTabItem("Triggers"))
				{
					ImGui.Columns(2, "##TriggerBar", false);
					ImGui.SetColumnWidth(0, 150);

					if (ImGui.Button("Reset Triggers"))
					{
						Configuration defaultConfig = Configuration.GetDefaultConfiguration();
						Plugin.Configuration.Triggers = defaultConfig.Triggers;
					}

					ImGui.NextColumn();
					if (ImGui.BeginCombo("Add New Trigger", null, ImGuiComboFlags.NoPreview))
					{
						foreach (KeyValuePair<string, Type> keyVal in triggerTypes)
						{
							if (ImGui.Selectable(keyVal.Key))
							{
								TriggerBase trigger = (TriggerBase)Activator.CreateInstance(keyVal.Value);
								Plugin.Configuration.Triggers.Add(trigger);
								trigger.Name = "New " + keyVal.Key;
							}
						}

						ImGui.EndCombo();
					}

					ImGui.Columns(1);

					ImGui.Separator();

					int indexToDelete = -1;
					for (int i = 0; i < Plugin.Configuration.Triggers.Count; i++)
					{
						bool keep = Plugin.Configuration.Triggers[i].OnEditorGuiTop(i);
						if (!keep)
						{
							indexToDelete = i;
						}
					}

					if (indexToDelete != -1)
						Plugin.Configuration.Triggers.RemoveAt(indexToDelete);

					ImGui.EndTabItem();
				}
				
				ImGui.EndTabBar();
			}
		}
	}
}
