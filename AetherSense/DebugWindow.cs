using System.Threading.Tasks;
using AetherSense.Patterns;
using ImGuiNET;

namespace AetherSense
{
	public static class DebugWindow
	{
		public static void OnGui()
		{
			if (ImGui.Button("Scan"))
			{
				Task.Run(async () => await Plugin.Buttplug.ScanAsync());
			}

			ImGui.SameLine();
			if (ImGui.Button("Test Pulse"))
			{
				ConstantPattern p = new ConstantPattern();
				p.RunFor(1000);
			}
			ImGui.SameLine();

			ImGui.ProgressBar((float)Plugin.Devices.Intensity);

			ImGui.Text($"Patterns: {PatternBase.ActivePatterns.Count}");

			lock (PatternBase.ActivePatterns)
			{
				foreach (PatternBase pattern in PatternBase.ActivePatterns)
				{
					string ac = pattern.Active ? "O" : "X";
					ImGui.Text($"    {ac} {pattern.GetType().Name} - {pattern.DurationLeft}ms");
				}
			}

			ImGui.Text($"Devices: {Plugin.Devices.Count}");

			foreach (Device device in Plugin.Devices.All)
			{
				ImGui.Text($"    > {device.ClientDevice.Name}");
			}
		}
	}
}
