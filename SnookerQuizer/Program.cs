using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SnookerQuizer.Model;
using SnookerQuizer.Helper.Config;
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
		public static int WorldSnookerId = Convert.ToInt32(ConfigurationManager.AppSettings["WorldSnookerId"]);
		
		public static string PlayersInEventXml = "Players.xml";
		public static string TranslatePlayersXml = "TranslatePlayers.xml";
		public static string MatchsInEventXml = "Matchs.xml";
		public static string GamerXml = @"Gamer{0}-{1}.xml";

		public static List<SnookerPlayer> SnookerPlayerList;
		public static List<TranslatePlayer> TranslatePlayerList;
		public static List<Match> MatchList;
		public static List<GamerInfo> GamerList;


		static void Main(string[] args)
		{
			ConfigureSerilog();

			if(Game.IsGameInit)
			{
				SnookerPlayerList = Processor.ImportPlayersInEvent();
				TranslatePlayerList = Processor.ImportTranslatePlayers();
				Processor.InitUserList();
			}

			if(Game.IsGameProcess)
			{
				MatchList = Processor.ImportMatchsInEvent();
				GamerList = Processor.ImportGamerList();
				DateTime dtStamp = DateTime.Now;
				Processor.ProcessMatchAndGamer(dtStamp);
				Processor.GenerateMail(dtStamp);


				Processor.SaveDatasToLocal();
			}

			bool isTest = true;

			if(isTest)
			{
				MatchList = Processor.ImportMatchsInEvent();
				//TestFunction.SendMail();
				//TestFunction.SaveGamer();
			}
			
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
