using AetherSense.Patterns;
using ImGuiNET;
using AetherSense.Triggers;
using System;
using System.Collections.Generic;

namespace AetherSense
{
	public static class ConfigurationEditor
	{
		private static Dictionary<string, Type> triggerTypes = new Dictionary<string, Type>()
		{
			{ "Chat Trigger", typeof(ChatTrigger) },
		};

		private static string[] addTriggerOptions;

		static ConfigurationEditor()
		{
			List<string> op = new List<string>();
			op.Add("...");
			op.AddRange(triggerTypes.Keys);
			addTriggerOptions = op.ToArray();
		}

		public static void OnGui()
		{
			int indexToDelete = -1;
			for (int i = 0; i < Plugin.Configuration.Triggers.Count; i++)
			{
				bool keep = Plugin.Configuration.Triggers[i].OnEditorGuiTop(i);
				if (!keep)
				{
					indexToDelete = i;
				}
			}

			if (indexToDelete != -1)
				Plugin.Configuration.Triggers.RemoveAt(indexToDelete);
			
			ImGui.Separator();
			int currentItem = 0;
			ImGui.Combo("Add Trigger", ref currentItem, addTriggerOptions, addTriggerOptions.Length);
	
			if (currentItem != 0)
			{
				string selected = addTriggerOptions[currentItem];
				Type t = triggerTypes[selected];
				TriggerBase trigger = (TriggerBase)Activator.CreateInstance(t);
				Plugin.Configuration.Triggers.Add(trigger);
				trigger.Name = "New " + selected;
			}

			ImGui.Spacing();

			if (ImGui.Button("Constant 1 second"))
			{
				ConstantPattern p = new ConstantPattern();
				p.RunFor(1000);
			}
			ImGui.SameLine();
			if (ImGui.Button("Pulse 10 second"))
			{
				PulsePattern p = new PulsePattern();
				p.UpDuration = 500;
				p.RunFor(10000);
			}
			ImGui.SameLine();
			if (ImGui.Button("Save"))
			{
				Plugin.Configuration.Save();
			}
		}
	}
}
