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

			// If it wasn't clear before that I dont know how to work ImGUI, I hope this clears it up for you:
			// We need 32px of space to accomodate the bottom bar, so...
			ImGui.Spacing();
			ImGui.Spacing();
			ImGui.Spacing();
			ImGui.Spacing();
			ImGui.Spacing();
			ImGui.Spacing();
			ImGui.Spacing();

			// Now set the rendering cursor to 32pixels above the window bottom.
			ImGui.SetCursorPosY(ImGui.GetWindowHeight() - 32);
			ImGui.Separator();

			if (ImGui.Button("Save"))
			{
				Plugin.Configuration.Save();
			}
		}
	}
}
