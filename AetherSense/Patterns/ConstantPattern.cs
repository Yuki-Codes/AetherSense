using System.Threading.Tasks;
using ImGuiNET;

namespace AetherSense.Patterns
{
	public class ConstantPattern : PatternBase
	{
		private float intensity = 1.0f;

		public float Intensity
		{
			get => this.intensity;
			set => this.intensity = value;
		}

		public override void Begin()
		{
			base.Begin();
			this.DevicesIntensity = this.Intensity;
		}

		protected override void OnEditorGui()
		{
			ImGui.SliderFloat("Intensity", ref this.intensity, 0.0f, 1.0f);
		}
	}
}
