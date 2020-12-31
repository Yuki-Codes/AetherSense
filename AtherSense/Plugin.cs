using Buttplug;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Threading.Tasks;

namespace AetherSense
{
	public class Plugin : IDalamudPlugin
	{
		public static DalamudPluginInterface DalamudPluginInterface;
		public static Configuration Configuration;

		public static ButtplugClient ButtplugClient;

		public string Name => "AetherSense";

		private bool visible;

		public void Initialize(DalamudPluginInterface pluginInterface)
		{
			DalamudPluginInterface = pluginInterface;
			Configuration = DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			DalamudPluginInterface.CommandManager.AddHandler("/sense", "Scans for new intimate devices", OnSense);
			DalamudPluginInterface.UiBuilder.OnBuildUi += DrawUI;

			Task.Run(this.InitializeAsync);
		}

		public async Task InitializeAsync()
		{
			PluginLog.Information("Initializing Buttplug Interface");

			try
			{
				ButtplugClient = new ButtplugClient("Aether Sense");
				ButtplugClient.DeviceAdded += this.OnDeviceAdded;
				ButtplugClient.ScanningFinished += this.OnScanningFinished;

				PluginLog.Information("Connect to embedded buttplug server");
				ButtplugEmbeddedConnectorOptions connectorOptions = new ButtplugEmbeddedConnectorOptions();
				connectorOptions.ServerName = "Aether Sense Server";
				await ButtplugClient.ConnectAsync(connectorOptions);

				PluginLog.Information("Scan for devices");
				await ButtplugClient.ScanAsync();

				PluginLog.Information("Devices {0}", ButtplugClient.Devices.Length);
				foreach (ButtplugClientDevice device in ButtplugClient.Devices)
				{
					PluginLog.Information("    Device {0} {1}", device.Index, device.Name);
					await device.VibrateAsync(1.0, 1000);
				}
			}
			catch (Exception ex)
			{
				PluginLog.Error(ex, "Buttplug exception");
			}
		}

		private void OnScanningFinished(object sender, EventArgs e)
		{
			PluginLog.Information("Scan for devices complete");
		}

		public void Dispose()
		{
			DalamudPluginInterface.CommandManager.ClearHandlers();
			DalamudPluginInterface.Dispose();
		}

		private void DrawUI()
		{
			if (!visible)
				return;

			////float scale = ImGui.GetIO().FontGlobalScale;

			if (ImGui.Begin("Aether Sense", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize))
			{
				ImGui.Text($"Devices: {ButtplugClient.Devices.Length}");

				foreach (ButtplugClientDevice device in ButtplugClient.Devices)
				{
					ImGui.Text(device.Name);
				}
			}

			ImGui.End();
		}

		private void OnDeviceAdded(object sender, DeviceAddedEventArgs e)
		{
			PluginLog.Information("Device {0} connected", e.Device.Name);
		}

		public static void Vibrate(double strength, int duration)
		{
			// TODO: configuration for enabling/disabling/intensity multiply
			foreach (ButtplugClientDevice device in ButtplugClient.Devices)
			{
				device.Vibrate(strength, duration);
			}
		}

		private void OnSense(string args)
		{
			this.visible = true;
		}
	}
}
