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
	}
}
