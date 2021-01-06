using System;
using System.Collections.Generic;
using System.Numerics;
using AetherSense.Patterns;
using AetherSense.Utils;
using ImGuiNET;
using Newtonsoft.Json;

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

		public bool Enabled
		{
			get => this.enabled;
			set => this.enabled = value;
		}

		[JsonIgnore]
		public bool IsAttached { get; private set; }

		public PatternBase Pattern { get; set; } = new ConstantPattern();

		public virtual void Attach()
		{
			this.IsAttached = true;
		}

		public virtual void Detach()
		{
			this.IsAttached = false;
		}

		public bool OnEditorGuiTop(int index)
		{
			bool keepEntry = true;
			ImGui.PushID("entry_" + index.ToString());

			string check = this.enabled ? "X" : " ";
			bool open = ImGui.CollapsingHeader(index.ToString(), ref keepEntry);
			ImGui.SameLine();
			ImGui.Text($"[{check}] {this.name}");

			if (open)
			{
				ImGui.Checkbox("Enable", ref this.enabled);
				ImGui.SameLine();
				ImGui.InputText("Name", ref this.name, 32);

				ImGui.Separator();
				ImGui.Columns(2);

				ImGui.Spacing();
				this.OnEditorGui();

				ImGui.NextColumn();

				if (ImGui.BeginCombo("Pattern", NameUtility.ToName(this.Pattern.GetType().Name)))
				{
					foreach (Type type in ConfigurationEditor.PatternTypes)
					{
						if (ImGui.Selectable(NameUtility.ToName(type.Name)))
						{
							this.Pattern = (PatternBase)Activator.CreateInstance(type);
						}
					}

					ImGui.EndCombo();
				}


				if (this.Pattern != null)
				{
					this.Pattern.OnEditorGuiTop();
				}

				ImGui.NextColumn();
				ImGui.Columns(1);
			}

			ImGui.PopID();

			return keepEntry;
		}

		public abstract void OnEditorGui();
	}
}
