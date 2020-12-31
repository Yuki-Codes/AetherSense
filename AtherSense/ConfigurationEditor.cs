using AetherSense.Patterns;
using ImGuiNET;

namespace AetherSense
{
	public static class ConfigurationEditor
	{
		public static void OnGui()
		{
			if (ImGui.Button("Constant 1 second"))
			{
				ConstantPattern p = new ConstantPattern();
				p.RunFor(1000);
			}

			if (ImGui.Button("Pulse 10 second"))
			{
				PulsePattern p = new PulsePattern();
				p.UpDuration = 500;
				p.RunFor(10000);
			}
		}
	}
}
