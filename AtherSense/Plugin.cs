using Dalamud.Game.Command;
using Dalamud.Plugin;
using ImGuiNET;
using System.Numerics;
using System.Threading;

namespace AetherSense
{
	public class Plugin : IDalamudPlugin
	{
		public static DalamudPluginInterface DalamudPluginInterface;
		public static Configuration Configuration;

		public string Name => "AetherSense";

		private bool visible;

		public void Initialize(DalamudPluginInterface pluginInterface)
		{
			DalamudPluginInterface = pluginInterface;
			Configuration = DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

			DalamudPluginInterface.CommandManager.AddHandler("/sense", "Test command", OnSense);

			DalamudPluginInterface.UiBuilder.OnBuildUi += DrawUI;
		}

		public void Dispose()
		{
			DalamudPluginInterface.CommandManager.ClearHandlers();
			DalamudPluginInterface.Dispose();
		}

		private void OnSense(string args)
		{
			this.visible = true;
		}

		private void DrawUI()
		{
			if (!visible)
				return;

			float scale = ImGui.GetIO().FontGlobalScale;
			////Vector2 size = new Vector2(400 * scale, 300 * scale);

			////ImGui.SetNextWindowSize(size, ImGuiCond.Always);
			////ImGui.SetNextWindowSizeConstraints(size, size);

			if (ImGui.Begin("Aether Sense", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize))
			{
			}
			ImGui.End();
		}
	}
}
