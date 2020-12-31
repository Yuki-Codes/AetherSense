using System.Numerics;
using AetherSense.Patterns;
using ImGuiNET;

namespace AetherSense.Triggers
{
	public abstract class TriggerBase
	{
		private string name = "";
		private bool enabled = true;

		public string Name
		{
			get => this.name;
			set => this.name = value;
		}

		public PatternBase Pattern { get; set; } = new ConstantPattern();

		public abstract void Attach();
		public abstract void Detach();

		public bool OnEditorGuiTop(int index)
		{
			bool keepEntry = true;
			bool open = ImGui.CollapsingHeader(index.ToString(), ref keepEntry);
			ImGui.SameLine();

			string check = this.enabled ? "X" : " ";
			ImGui.Text($"[{check}] {this.name}");

			if (open)
			{
				ImGui.InputText("Name", ref this.name, 32);
				ImGui.Checkbox("Enable", ref this.enabled);

				this.OnEditorGui();
			}


			return keepEntry;
		}

		public abstract void OnEditorGui();
	}
}
