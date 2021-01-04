using Buttplug;
using Dalamud.Configuration;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Threading.Tasks;
using AetherSense.Triggers;
using System.IO;
using AetherSense.Patterns;

namespace AetherSense
{
	public class Plugin : IDalamudPlugin
	{
		public static DalamudPluginInterface DalamudPluginInterface;
		public static Configuration Configuration;
		public static Devices Devices = new Devices();
		public static ButtplugClient Buttplug;

		public static ButtplugConnectorException lastConnectionError;

		public string Name => "AetherSense";

		private bool enabled;
		private bool debugVisible;
		private bool configurationIsVisible;
		private bool configurationWasVisible = false;

		public void Initialize(DalamudPluginInterface pluginInterface)
		{
			DalamudPluginInterface = pluginInterface;
			////Configuration = DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
			Configuration = Configuration.Load();
			DalamudPluginInterface.CommandManager.AddHandler("/sense", "Opens the Aether Sense configuration window", this.OnShowConfiguration);
			DalamudPluginInterface.CommandManager.AddHandler("/senseDebug", "Opens the Aether Sense debug window", this.OnShowDebug);
			DalamudPluginInterface.UiBuilder.OnBuildUi += OnGui;
			DalamudPluginInterface.UiBuilder.OnOpenConfigUi += (s, e) => this.configurationIsVisible = true;

			Task.Run(this.InitializeAsync);
		}

		/// <summary>
		/// Initializes the plugin in mock test mode outside of the game
		/// </summary>
		/// <param name="configuration">the plugin configuration to use</param>
		/// <returns>an action to be invoked for ImGUI drawing</returns>
		public Action InitializeMock()
		{
			this.configurationIsVisible = true;
			this.debugVisible = true;
			Configuration = Configuration.Load();
			Task.Run(this.InitializeAsync);
			return this.OnGui;
		}

		public async Task InitializeAsync()
		{
			if (!Configuration.Enabled)
				return;

			PluginLog.Information("Initializing Buttplug Interface");
			this.enabled = true;

			try
			{
				if (Buttplug == null)
				{
					Buttplug = new ButtplugClient("Aether Sense");
					Buttplug.DeviceAdded += this.OnDeviceAdded;
					Buttplug.DeviceRemoved += this.OnDeviceRemoved;
					Buttplug.ScanningFinished += (o, e) =>
					{
						PluginLog.Information("Scan for devices complete");
					/*Task.Run(async () =>
					{
						await client.StopScanningAsync();
						await client.StartScanningAsync();
					});*/
					};
				}

				if (!Buttplug.Connected)
				{
					lastConnectionError = null;

					try
					{

						/*PluginLog.Information("Connect to embedded buttplug server");
						ButtplugEmbeddedConnectorOptions connectorOptions = new ButtplugEmbeddedConnectorOptions();
						connectorOptions.ServerName = "Aether Sense Server";*/

						PluginLog.Information("Connect to buttplug local server");
						ButtplugWebsocketConnectorOptions wsOptions = new ButtplugWebsocketConnectorOptions(new Uri("ws://127.0.0.1:12345"));
						await Buttplug.ConnectAsync(wsOptions);
					}
					catch (ButtplugConnectorException ex)
					{
						lastConnectionError = ex;
						this.OnShowConfiguration(null);
						throw;
					}
				}

				PluginLog.Information("Scan for devices");
				await Buttplug.StartScanningAsync();

				PluginLog.Information("Loading Triggers");
				foreach (TriggerBase trigger in Configuration.Triggers)
				{
					if (!trigger.Enabled || Plugin.DalamudPluginInterface == null)
						continue;

					PluginLog.Information("    > " + trigger.Name);
					trigger.Attach();
				}

				PluginLog.Information("Running");
				while (this.enabled)
				{
					await Devices.Write(32);

					// 33 ms = 30fps max
					await Task.Delay(32);
				}

				// Unload triggers as we are stopping.
				PluginLog.Information("Unloading Triggers");
				foreach (TriggerBase trigger in Configuration.Triggers)
				{
					if (Plugin.DalamudPluginInterface == null)
						continue;

					trigger.Detach();
				}
			}
			catch (Exception ex)
			{
				PluginLog.Error(ex, "Buttplug exception");
			}
		}

		public void Dispose()
		{
			DalamudPluginInterface.CommandManager.ClearHandlers();
			DalamudPluginInterface.Dispose();

			this.enabled = false;
		}

		public void OnGui()
		{
			if (this.debugVisible && ImGui.Begin("Aether Sense Status", ref this.debugVisible))
			{
				DebugWindow.OnGui();
				ImGui.End();
			}
			
			if (this.configurationIsVisible && ImGui.Begin("Aether Sense", ref this.configurationIsVisible))
			{
				this.configurationWasVisible = true;
				ConfigurationEditor.OnGui();
				ImGui.End();
			}

			// Have we just closed the config window
			if (!this.configurationIsVisible && this.configurationWasVisible)
			{
				this.configurationWasVisible = false;
				Configuration.Save();
				Task.Run(this.InitializeAsync);
				return;
			}
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

		private void OnShowConfiguration(string args)
		{
			this.enabled = false;
			this.configurationIsVisible = true;
		}

		private void OnShowDebug(string args)
		{
			this.debugVisible = true;
		}
	}
}
