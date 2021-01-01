using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dalamud.Plugin;
using Newtonsoft.Json;

namespace AetherSense.Patterns
{
	public abstract class PatternBase
	{
		public static List<PatternBase> ActivePatterns = new List<PatternBase>();

		[JsonIgnore]
		public bool Active { get; private set; }

		[JsonIgnore]
		public int DurationLeft { get; private set; }

		protected double DevicesIntensity
		{
			get => Plugin.Devices.Intensity;
			set => Plugin.Devices.Intensity = value;
		}

		private Task lastRunTask;



		public abstract void OnEditorGui();

		public void RunFor(int duration)
		{
			if (this.Active)
			{
				PluginLog.Information("Extend pattern: " + this.GetType().Name + " for " + duration);
				this.DurationLeft += duration;
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
				this.DurationLeft += duration;
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
			lock (ActivePatterns)
			{
				ActivePatterns.Add(this);
			}

			this.Active = true;

			if (this.lastRunTask != null && !this.lastRunTask.IsCompleted && !this.lastRunTask.IsFaulted)
				throw new Exception("Last pattern task did not complete");

			this.lastRunTask = Task.Run(this.Run);
		}

		public virtual void End()
		{
			this.Active = false;

			lock (ActivePatterns)
			{
				ActivePatterns.Remove(this);
			}

			if (this.lastRunTask.IsFaulted)
			{
				PluginLog.Error(this.lastRunTask.Exception, "Error in pattern task");
			}
		}

		protected abstract Task Run();
	}
}
