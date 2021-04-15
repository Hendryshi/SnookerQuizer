using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnookerQuizer.Helper;
using SnookerQuizer.Helper.Config;
using SnookerQuizer.Model;
using System.IO;
using Serilog;
using System.Configuration;

namespace SnookerQuizer.CoreProcess
{
	class Processor
	{
		public static List<SnookerPlayer> ImportPlayersInEvent()
		{
			Log.Information("");
			Log.Information("------------- Loading Snooker Players -------------");
			
			List<SnookerPlayer> players = new List<SnookerPlayer>();
			if(!XMLHelper.XMLExist(Program.PlayersInEventXml))
			{
				players = HttpHelper.GetPlayersInEvent();
				List<RankingInfo> playerRanks = HttpHelper.GetPlayerRank();

				foreach(SnookerPlayer pl in players)
				{
					RankingInfo r = playerRanks.Find(p => p.PlayerID == pl.IdPlayer);
					if(r != null)
						pl.RankPosition = r.Position;
				}
				XMLHelper.SaveListToXML(players, Program.PlayersInEventXml);
			}
			else 
				players = XMLHelper.LoadXMLToList<SnookerPlayer>(Program.PlayersInEventXml);

			return players;
		}

		public static List<TranslatePlayer> ImportTranslatePlayers()
		{
			Log.Information("");
			Log.Information("------------- Loading Translate Players -------------");

			List<TranslatePlayer> players = new List<TranslatePlayer>();
			if(!XMLHelper.XMLExist(Program.TranslatePlayersXml))
			{
				foreach(SnookerPlayer pl in Program.SnookerPlayerList)
				{
					players.Add(new TranslatePlayer() { IdPlayer = pl.IdPlayer, Name = pl.DisplayName() });
				}
				XMLHelper.SaveListToXML(players, Program.TranslatePlayersXml);
			}
			else
				players = XMLHelper.LoadXMLToList<TranslatePlayer>(Program.TranslatePlayersXml);
			
			return players;	
		}

		public static void InitUserList()
		{
			Log.Information("");
			Log.Information("------------- Initializing Game Users -------------");
			foreach(string userName in Game.UserList.Split(';'))
			{
				string xmlName = string.Format("Gamer{0}-{1}.xml", Program.IdEvent, userName);
				if(!XMLHelper.XMLExist(xmlName))
				{
					Log.Information(string.Format("Creating new user xml {0}", xmlName));
					List<Quiz> lstQuiz = new List<Quiz>() { new Quiz() };
					GamerInfo gi = new GamerInfo() { IdEvent = Program.IdEvent, Bonus = false, UserName = userName, QuizList = lstQuiz };
					XMLHelper.SaveToXML(gi, xmlName);
				}
			}
		}

		public static List<Match> ImportMatchsInEvent(bool saveToLocal = true)
		{
			Log.Information("");
			Log.Information("------------- Loading Matchs ----------------------");
			
			List<Match> matchs = HttpHelper.GetMatchsInEvent();

			if(XMLHelper.XMLExist(Program.MatchsInEventXml))
			{
				Log.Information("BackUp the Match Xml");
				string backUpFolder = ConfigurationManager.AppSettings["BackUpFolder"];
				XMLHelper.XMLCopy(Program.MatchsInEventXml, Path.Combine(backUpFolder, Program.MatchsInEventXml));
			}

			if(saveToLocal)
				XMLHelper.SaveListToXML(matchs, Program.MatchsInEventXml);

			//foreach(Match m in matchs)
			//	Console.WriteLine(m.GetHeadToHeadInfo());

			//foreach(Match m in matchs)
			//	Console.WriteLine(m.DtSchedule?.ToString("hh:mm"));

			return matchs;
		}

		public static List<GamerInfo> ImportGamerList()
		{
			Log.Information("");
			Log.Information("------------- Loading Gamer List ------------------");
			
			List<GamerInfo> gamerInfos = new List<GamerInfo>();
			foreach(string xmlName in XMLHelper.GetXMLList("Gamer*.xml"))
			{
				gamerInfos.Add(XMLHelper.LoadXML<GamerInfo>(xmlName));
			}
			
			return gamerInfos;
		}

		public static void SaveDatasToLocal()
		{
			Log.Information("");
			Log.Information("------------- Saving XML List ---------------------");

			//XMLHelper.SaveListToXML(Program.MatchList, Program.MatchsInEventXml);

			foreach(GamerInfo gi in Program.GamerList)
				XMLHelper.SaveToXML(gi, string.Format(Program.GamerXml, gi.IdEvent, gi.UserName));
		}

		public static void ProcessMatchAndGamer(DateTime dtStamp)
		{
			Log.Information("");
			Log.Information("------------- Processing Match ---------------------");

			List<Match> lstMatch = Program.MatchList.FindAll(m => m.DtEnd != null && m.DtEnd > dtStamp.AddDays(-1) && m.DtEnd <= dtStamp);

			if(lstMatch.Count() > 0)
			{
				Log.Information(string.Format("Found {0} match ended during the last day", lstMatch.Count()));
				
				foreach(Match m in lstMatch)
				{
					Log.Information(string.Format("Process the match [Round {0} Number {1}] {2} vs {3}", m.IdRound, m.IdNumber, m.Player1Name, m.Player2Name));
					foreach(GamerInfo gi in Program.GamerList)
					{
						Quiz mQuiz = gi.QuizList.Find(q => q.IdEvent == m.IdEvent && q.IdNumber == m.IdNumber);

						if(mQuiz == null)
							throw new Exception(string.Format("Cannot find the match [Round {0} Number {1}] in GameUser {2}, Please Check !", m.IdRound, m.IdNumber, gi.UserName));

						if(mQuiz.dtUpdate != null)
						{
							Log.Warning(string.Format("This match has already been treated in GameUser {0}, skip it.", gi.UserName));
							continue;
						}

						mQuiz.GamePoint = CalculatePoint(m, mQuiz, gi.Bonus);
						mQuiz.dtUpdate = dtStamp;
						gi.TotalPoint += mQuiz.GamePoint;
					}
				}

				foreach(GamerInfo gi in Program.GamerList)
				{
					int pointToday = gi.QuizList.FindAll(q => q.dtUpdate == dtStamp).Sum(p => p.GamePoint);
					Log.Information(string.Format("User {0} gained {1} points today", gi.UserName, pointToday));
					gi.PointList.Add(new GamePoint() { dtPoint = dtStamp, valPoint = gi.TotalPoint });
				}
			}
		}

		private static int CalculatePoint(Match mMatch, Quiz mQuiz, bool Bonus = false)
		{
			int point = 0;

			if(mMatch.IdPlayer1 == mQuiz.IdPlayer1 && mMatch.IdPlayer2 == mQuiz.IdPlayer2 && mMatch.Score == mQuiz.Score)
				point += (int)PointRule.ExactScore;

			if(mMatch.IdWinner == mQuiz.IdWinner)
			{
				bool isDouble = Math.Abs(mMatch.Score1 - mMatch.Score2) == 1;

				switch(mMatch.IdRound)
				{
					case (int)Round.Round1:
						point += (int)PointRule.SixteenFinal * (isDouble ? 2 : 1);
						break;
					case (int)Round.Round2:
						point += (int)PointRule.EightFinal * (isDouble ? 2 : 1);
						break;
					case (int)Round.Round3:
						point += (int)PointRule.QuarterFinal * (isDouble ? 2 : 1);
						break;
					case (int)Round.Round4:
						point += (int)PointRule.SemiFinal * (isDouble ? 2 : 1);
						break;
					case (int)Round.Round5:
						point += (int)PointRule.Final * (isDouble ? 2 : 1);
						break;
					default:
						point += (int)PointRule.SixteenFinal * (isDouble ? 2 : 1);
						break;
				}
			}
			return Bonus ? point * 2 : point;
		}

		public static void GenerateMail(DateTime dtStamp)
		{
			Log.Information("");
			Log.Information("------------- Generating Email Message ---------------------");
			string emailResultBody = File.ReadAllText(ConfigurationManager.AppSettings["EmailTemplate"]);

			//Generate LastDay Match
			string strLastMatch = string.Empty;
			List<Match> lstLastDayMatch = Program.MatchList.FindAll(m => m.DtEnd != null && m.DtEnd > dtStamp.AddDays(-1) && m.DtEnd <= dtStamp);

			if(lstLastDayMatch.Count() > 0)
			{
				foreach(Match m in lstLastDayMatch)
				{
					strLastMatch += string.Format("<tr style=height: 160px;'>");
					strLastMatch += string.Format("<td style='width: 25%; height: 160px; text-align: center; border: 1px solid black;'>");
					strLastMatch += string.Format("<p>{0}</p>", m.Player1Name);
					strLastMatch += string.Format("<img alt='{0}' src='{1}' data-file-width='2195' data-file-height='2359' style='border-top-left-radius: 50%; border-top-right-radius: 50%; border-bottom-right-radius: 50%; border-bottom-left-radius: 50%; width: 81px; height: 87px; display: block; margin-left: auto; margin-right: auto;' /></td>", m.Player1Name, m.GetPlayer(m.IdPlayer1)?.Photo);
					strLastMatch += string.Format("<td style='width: 25%; text-align: center; height: 160px; border: 1px solid black;'>");
					strLastMatch += string.Format("<h3>{0}</h3>", m.Score);
					strLastMatch += string.Format("<a href='https://www.w3schools.com' style='text-decoration: underline;'>比分详情</a></td>");
					strLastMatch += string.Format("<td style='width: 25%; height:160px; text-align: center;'>");
					strLastMatch += string.Format("<p>{0}</p>", m.Player2Name);
					strLastMatch += string.Format("<img alt='{0}' src='{1}' data-file-width='2195' data-file-height='2359' style='border-top-left-radius: 50%; border-top-right-radius: 50%; border-bottom-right-radius: 50%; border-bottom-left-radius: 50%; width: 81px; height: 87px; display: block; margin-left: auto; margin-right: auto;' /></td>", m.Player2Name, m.GetPlayer(m.IdPlayer2)?.Photo);
					strLastMatch += string.Format("</tr>");
					strLastMatch += string.Format("<tr>");
					strLastMatch += string.Format("<td colspan='3' style = 'text-align: center; width:75%;'><strong>YeJia</strong>5:10 &nbsp; <strong>TianLe</strong>10:5</td>");
					strLastMatch += string.Format("</tr>");
				}
			}

			emailResultBody = emailResultBody.Replace("[LastDayMatch]", strLastMatch);

			//Generate Gamer point list
			string strGamerInfo = string.Empty;

			// Geneate table


			emailResultBody = emailResultBody.Replace("[GamerInfo]", strGamerInfo);


			//Generate Today Match
			string strTodayMatch = string.Empty;
			List<Match> lstTodayMatch = Program.MatchList.FindAll(m => m.DtSchedule != null && m.DtSchedule.Value.Date == dtStamp.Date);
			if(lstTodayMatch.Count() > 0)
			{
				foreach(Match m in lstTodayMatch)
				{
					//Generate Today Match
				}
			}

			emailResultBody = emailResultBody.Replace("[TodayMatch]", strTodayMatch);

			string htmlName = string.Format("GameResult-{0}.html", dtStamp.ToString("yyyymmdd"));
			Log.Information(string.Format("Save genetared html file to local: {0}", htmlName));
			string emailPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], ConfigurationManager.AppSettings["EmailTemplate"]);
			File.WriteAllText(Path.Combine(emailPath, htmlName), emailResultBody);

			if(Game.SendEmail)
			{
				Log.Information("");
				Log.Information("------------- Sending Email ---------------------");
				Log.Information(string.Format("Email send to {0}", Game.UserEmail));
				MailHelper.SendMail(Game.UserEmail, string.Format("Snooker Result {0}", dtStamp.ToString("mm/dd")), emailResultBody);
			}
		}

		private static string GetPotentialPrediction(Match mMatch)
		{
			string result = string.Empty;

			foreach(GamerInfo gi in Program.GamerList)
			{
				Quiz mQuiz = gi.QuizList.Find(q => q.IdEvent == mMatch.IdEvent && q.IdNumber == mMatch.IdNumber);
				if(mQuiz != null)
				{
					if(mQuiz.IdWinner == mMatch.IdPlayer1 || mQuiz.IdWinner == mMatch.IdPlayer2)
					{
						result += string.Format("{0}: {1} 胜", gi.UserName, mQuiz.WinnerName);

						if(mQuiz.IdPlayer1 == mMatch.IdPlayer1 && mQuiz.IdPlayer2 == mMatch.IdPlayer2)
							result += string.Format(" ({0})   ", mQuiz.Score);
					}
				}
			}
			return result.TrimEnd();
		}
	}
}
