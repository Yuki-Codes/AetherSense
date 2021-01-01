using System;
using System.Threading.Tasks;
using Buttplug;

namespace AetherSense
{
	public class Device
	{
		public readonly ButtplugClientDevice ClientDevice;
		private double lastIntensity;

		public double Intensity { get; set; }

		public Device(ButtplugClientDevice clientDevice)
		{
			this.ClientDevice = clientDevice;
		}

		public async Task Write()
		{
			if (this.Intensity == this.lastIntensity)
				return;

			this.lastIntensity = this.Intensity;

			double i = Math.Max(this.Intensity, 0);
			i = Math.Min(i, 1);

			if (i <= 0)
			{
				await this.ClientDevice.SendStopDeviceCmd();
			}
			else
			{
				await this.ClientDevice.SendVibrateCmd(i);
			}
		}
	}
}
