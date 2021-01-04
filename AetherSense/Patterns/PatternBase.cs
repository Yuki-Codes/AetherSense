using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace AetherSense.Patterns
{
	public abstract class PatternBase
	{
		[JsonIgnore]
		public bool Active { get; private set; }

		[JsonIgnore]
		public int DurationLeft { get; private set; }

		protected double DevicesIntensity
		{
			get => Plugin.Devices.DesiredIntensity;
			set => Plugin.Devices.DesiredIntensity = value;
		}

		private Task lastRunTask;



		public abstract void OnEditorGui();

		public void RunFor(int duration)
		{
			if (this.Active)
			{
				PluginLog.Information("Extend pattern: " + this.GetType().Name + " for " + duration);
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
				PluginLog.Information("Extend pattern: " + this.GetType().Name + " for " + duration);
				this.DurationLeft = Math.Max(this.DurationLeft, duration);
				return;
			}

			this.DurationLeft = duration;

			PluginLog.Information("Run pattern: " + this.GetType().Name + " for " + duration);

			this.Begin();

			while (this.DurationLeft > 0)
			{
				await Task.Delay(100);
				this.DurationLeft -= 100;

				// Cap duration at 10 seconds
				this.DurationLeft = Math.Min(this.DurationLeft, 10000);
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

		protected abstract Task Run();
	}
}
