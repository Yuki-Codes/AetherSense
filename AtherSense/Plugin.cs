using AetherSense.Patterns;
using Buttplug;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AetherSense
{
	public class Plugin : IDalamudPlugin
	{
		public static DalamudPluginInterface DalamudPluginInterface;
		public static Configuration Configuration;
		public static Devices Devices;

		public string Name => "AetherSense";

		private bool enabled;
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
			this.enabled = true;

			try
			{
				Devices = new Devices();

				ButtplugClient client = new ButtplugClient("Aether Sense");
				client.DeviceAdded += this.OnDeviceAdded;
				client.DeviceRemoved += this.OnDeviceRemoved;
				client.ScanningFinished += this.OnScanningFinished;

				PluginLog.Information("Connect to embedded buttplug server");
				ButtplugEmbeddedConnectorOptions connectorOptions = new ButtplugEmbeddedConnectorOptions();
				connectorOptions.ServerName = "Aether Sense Server";
				await client.ConnectAsync(connectorOptions);

				PluginLog.Information("Scan for devices");
				await client.ScanAsync();

				while (this.enabled)
				{
					await Devices.Write();

					// 33 ms = 30fps max
					await Task.Delay(32);
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

			this.enabled = false;
		}

		private void DrawUI()
		{
			if (!visible)
				return;

			////float scale = ImGui.GetIO().FontGlobalScale;

			if (ImGui.Begin("Aether Sense", ref this.visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse | ImGuiWindowFlags.NoResize))
			{
			}

			ImGui.End();
		}

		private void OnDeviceAdded(object sender, DeviceAddedEventArgs e)
		{
			PluginLog.Information("Device {0} added", e.Device.Name);
			Devices.AddDevice(e.Device);
		}

		private void OnDeviceRemoved(object sender, DeviceRemovedEventArgs e)
		{
			PluginLog.Information("Device {0} removed", e.Device.Name);
			Devices.RemoveDevice(e.Device);
		}

		private void OnSense(string args)
		{
			this.visible = true;
		}
	}
}
