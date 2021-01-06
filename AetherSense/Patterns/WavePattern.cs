using System;
using System.Threading.Tasks;
using ImGuiNET;

namespace AetherSense.Patterns
{
	public class WavePattern : PatternBase
	{
		public float MaxIntensity = 1.0f;
		public float MinIntensity = 0.0f;
		public int Frequency = 1000;

		protected override void OnEditorGui()
		{
			ImGui.SliderFloat("Range", ref this.MaxIntensity, 0.0f, 1.0f);
			ImGui.SliderFloat("Min", ref this.MinIntensity, 0.0f, 1.0f);
			ImGui.InputInt("Frequency", ref this.Frequency);
		}

		protected override async Task Run()
		{
			double t = 0;
			while (this.Active)
			{
				double intensity = 0.5 + (Math.Sin(t * (4000 / this.Frequency)) * 0.5f);
				t += 0.016;

				this.DevicesIntensity = this.MinIntensity + (intensity * this.MaxIntensity);

				await Task.Delay(16);
			}
		}
	}
}
