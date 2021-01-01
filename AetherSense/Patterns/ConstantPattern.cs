using System.Threading.Tasks;
using ImGuiNET;

namespace AetherSense.Patterns
{
	public class ConstantPattern : PatternBase
	{
		public double Intensity { get; set; } = 1.0;

		public override void Begin()
		{
			base.Begin();
			this.DevicesIntensity += this.Intensity;
		}

		public override void End()
		{
			this.DevicesIntensity -= this.Intensity;
		}

		public override void OnEditorGui()
		{
			ImGui.Text("Hello There");
		}

		protected override Task Run()
		{
			return Task.CompletedTask;
		}
	}
}
