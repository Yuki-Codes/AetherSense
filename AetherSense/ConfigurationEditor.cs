using AetherSense.Patterns;
using ImGuiNET;
using AetherSense.Triggers;
using System;
using System.Collections.Generic;
using AetherSense.Utils;

namespace AetherSense
{
	public static class ConfigurationEditor
	{
		public static List<Type> TriggerTypes = new List<Type>()
		{
			typeof(ChatTrigger),
		};

		public static List<Type> PatternTypes = new List<Type>()
		{
			typeof(ConstantPattern),
			typeof(AlternatingPattern),
			typeof(WavePattern)
		};

		public static void OnGui()
		{
			if (Plugin.lastConnectionError != null)
			{
				ImGui.Text("Failed to connect to Buttplug server:\nIs the server running and available?");
			}
			else
			{
				ImGui.Text("Triggers are disabled while this window is visible.");
			}

			bool enabled = Plugin.Configuration.Enabled;
			ImGui.Checkbox("Enabled", ref enabled);
			Plugin.Configuration.Enabled = enabled;

			string address = Plugin.Configuration.ServerAddress;
			ImGui.InputText("Server", ref address, 99);
			Plugin.Configuration.ServerAddress = address;

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
						foreach (Type type in TriggerTypes)
						{
							if (ImGui.Selectable(NameUtility.ToName(type.Name)))
							{
								TriggerBase trigger = (TriggerBase)Activator.CreateInstance(type);
								Plugin.Configuration.Triggers.Add(trigger);
								trigger.Name = "New " + NameUtility.ToName(type.Name);
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

				if (ImGui.BeginTabItem("Devices"))
				{
					ImGui.Columns(2);
					ImGui.Text("Name");
					ImGui.NextColumn();
					ImGui.Text("Group");
					ImGui.NextColumn();

					foreach (Device device in Plugin.Devices.All)
					{
						string name = device.ClientDevice.Name;

						if (!Plugin.Configuration.DeviceGroups.ContainsKey(name))
							Plugin.Configuration.DeviceGroups.Add(name, 1);

						ImGui.Text(name);

						ImGui.NextColumn();
						int group = Plugin.Configuration.DeviceGroups[name];

						string label = group == 0 ? "Disabled" : "Group " + group;
						if (ImGui.BeginCombo($"##{name}combo", label))
						{
							for (int n = 0; n < 10; n++)
							{
								label = n == 0 ? "Disabled" : "Group " + n;
								if (ImGui.Selectable(label, n == group))
								{
									group = n;
								}
							}

							ImGui.EndCombo();
						}

						Plugin.Configuration.DeviceGroups[name] = group;
					}

				}
				
				ImGui.EndTabBar();
			}
		}
	}
}
