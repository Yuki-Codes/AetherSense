using System;
using System.Threading.Tasks;
using AetherSense;
using AetherSense.Patterns;
using Serilog;
using Serilog.Events;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			Run().Wait();
			Console.ReadLine();
		}

		private static async Task Run()
		{
			LoggerConfiguration loggers = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext();

			loggers.WriteTo.Logger(logger => logger.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose));

			Log.Logger = loggers.CreateLogger();
			Log.Logger.Information("Logger is initialized");

			Plugin plugin = new Plugin();

			// Hack to boot the plugin and get its write thread going
			_ = Task.Run(plugin.InitializeAsync);
			await Task.Delay(100);

			Log.Information("Constant");
			ConstantPattern c = new ConstantPattern();
			await c.RunForAsync(1000);
			Log.Information("Done");

			Log.Information("Pulse");
			PulsePattern p = new PulsePattern();
			p.DownIntensity = 0.5;
			p.UpDuration = 500;
			await p.RunForAsync(10000);
			Log.Information("Done");
		}
	}
}
