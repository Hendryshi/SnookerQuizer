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
			try
			{
				//DateTime dtStamp = new DateTime(2021, 04, 23, 8, 0, 0);
				DateTime dtStamp = DateTime.Now;
				Log.Information("------------- Staring Process Snooker Quiz Program ---------------------");
				Log.Information(string.Format("------------------{0}-------------------------------------", dtStamp.ToString("MM/dd/yyyy HH:mm")));
				Log.Information("");

				if(Game.IsGameInit)
				{
					SnookerPlayerList = Processor.ImportPlayersInEvent();
					TranslatePlayerList = Processor.ImportTranslatePlayers();
					Processor.InitUserList();
				}

				if(Game.IsGameProcess)
				{
					MatchList = Processor.ImportMatchsInEvent(true);
					GamerList = Processor.ImportGamerList();

					Processor.ProcessMatchAndGamer(dtStamp);
					Processor.GenerateMail(dtStamp);

					if(!Game.CheckMail)
						Processor.SaveDatasToLocal();
				}

				if(Game.IsTest)
				{
					TestFunction.TestWhatYouWant();
					//TestFunction.TestMatchResult();
					//TestFunction.SendMail();
				}

				Log.Information("");
				Log.Information("--------------------------------------------");
				Log.Information("Execution completed, press a key to continue");
				Log.Information("--------------------------------------------");
				Log.Information("");
			}
			catch(Exception ex)
			{
				Log.Information("");
				Log.Information("-----------------------------------------------------");
				Log.Error(ex, "Execution completed with error, press a key to continue");
				Log.Information("-----------------------------------------------------");
				Log.Information("");
			}
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
