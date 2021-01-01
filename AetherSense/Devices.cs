using System.Collections.Generic;
using System.Threading.Tasks;
using Buttplug;

namespace AetherSense
{
	public class Devices
	{
		private List<Device> devices = new List<Device>();
		private double intensity;

		public double Intensity
		{
			get
			{
				return this.intensity;
			}

			set
			{
				this.intensity = value;

				foreach (Device device in this.devices)
				{
					device.Intensity = value;
				}
			}
		}

		public int Count => this.devices.Count;

		public IReadOnlyCollection<Device> All => this.devices.AsReadOnly();

		public void AddDevice(ButtplugClientDevice clientDevice)
		{
			Device device = new Device(clientDevice);
			this.devices.Add(device);
			device.Intensity = this.Intensity;
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

		public async Task Write()
		{
			foreach (Device device in this.devices)
			{
				await device.Write();
			}
		}
	}
}
