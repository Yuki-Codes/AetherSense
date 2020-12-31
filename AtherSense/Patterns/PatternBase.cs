using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AetherSense.Patterns
{
	public abstract class PatternBase
	{
		protected bool Active { get; private set; }

		private Task lastRunTask;

		public double DevicesIntensity
		{
			get => Plugin.Devices.Intensity;
			set => Plugin.Devices.Intensity = value;
		}

		public async Task RunFor(int duration)
		{
			this.Begin();
			await Task.Delay(duration);
			this.End();
		}

		public virtual void Begin()
		{
			this.Active = true;

			if (this.lastRunTask != null && !this.lastRunTask.IsCompleted)
				throw new Exception("Last pattern task did not complete");

			this.lastRunTask = Task.Run(this.Run);
		}

		public virtual void End()
		{
			this.Active = false;
		}

		protected abstract Task Run();
	}
}
