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
			Log.Information("------------- Loading Translate Players -----------");

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

		public static List<Match> ImportMatchsInEvent(bool updateFromApi = true, bool saveToLocal = true)
		{
			Log.Information("");
			Log.Information("------------- Loading Matchs ----------------------");

			if(XMLHelper.XMLExist(Program.MatchsInEventXml))
			{
				Log.Information("BackUp the Match Xml");
				string backUpFolder = ConfigurationManager.AppSettings["BackUpFolder"];
				XMLHelper.XMLCopy(Program.MatchsInEventXml, Path.Combine(backUpFolder, Program.MatchsInEventXml));
			}

			List<Match> matchs = new List<Match>();

			if(updateFromApi)
			{
				matchs = HttpHelper.GetMatchsInEvent();

				if(saveToLocal)
					XMLHelper.SaveListToXML(matchs, Program.MatchsInEventXml);
			}
			else
				matchs = XMLHelper.LoadXMLToList<Match>(Program.MatchsInEventXml);

			return matchs;
		}

		public static List<GamerInfo> ImportGamerList()
		{
			Log.Information("");
			Log.Information("------------- Loading Gamer List ------------------");

			List<GamerInfo> gamerInfos = new List<GamerInfo>();
			foreach(string xmlName in XMLHelper.GetXMLList("Gamer*.xml"))
			{
				if(XMLHelper.XMLExist(xmlName))
				{
					Log.Information("BackUp the Gamer Xml");
					string backUpFolder = ConfigurationManager.AppSettings["BackUpFolder"];
					XMLHelper.XMLCopy(Program.MatchsInEventXml, Path.Combine(backUpFolder, xmlName));
				}
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
					Log.Information(string.Format("Process the match [Round {0} Number {1}] {2} vs {3} Result: {4}", m.IdRound, m.IdNumber, m.Player1Name, m.Player2Name, m.Score));
					foreach(GamerInfo gi in Program.GamerList)
					{
						Quiz mQuiz = gi.QuizList.Find(q => q.IdEvent == m.IdEvent && q.IdRound == m.IdRound && q.IdNumber == m.IdNumber);

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
			{
				point += (int)PointRule.ExactScore;
				mQuiz.WinScore = true;
			}
			else
				mQuiz.WinScore = false;

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
				mQuiz.WinPlayer = true;
			}
			else
				mQuiz.WinPlayer = false;

			return Bonus ? point * 2 : point;
		}

		public static void GenerateMail(DateTime dtStamp)
		{
			Log.Information("");
			Log.Information("------------- Generating Email Message -------------");

			string emailResultBody = File.ReadAllText(ConfigurationManager.AppSettings["EmailTemplate"]);
			
			if(Game.UseFinalEmailTemplate)
				emailResultBody = File.ReadAllText(ConfigurationManager.AppSettings["FinalEmailTemplate"]);

			int matchCount = 0;

			//Generate LastDay Match
			var strLastMatch = new StringBuilder();
			List<Match> lstLastDayMatch = Program.MatchList.FindAll(m => m.DtEnd != null && m.DtEnd > dtStamp.AddDays(-1) && m.DtEnd <= dtStamp);

			if(lstLastDayMatch.Count() > 0)
			{
				matchCount = lstLastDayMatch.Count();
				foreach(Match m in lstLastDayMatch)
				{
					strLastMatch.AppendFormat("<tr>");
					strLastMatch.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
					strLastMatch.AppendFormat("<td style='width:18px;min-width:18px'></td>");
					strLastMatch.AppendFormat("<td style='width:100%;font-size: 13px; font-family: Arial,Helvetica,sans-serif;line-height:18px;padding-bottom:14px;' colspan='3'>");
					strLastMatch.AppendFormat("<table width='100%' border-collapse='collapse' cellpadding='5' cellspacing='0' style='text-align: center;border:1px solid #AAAAAA;'>");
					strLastMatch.AppendFormat("<tbody>");
					strLastMatch.AppendFormat("<tr style='border:1px solid #AAAAAA'>");
					strLastMatch.AppendFormat("<td><p style='text-overflow:clip;overflow:hidden;white-space:nowrap'>{0}</p><img alt='{1}' src='{2}' /></td>", m.Player1Name, m.IdPlayer1, m.GetPlayer(m.IdPlayer1)?.WorldSnookerPhoto);
					strLastMatch.AppendFormat("<td><h3>{0}</h3><a href='{1}' style='text-decoration: underline'>比分详情</a></td>", m.Score, m.GetWorldSnookerResult());
					strLastMatch.AppendFormat("<td><p style='text-overflow:clip;overflow:hidden;white-space:nowrap'>{0}</p><img alt='{1}' src='{2}' /></td>", m.Player2Name, m.IdPlayer2, m.GetPlayer(m.IdPlayer2)?.WorldSnookerPhoto);
					strLastMatch.AppendFormat("</tr>");
					strLastMatch.AppendFormat("</tbody>");
					strLastMatch.AppendFormat("</table>");
					strLastMatch.AppendFormat("</td>");
					strLastMatch.AppendFormat("<td style='width:18px;min-width:18px'></td>");
					strLastMatch.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
					strLastMatch.AppendFormat("</tr>");
				}
			}

			emailResultBody = emailResultBody.Replace("[LastDayMatch]", strLastMatch.ToString());

			if(!Game.UseFinalEmailTemplate)
			{
				//Generate Gamer point list
				var strGamerSummary = new StringBuilder();

				if(matchCount > 0)
				{
					strGamerSummary.AppendFormat("<tr>");
					strGamerSummary.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
					strGamerSummary.AppendFormat("<td style='width:18px;min-width:18px'></td>");
					strGamerSummary.AppendFormat("<td style='width:100%;font-size: 13px; font-family: Arial,Helvetica,sans-serif;line-height:18px;padding-bottom:14px;' colspan='3'>");
					strGamerSummary.AppendFormat("昨日共进行了{0}场比赛:", matchCount);
					strGamerSummary.AppendFormat("<ul style='list-style-type: circle;'>");

					foreach(GamerInfo gi in Program.GamerList)
					{
						strGamerSummary.AppendFormat("<li>{0}</li>", gi.GetGamerSummary(dtStamp));
					}

					strGamerSummary.AppendFormat("</td>");
					strGamerSummary.AppendFormat("<td style='width:18px;min-width:18px'></td>");
					strGamerSummary.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
					strGamerSummary.AppendFormat("</tr>");
				}

				emailResultBody = emailResultBody.Replace("[GamerSummary]", strGamerSummary.ToString());

				var strGamerTable = new StringBuilder();

				if(matchCount > 0)
				{
					bool firstUser = true;
					foreach(GamerInfo gi in Program.GamerList.OrderByDescending(g => g.TotalPoint))
					{
						if(firstUser)
						{
							strGamerTable.AppendFormat("<thead>");
							strGamerTable.AppendFormat("<tr>");
							strGamerTable.AppendFormat("<th style='font-weight: bold;'>玩家</th>");
							foreach(GamePoint gp in gi.PointList.OrderByDescending(i => i.dtPoint).Take(3))
							{
								strGamerTable.AppendFormat("<th style='font-weight: bold;'>{0}</th>", gp.dtPoint.ToString("MM/dd"));
							}
							strGamerTable.AppendFormat("<th style='font-weight: bold;'>晋级(比分)</th>");
							strGamerTable.AppendFormat("<th style='font-weight: bold;'>奖金</th>");
							strGamerTable.AppendFormat("</tr>");
							strGamerTable.AppendFormat("</thead>");
							strGamerTable.AppendFormat("<tbody>");
							strGamerTable.AppendFormat("<tr style='color: #30adf4;'>");
							firstUser = false;
						}
						else
						{
							strGamerTable.AppendFormat("<tr>");
						}

						strGamerTable.AppendFormat("<td><div class='product-name-line'>{0}</div></td>", gi.UserName);

						foreach(GamePoint gp in gi.PointList.OrderByDescending(i => i.dtPoint).Take(3))
						{
							strGamerTable.AppendFormat("<td>{0}</td>", gp.valPoint);
						}

						strGamerTable.AppendFormat("<td>{0}({1})</td>", gi.GetWinnerPlayerCount(), gi.GetWinnerScoreCount());
						strGamerTable.AppendFormat("<td>{0}</td>", GetGamerDiffScore(gi));
						strGamerTable.AppendFormat("</tr>");
					}

					if(!firstUser)
						strGamerTable.AppendFormat("</tbody>");
				}

				emailResultBody = emailResultBody.Replace("[GamerTable]", strGamerTable.ToString());

				//Generate Today Match
				var strTodayMatch = new StringBuilder();
				List<Match> lstTodayMatch = Program.MatchList.FindAll(m => m.DtSchedule != null && m.DtSchedule.Value.Date == dtStamp.Date).OrderBy(d => d.DtSchedule).ToList();
				if(lstTodayMatch.Count() > 0)
				{
					foreach(Match m in lstTodayMatch)
					{
						strTodayMatch.AppendFormat("<tr>");
						strTodayMatch.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
						strTodayMatch.AppendFormat("<td style='width:18px;min-width:18px'></td>");
						strTodayMatch.AppendFormat("<td style='width:100%;font-size: 13px; font-family: Arial,Helvetica,sans-serif;line-height:18px;padding-bottom:14px;' colspan='3'>");
						strTodayMatch.AppendFormat("<table width='100%' border-collapse='collapse' cellpadding='5' cellspacing='0' style='text-align: center;border:1px solid #AAAAAA;'>");
						strTodayMatch.AppendFormat("<tbody>");
						strTodayMatch.AppendFormat("<tr style='border:1px solid #AAAAAA'>");
						strTodayMatch.AppendFormat("<td><p style='text-overflow:clip;overflow:hidden;white-space:nowrap'>{0}</p><img alt='{1}' src='{2}' /></td>", m.Player1Name, m.IdPlayer1, m.GetPlayer(m.IdPlayer1)?.WorldSnookerPhoto);
						strTodayMatch.AppendFormat("<td>");
						string sessionDate = string.Join("<br />", m.GetSessionDateList().Where(s => s.Date == dtStamp.Date).Select(l => l.AddHours(6).ToString("HH:mm")));
						strTodayMatch.AppendFormat("<h3>{0}{1}</h3>", m.DtSchedule?.AddHours(8).ToString("HH:mm"), string.IsNullOrEmpty(sessionDate) ? "" : "<br />" + sessionDate);
						strTodayMatch.AppendFormat("<h3>{0}</h3>", m.Score);
						strTodayMatch.AppendFormat("<a href='{0}' style='text-decoration: underline; '>历史战绩</a></td>", m.GetHeadToHeadInfo());
						strTodayMatch.AppendFormat("<td><p style='text-overflow:clip;overflow:hidden;white-space:nowrap'>{0}</p><img alt='{1}' src='{2}' /></td>", m.Player2Name, m.IdPlayer2, m.GetPlayer(m.IdPlayer2)?.WorldSnookerPhoto);
						strTodayMatch.AppendFormat("</tr>");
						strTodayMatch.AppendFormat("</tbody>");
						strTodayMatch.AppendFormat("</table>");
						strTodayMatch.AppendFormat("</td>");
						strTodayMatch.AppendFormat("<td style='width:18px;min-width:18px'></td>");
						strTodayMatch.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
						strTodayMatch.AppendFormat("</tr>");

						strTodayMatch.AppendFormat("<tr>");
						strTodayMatch.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
						strTodayMatch.AppendFormat("<td style='width:18px;min-width:18px'></td>");
						strTodayMatch.AppendFormat("<td style='width:100%;font-size: 13px; font-family: Arial,Helvetica,sans-serif;line-height:18px;padding-bottom:14px;' colspan='3'>");

						strTodayMatch.AppendFormat("<table class='price-alerts-table' width='100%' border='0' cellpadding='5' cellspacing='0'>");

						foreach(GamerInfo gi in Program.GamerList)
						{
							string prediction = GetPotentialPrediction(m, gi);
							if(!string.IsNullOrEmpty(prediction))
							{
								strTodayMatch.AppendFormat("<tr><td><div class='product-name-line'>{0}</div></td>", gi.UserName);
								strTodayMatch.AppendFormat("<td>{0}</td></tr>", prediction);
							}
						}

						strTodayMatch.AppendFormat("</table>");
						strTodayMatch.AppendFormat("</td>");
						strTodayMatch.AppendFormat("<td style='width:18px;min-width:18px'></td>");
						strTodayMatch.AppendFormat("<td style='font-size:1px;line-height:1px'></td>");
						strTodayMatch.AppendFormat("</tr>");
					}
				}

				emailResultBody = emailResultBody.Replace("[TodayMatch]", strTodayMatch.ToString());
			}
			else
			{
				//Generate final Gamer Point Result
				var strFinalGamerResult = new StringBuilder();

				int index = 1;
				foreach(GamerInfo gi in Program.GamerList.OrderByDescending(g => g.TotalPoint).ThenByDescending(c => c.GetWinnerPlayerCount()))
				{
					string prefix = string.Format("第{0}名", index);
					if(index == 1)
						prefix = "冠军";
					else if(index == 2)
						prefix = "亚军";
					else if(index == Program.GamerList.Count())
						prefix = "倒霉蛋";

					if(index == 1)
						strFinalGamerResult.AppendFormat("<b style='font-size: 20px; color: red'> 恭喜玩家 {0} 获得本次比赛冠军</b>", gi.UserName);
					else if(index == Program.GamerList.Count())
						strFinalGamerResult.AppendFormat("<b style='font-size: 20px; color: blue'> {0} {1}</b>", prefix, gi.UserName);
					else
						strFinalGamerResult.AppendFormat("<b style='font-size: 18px'> {0} {1}</b>", prefix, gi.UserName);

					strFinalGamerResult.AppendFormat("<ul style='list-style-type: circle;'>");
					strFinalGamerResult.AppendFormat("<li>玩家 {0} 获得积分: <b>{1}</b>  {2}: <b>{3}¥</b></li>", gi.UserName, gi.TotalPoint, index == 1 ? "获得奖金" : "需支付", Math.Abs(GetGamerDiffScore(gi)));
					strFinalGamerResult.AppendFormat("<li>玩家 {0} 在本次比赛中共猜对晋级名额 <b>{1}</b> 次</li>", gi.UserName, gi.GetWinnerPlayerCount());

					if(gi.GetWinnerScoreCount() > 0)
						strFinalGamerResult.AppendFormat("<li>玩家 {0} 在本次比赛中共猜对比分 <b>{1}</b> 次, 分别为: {2}</li>", gi.UserName, gi.GetWinnerScoreCount(), gi.GetWinnerScoreList());
					
					strFinalGamerResult.AppendFormat("</ul>");
					index++;
				}

				emailResultBody = emailResultBody.Replace("[FinalGamerResult]", strFinalGamerResult.ToString());
			}

			string htmlName = string.Format("GameResult-{0}.html", dtStamp.ToString("yyyyMMdd"));
			Log.Information(string.Format("Save genetared html file to local: {0}", htmlName));
			string emailPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], ConfigurationManager.AppSettings["EmailFolder"]);
			File.WriteAllText(Path.Combine(emailPath, htmlName), emailResultBody);

			if(Game.SendEmail)
			{
				Log.Information("");
				Log.Information("------------- Sending Email ---------------------");

				string recipientlst = Game.CheckMail ? Game.TestEmail : Game.UserEmail;

				foreach(string receipt in recipientlst.Split(';'))
				{
					Log.Information(string.Format("Email send to {0}", receipt));
					MailHelper.SendMail(receipt, string.Format("斯诺克竞猜 {0}", dtStamp.ToString("MM/dd")), emailResultBody);
				}
			}
		}

		private static string GetPotentialPrediction(Match mMatch, GamerInfo gi)
		{
			string result = string.Empty;

			Quiz mQuiz = gi.QuizList.Find(q => q.IdEvent == mMatch.IdEvent && q.IdRound == mMatch.IdRound && q.IdNumber == mMatch.IdNumber);
			if(mQuiz != null)
			{
				if(mQuiz.IdWinner == mMatch.IdPlayer1 || mQuiz.IdWinner == mMatch.IdPlayer2)
				{
					result += string.Format("预测 {0} 胜", mQuiz.WinnerName);

					if(mQuiz.IdPlayer1 == mMatch.IdPlayer1 && mQuiz.IdPlayer2 == mMatch.IdPlayer2)
						result += string.Format(" ({0})", mQuiz.Score);

				}
			}

			return result;
		}

		private static int GetGamerDiffScore(GamerInfo gi)
		{
			int result = 0;
			int pointMax = Program.GamerList.Max(g => g.TotalPoint);
			if(gi.TotalPoint == pointMax)
			{
				foreach(GamerInfo g in Program.GamerList)
					result += pointMax - g.TotalPoint;
			}
			else
				result = gi.TotalPoint - pointMax;

			return result;
		}
	}
}
