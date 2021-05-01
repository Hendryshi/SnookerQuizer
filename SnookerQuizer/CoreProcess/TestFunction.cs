using System;
using System.Collections.Generic;
using System.IO;
using SnookerQuizer.Helper;
using SnookerQuizer.Helper.Config;
using SnookerQuizer.Model;
using Serilog;
using System.Linq;

namespace SnookerQuizer.CoreProcess
{
	class TestFunction
	{
		public static void SendMail()
		{
			string HTMLBody = File.ReadAllText(@"C:\Users\SHI YEJIA\Desktop\GameResult-20210429.html");
			
			MailHelper.SendMail("yejia.shi@hotmail.com", "Result", HTMLBody);
		}

		public static void SaveGamer()
		{
			List<Quiz> lstQuiz = new List<Quiz>();
			List<GamePoint> lstPoint = new List<GamePoint>();

			lstQuiz.Add(new Quiz() { IdWinner = 376, GamePoint = 12  });
			lstQuiz.Add(new Quiz() { IdWinner = 376});
			lstPoint.Add(new GamePoint() { dtPoint = DateTime.Today, valPoint = 15 });

			GamerInfo gi1 = new GamerInfo("Yejia", 1014, lstQuiz, lstPoint);

			XMLHelper.SaveToXML(gi1, string.Format(Program.GamerXml, Program.IdEvent, gi1.UserName));
		}

		public static void TestWhatYouWant()
		{
			Program.SnookerPlayerList = Processor.ImportPlayersInEvent();
			Program.TranslatePlayerList = Processor.ImportTranslatePlayers();
			Program.MatchList = Processor.ImportMatchsInEvent(false, false);


			foreach(Match m in Program.MatchList)
			{
				string result = string.Join("<br />", m.GetSessionDateList().Where(s => s.Date == DateTime.Today.Date).Select(l => l.AddHours(8).ToString("HH:mm")));
				if(!string.IsNullOrEmpty(result))
					Console.WriteLine(result);
			}
			Console.ReadKey();
		}

		public static void TestMatchResult()
		{
			Program.SnookerPlayerList = Processor.ImportPlayersInEvent();
			Program.TranslatePlayerList = Processor.ImportTranslatePlayers();
			Program.MatchList = Processor.ImportMatchsInEvent(false, false);
			Program.GamerList = Processor.ImportGamerList();

			//Day 1
			DateTime dtStamp1 = new DateTime(2021, 04, 18, 1, 5, 0);
			UpdateMatch(1014, 7, 1, null, new DateTime(2021, 04, 17, 20, 5, 0), null, 1, 10, 4);
			UpdateMatch(1014, 7, 4, null, new DateTime(2021, 04, 17, 20, 5, 0), null, 2, 7, 10);
			UpdateMatch(1014, 7, 9, null, new DateTime(2021, 04, 17, 20, 5, 0), null, 1, 10, 5);
			Processor.ProcessMatchAndGamer(dtStamp1);
			Processor.GenerateMail(dtStamp1);

			//DateTime dtStamp2 = new DateTime(2021, 04, 19, 1, 5, 0);
			//UpdateMatch(1014, 7, 10, null, new DateTime(2021, 04, 18, 20, 5, 0), null, 1, 10, 9);
			//Processor.ProcessMatchAndGamer(dtStamp2);
			//Processor.GenerateMail(dtStamp2);

			//DateTime dtStamp3 = new DateTime(2021, 04, 20, 1, 5, 0);
			//UpdateMatch(1014, 7, 11, null, new DateTime(2021, 04, 19, 20, 5, 0), null, 1, 10, 7);
			//Processor.ProcessMatchAndGamer(dtStamp3);
			//Processor.GenerateMail(dtStamp3);

			//DateTime dtStamp4 = new DateTime(2021, 04, 21, 1, 5, 0);
			//UpdateMatch(1014, 7, 8, null, new DateTime(2021, 04, 19, 20, 5, 0), null, 1, 10, 9);
			//Processor.ProcessMatchAndGamer(dtStamp4);
			//Processor.GenerateMail(dtStamp4);
		}

		public static void UpdateMatch(int idEvent, int idRound, int idNumber, DateTime? dtStart, DateTime? dtEnd, DateTime? dtSchedule, int idWinner = 0, int score1 = 0, int score2 = 0, int idPlayer1 = 0, int idPlayer2 = 0)
		{
			Match match = Program.MatchList.Find(m => m.IdEvent == idEvent && m.IdRound == idRound && m.IdNumber == idNumber);
			if(match == null)
				throw new Exception(string.Format("Match idEvent {0} idRound {1} idNumber {2} not found", idEvent, idRound, idNumber));

			if(score1 != 0) match.Score1 = score1;
			if(score2 != 0) match.Score2 = score2;
			if(idWinner != 0) match.IdWinner = idWinner == 1 ? match.IdPlayer1 : match.IdPlayer2;
			if(idPlayer1 != 0) match.IdPlayer1 = idPlayer1;
			if(idPlayer2 != 0) match.IdPlayer2 = idPlayer2;
			if(dtStart.HasValue) match.DtStart = dtStart;
			if(dtEnd.HasValue) match.DtEnd = dtEnd;
			if(dtSchedule.HasValue) match.DtSchedule = dtSchedule;
		}
	}
}
