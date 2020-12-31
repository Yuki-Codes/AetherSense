using System.Threading.Tasks;

namespace AetherSense.Patterns
{
	public class PulsePattern : PatternBase
	{
		public double UpIntensity = 1.0;
		public int UpDuration = 1000;
		public double DownIntensity = 0.0;
		public int DownDuration = 1000;

		protected override async Task Run()
		{
			while(this.Active)
			{
				this.DevicesIntensity += this.UpIntensity;
				await Task.Delay(this.UpDuration);
				this.DevicesIntensity -= this.UpIntensity;

				if (!this.Active)
					return;

				this.DevicesIntensity += this.DownIntensity;
				await Task.Delay(this.DownDuration);
				this.DevicesIntensity -= this.DownIntensity;
			}
		}
	}
}
