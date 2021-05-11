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
		public int DeviceGroup { get; set; } = 0;

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
				ImGui.InputText("Name", ref this.name, 32);

				ImGui.Columns(2);

				ImGui.Checkbox("Enable", ref this.enabled);

				ImGui.NextColumn();

				// Group selector
				string label = this.DeviceGroup == 0 ? "All" : "Group " + this.DeviceGroup;
				if (ImGui.BeginCombo($"Devices", label))
				{
					for (int n = 0; n < 10; n++)
					{
						label = n == 0 ? "All" : "Group " + n;
						if (ImGui.Selectable(label, n == this.DeviceGroup))
						{
							this.DeviceGroup = n;
						}
					}

					ImGui.EndCombo();
				}

				ImGui.Columns(1);
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
