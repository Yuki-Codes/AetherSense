using System.Threading.Tasks;
using ImGuiNET;

namespace AetherSense.Patterns
{
	public class AlternatingPattern : PatternBase
	{
		public float AIntensity = 1.0f;
		public int ADuration = 1000;
		public float BIntensity = 0.0f;
		public int BDuration = 1000;

		protected override void OnEditorGui()
		{
			ImGui.SliderFloat("A Intensity", ref this.AIntensity, 0.0f, 1.0f);
			ImGui.InputInt("A Duration", ref this.ADuration);
			ImGui.SliderFloat("B Intensity", ref this.BIntensity, 0.0f, 1.0f);
			ImGui.InputInt("B Duration", ref this.BDuration);
		}

		protected override async Task Run()
		{
			while(this.Active)
			{
				this.DevicesIntensity = this.AIntensity;
				await Task.Delay(this.ADuration);

				if (!this.Active)
					return;

				this.DevicesIntensity = this.BIntensity;
				await Task.Delay(this.BDuration);
			}
		}
	}
}
