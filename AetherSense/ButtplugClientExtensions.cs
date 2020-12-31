using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Buttplug;

namespace AetherSense
{
	public static class ButtplugclientExtensions
	{
		public static async Task ScanAsync(this ButtplugClient client)
		{
			bool scanning = true;
			await client.StartScanningAsync();
			client.ScanningFinished += (s, e) => scanning = false;

			while (scanning)
			{
				await Task.Delay(100);
			}
		}
	}
}
