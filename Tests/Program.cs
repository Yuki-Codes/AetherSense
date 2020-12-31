using System;
using AetherSense;
using Serilog;
using Serilog.Events;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			LoggerConfiguration loggers = new LoggerConfiguration().MinimumLevel.Verbose().Enrich.FromLogContext();

			loggers.WriteTo.Logger(logger => logger.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose));

			Log.Logger = loggers.CreateLogger();
			Log.Logger.Information("Logger is initialized");

			Plugin p = new Plugin();
			p.InitializeAsync().Wait();

			Console.ReadLine();
		}
	}
}
