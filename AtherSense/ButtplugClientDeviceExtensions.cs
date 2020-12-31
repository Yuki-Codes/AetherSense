using System.Threading.Tasks;
using Buttplug;

namespace AetherSense
{
	public static class ButtplugClientDeviceExtensions
	{
		public static void Vibrate(this ButtplugClientDevice device, double intensity, int duration)
		{
			Task.Run(() => device.VibrateAsync(intensity, duration));
		}

		public static async Task VibrateAsync(this ButtplugClientDevice device, double intensity, int duration)
		{
			await device.SendVibrateCmd(intensity);
			await Task.Delay(duration);
			await device.SendStopDeviceCmd();
		}
	}
}
