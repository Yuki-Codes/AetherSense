using System.Threading.Tasks;
using AetherSense.Patterns;
using AetherSense.Triggers;
using ImGuiNET;

namespace AetherSense
{
	public static class DebugWindow
	{
		public static void OnGui()
		{
			if (ImGui.Button("Clear Devices"))
			{
				Plugin.Devices.Clear();
			}

			if (Plugin.Configuration.Enabled)
			{
				foreach (Devices.Group group in Plugin.Devices.Groups)
				{
					ImGui.Text("Group: " + group.GroupId);
					ImGui.SameLine();
					ImGui.Text(group.DesiredIntensity.ToString("F2"));
					ImGui.SameLine();
					ImGui.Text("/");
					ImGui.SameLine();
					ImGui.Text(group.Maximum.ToString("F2"));
					ImGui.SameLine();
					ImGui.ProgressBar((float)group.CurrentIntensity);
				}
			}
				
			else
			{
				ImGui.Text("AetherSense is disabled");
			}

			ImGui.Text($"Triggers: {Plugin.Configuration.Triggers.Count}");
			int attachedCount = 0;
			foreach (TriggerBase trigger in Plugin.Configuration.Triggers)
			{
				string ac = trigger.IsAttached ? "X" : " ";
				ImGui.Text($"    [{ac}] {trigger.Name}");

				if (trigger.IsAttached)
					attachedCount++;

				if (trigger.Pattern != null && trigger.Pattern.Active)
				{
					ImGui.Text($"        {trigger.Pattern.GetType().Name} - {trigger.Pattern.DurationLeft}ms");
				}
			}

			ImGui.Text($"{attachedCount} loaded");

			ImGui.Text($"Devices: {Plugin.Devices.Count}");
			foreach (Device device in Plugin.Devices.All)
			{
				ImGui.Text($"    > {device.ClientDevice.Name}");
			}
		}
	}
}
