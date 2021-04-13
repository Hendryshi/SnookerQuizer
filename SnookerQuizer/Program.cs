using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnookerQuizer.Model;
using SnookerQuizer.Helper;
using SnookerQuizer.CoreProcess;
using Serilog;
using Serilog.Exceptions;
using System.Configuration;
using System.IO;

namespace SnookerQuizer
{
	class Program
	{
		public static int IdEvent = Convert.ToInt32(ConfigurationManager.AppSettings["Event"]);
		public static string PlayersInEventXml = @"Event\players.xml";
		public static string MatchsInEventXml = @"Event\matchs.xml";
		public static string GamerFolder = @"Gamer\{0}.xml";

		public static List<SnookerPlayer> SnookerPlayerList;
		public static List<Match> MatchList;
		public static List<GamerInfo> GamerList;

		public 

		static void Main(string[] args)
		{
			ConfigureSerilog();

			SnookerPlayerList = Processor.ImportPlayersInEvent();
			MatchList = Processor.ImportMatchsInEvent();


			GamerList = Processor.ImportGamerList();

			Processor.CalculatePoint();
			Processor.SaveDatasToLocal();
			
			
			//Test Function
			//TestFunction.SendMail();

			Console.WriteLine("Execution completed, press a key to continue");
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
