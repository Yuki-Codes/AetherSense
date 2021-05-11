using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Plugin;
using ImGuiNET;
using Newtonsoft.Json;

namespace AetherSense.Patterns
{
	public abstract class PatternBase
	{
		[JsonIgnore]
		public bool Active { get; private set; }

		[JsonIgnore]
		public int DurationLeft { get; private set; }

		[JsonIgnore]
		public double DevicesIntensity { get; protected set; } = 0;

		private Task lastRunTask;

		public void OnEditorGuiTop()
		{
			ImGui.SameLine();
			ImGui.PushButtonRepeat(true);
			if (ImGui.ArrowButton("##TestButton", ImGuiDir.Right))
			{
				this.RunFor(1000);
			}
			ImGui.PushButtonRepeat(false);

			this.OnEditorGui();
		}

		protected abstract void OnEditorGui();

		public void RunFor(int duration)
		{
			if (this.Active)
			{
				this.DurationLeft = Math.Max(this.DurationLeft, duration);
				return;
			}

			Task.Run(async () =>
			{
				await this.RunForAsync(duration);
			});
		}

		public async Task RunForAsync(int duration)
		{
			if (this.Active)
			{
				this.DurationLeft = Math.Max(this.DurationLeft, duration);
				return;
			}

			this.DurationLeft = duration;

			this.Begin();

			while (this.DurationLeft > 0)
			{
				await Task.Delay(100);
				this.DurationLeft -= 100;
			}

			this.End();
		}

		public virtual void Begin()
		{
			this.Active = true;

			if (this.lastRunTask != null && !this.lastRunTask.IsCompleted && !this.lastRunTask.IsFaulted)
				throw new Exception("Last pattern task did not complete");

			this.lastRunTask = Task.Run(this.Run);
		}

		public virtual void End()
		{
			this.Active = false;

			if (this.lastRunTask.IsFaulted)
			{
				PluginLog.Error(this.lastRunTask.Exception, "Error in pattern task");
			}
		}

		protected virtual Task Run()
		{
			return Task.CompletedTask;
		}
	}
}
