using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AetherSense.Triggers;
using Buttplug;
using Dalamud.Plugin;

namespace AetherSense
{
	public class Devices
	{
		private List<Device> devices = new List<Device>();

		public double DesiredIntensity { get; private set; } = 0;
		public double Maximum { get; private set; } = 1.0;
		public double CurrentIntensity { get; set; }

		public int Count => this.devices.Count;

		public IReadOnlyCollection<Device> All => this.devices.AsReadOnly();

		public void Clear()
		{
			this.devices.Clear();
		}

		public void AddDevice(ButtplugClientDevice clientDevice)
		{
			Device device = new Device(clientDevice);
			this.devices.Add(device);
			device.Intensity = this.CurrentIntensity;
		}

		public void RemoveDevice(ButtplugClientDevice clientDevice)
		{
			foreach (Device device in this.devices)
			{
				if (device.ClientDevice == clientDevice)
				{
					this.devices.Remove(device);
					return;
				}
			}
		}

		public async Task Write(List<TriggerBase> triggers, int delta)
		{
			this.DesiredIntensity = 0;
			foreach (TriggerBase trigger in triggers)
			{
				if (!trigger.Enabled || trigger.Pattern == null)
					continue;

				if (!trigger.Pattern.Active)
					continue;

				this.DesiredIntensity += trigger.Pattern.DevicesIntensity;
			}

			// Get the maximum intensity
			this.Maximum = Math.Max(this.DesiredIntensity, this.Maximum);
			
			// drag the max value down towards 1.0 at a rate of 0.001 per ms
			this.Maximum -= delta * 0.001;
			this.Maximum = Math.Max(1.0, this.Maximum);

			this.CurrentIntensity = this.DesiredIntensity / this.Maximum;

			if (!Plugin.Configuration.Enabled)
				this.CurrentIntensity = 0;

			foreach (Device device in this.devices)
			{
				try
				{
					device.Intensity = this.CurrentIntensity;
					await device.Write();
				}
				catch (Exception ex)
				{
					PluginLog.Error(ex, "Failed to write to device");
				}
			}
		}
	}
}
