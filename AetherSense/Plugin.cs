using Buttplug;
using Dalamud.Configuration;
using Dalamud.Plugin;
using ImGuiNET;
using System;
using System.Threading.Tasks;
using AetherSense.Triggers;
using System.IO;
using AetherSense.Patterns;
using Dalamud.IoC;
using Dalamud.Game.Command;
using Dalamud.Logging;
using Dalamud.Game.Gui;

namespace AetherSense
{
    public sealed class Plugin : IDalamudPlugin
    {
        [PluginService] public static ChatGui ChatGui { get; private set; } = null!;
        [PluginService] public static CommandManager CommandManager { get; private set; } = null!;

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


        public Plugin(DalamudPluginInterface pluginInterface)
        {
            DalamudPluginInterface = pluginInterface;

            pluginInterface.Inject(this);

			////Configuration = DalamudPluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            CommandManager.AddHandler("/sense", "Opens the Aether Sense configuration window", this.OnShowConfiguration);
            CommandManager.AddHandler("/senseDebug", "Opens the Aether Sense debug window", this.OnShowDebug);
            DalamudPluginInterface.UiBuilder.Draw += OnGui;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += () => this.configurationIsVisible = true;

			Devices.Clear();

			Configuration = Configuration.Load();
			Task.Run(this.Connect);
			_ = Task.Run(this.Run);
			this.LoadTriggers();
		}

		/// <summary>
		/// Initializes the plugin in mock test mode outside of the game
		/// </summary>
		/// <param name="configuration">the plugin configuration to use</param>
		/// <returns>an action to be invoked for ImGUI drawing</returns>
		public Action InitializeMock()
		{
			Configuration = Configuration.Load();
			Task.Run(this.Connect);
			_ = Task.Run(this.Run);
			this.LoadTriggers();

			Task.Run(async () =>
			{
				await Task.Delay(500);
				this.OnShowDebug(null);
				this.OnShowConfiguration(null);
			});

			return this.OnGui;
		}

		public async Task Connect()
		{
			if (!Configuration.Enabled)
				return;

			PluginLog.Information("Connecting to buttplug");
			
			try
			{
				if (Buttplug == null)
				{
					Buttplug = new ButtplugClient("Aether Sense");
					Buttplug.DeviceAdded += this.OnDeviceAdded;
					Buttplug.DeviceRemoved += this.OnDeviceRemoved;
					Buttplug.ScanningFinished += (o, e) =>
					{
						Task.Run(async () =>
						{
							await Task.Delay(1000);
							try
							{
								await Buttplug.StartScanningAsync();
							}
							catch (Exception)
							{
							}
						});
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
						ButtplugWebsocketConnectorOptions wsOptions = new ButtplugWebsocketConnectorOptions(new Uri(Configuration.ServerAddress));
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
			}
			catch (Exception ex)
			{
				PluginLog.Error(ex, "Buttplug exception");
			}
		}

		private async Task Run()
		{
			this.enabled = true;
			PluginLog.Information("Running");
			while (this.enabled)
			{
				await Devices.Write(Configuration.Triggers, 250);
				await Task.Delay(250);
			}
		}

		private void LoadTriggers()
		{
			PluginLog.Information("Loading Triggers");
			foreach (TriggerBase trigger in Configuration.Triggers)
			{
				if (!trigger.Enabled || Plugin.DalamudPluginInterface == null)
					continue;

				PluginLog.Information("    > " + trigger.Name);
				trigger.Attach();
			}
		}

		private void UnloadTriggers()
		{
			// Unload triggers as we are stopping.
			PluginLog.Information("Unloading Triggers");
			foreach (TriggerBase trigger in Configuration.Triggers)
			{
				if (Plugin.DalamudPluginInterface == null)
					continue;

				trigger.Detach();
			}
		}

		public void Dispose()
		{
			DalamudPluginInterface.UiBuilder.Draw -= OnGui;
			CommandManager.ClearHandlers();
			DalamudPluginInterface.Dispose();

			this.UnloadTriggers();
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
				Task.Run(this.Connect);
				_ = Task.Run(this.Run);
				this.LoadTriggers();
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
			this.UnloadTriggers();
			this.configurationIsVisible = true;
		}

		private void OnShowDebug(string args)
		{
			this.debugVisible = true;
		}
	}
}
