using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerQuizer.Model
{
	public class Quiz
	{
		public int IdEvent;
		public int IdRound;
		public int IdNumber;
		public int IdPlayer1;
		public int IdPlayer2;
		public int IdWinner;
		public int Score1;
		public int Score2;
		public bool? WinPlayer;
		public bool? WinScore;
		public DateTime? dtUpdate;
		public int GamePoint = 0;
		private string score;
		private string player1Name;
		private string player2Name;
		private string winnerName;

		public string Score
		{
			get { return string.Format("{0} : {1}", Score1, Score2); }
			set { score = value; }
		}

		public string Player1Name
		{
			get
			{
				SnookerPlayer player = Program.SnookerPlayerList.Find(pl => pl.IdPlayer == IdPlayer1);
				if(player != null)
					return player.DisplayChineseName(false);
				else
					return string.Empty;
			}
			set { player1Name = value; }
		}

		public string Player2Name
		{
			get
			{
				SnookerPlayer player = Program.SnookerPlayerList.Find(pl => pl.IdPlayer == IdPlayer2);
				if(player != null)
					return player.DisplayChineseName(false);
				else
					return string.Empty;
			}
			set { player2Name = value; }
		}

		public string WinnerName
		{
			get
			{
				SnookerPlayer player = Program.SnookerPlayerList.Find(pl => pl.IdPlayer == IdWinner);
				if(player != null)
					return player.DisplayChineseName(false);
				else
					return string.Empty;
			}
			set { winnerName = value; }
		}
	}

	public class GamePoint
	{
		public DateTime dtPoint;
		public int valPoint;
	}

	public class GamerInfo
	{
		public string UserName;
		public int IdEvent;
		public bool Bonus = false;
		public int TotalPoint = 0;
		public List<GamePoint> PointList = new List<GamePoint>();
		public List<Quiz> QuizList = new List<Quiz>();

		public GamerInfo(string userName, int idEvent, List<Quiz> lstQuiz, List<GamePoint> lstPoint)
		{
			UserName = userName;
			IdEvent = idEvent;
			QuizList = lstQuiz;
			PointList = lstPoint;
		}

		public GamerInfo()
		{

		}

		public string GetGamerSummary(DateTime dtStamp)
		{
			string result = string.Empty;
			
			int pointToday = QuizList.FindAll(q => q.dtUpdate == dtStamp).Sum(p => p.GamePoint);
			int winScoreCount = QuizList.FindAll(q => q.dtUpdate == dtStamp && q.WinScore.HasValue && q.WinScore == true).Count();
			int winPlayerCount = QuizList.FindAll(q => q.dtUpdate == dtStamp && q.WinPlayer.HasValue && q.WinPlayer == true).Count();

			if(pointToday == 0)
				result += string.Format("玩家 {0} 昨日一无所获，请继续努力!", UserName);
			else
			{
				result += string.Format("玩家 {0} 猜中{1}场比赛晋级名额 ", UserName, winPlayerCount);
				if(winScoreCount > 0)
					result += string.Format(", 并且还猜中了{0}场比分 (运气真好!) ", winScoreCount);

				result += string.Format("共获得{0}点积分!", pointToday);
			}

			return result;
		}

		public int GetWinnerScoreCount()
		{
			return QuizList.FindAll(q => q.WinScore.HasValue && q.WinScore == true).Count();
		}

		public int GetWinnerPlayerCount()
		{
			return QuizList.FindAll(q => q.WinPlayer.HasValue && q.WinPlayer == true).Count();
		}

		public string GetWinnerScoreList()
		{
			string result = "";
			foreach(Quiz q in QuizList.FindAll(q => q.WinScore.HasValue && q.WinScore == true))
			{
				result += string.Format("{0} vs {1} ({2}) ", q.Player1Name, q.Player2Name, q.Score);
			}
			return result.TrimEnd();
		}
	}
}
