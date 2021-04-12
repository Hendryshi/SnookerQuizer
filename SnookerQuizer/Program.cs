using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using SnookerQuizer.Model;
using SnookerQuizer.Helper;
using Serilog;
using Serilog.Exceptions;
using System.Configuration;

namespace SnookerQuizer
{
	class Program
	{
		static void Main(string[] args)
		{
			ConfigureSerilog();
			//List<Match> matchs = HttpHelper.GetData<Match>("http://api.snooker.org", "?e=397&r=1&n=5");

			//foreach(Match m in matchs)
			//{
			//	Console.WriteLine("{0}", m.IdPlayer1);
			//	//Log.Information(m.StartDate);
			//	//Log.Warning(m.StartDate);
			//	//Log.Error(m.StartDate);
			//	//Log.Fatal(m.StartDate);
			//}

			List<SnookerPlayer> players = HttpHelper.GetData<SnookerPlayer>("http://api.snooker.org", "?t=9&e=1014");
			foreach (SnookerPlayer p in players)
			{
				Console.WriteLine("{0}", p.NameDisplay());
				//Log.Information(m.StartDate);
				//Log.Warning(m.StartDate);
				//Log.Error(m.StartDate);
				//Log.Fatal(m.StartDate);
			}

			Console.ReadKey();
		}

		private static void ConfigureSerilog()
		{
			Log.Logger = new LoggerConfiguration()
				.Enrich.WithExceptionDetails()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File(
					System.Configuration.ConfigurationManager.AppSettings["logDebugPath"],
					 rollingInterval: RollingInterval.Day
				)
				.WriteTo.File(
					System.Configuration.ConfigurationManager.AppSettings["logErrorPath"],
					 rollingInterval: RollingInterval.Day,
					 restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error
				)
				.CreateLogger();
		}
	}
}
