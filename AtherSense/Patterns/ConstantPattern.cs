using System.Threading.Tasks;

namespace AetherSense.Patterns
{
	public class ConstantPattern : PatternBase
	{
		public double Intensity = 1.0;

		public override void Begin()
		{
			base.Begin();
			this.DevicesIntensity += this.Intensity;
		}

		public override void End()
		{
			this.DevicesIntensity -= this.Intensity;
		}

		protected override Task Run()
		{
			return Task.CompletedTask;
		}
	}
}
