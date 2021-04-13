using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerQuizer.Model
{
	public class Quiz
	{
		public int IdMatch;
		public int IdWinner;
		public int Score1;
		public int Score2;
		public DateTime? dtUpdate;
		public int GamePoint;
		private string score;
		private string winnerName;

		public string Score
		{
			get { return string.Format("{0} - {1}", Score1, Score2); }
			set { score = value; }
		}

		public string WinnerName
		{
			get
			{
				SnookerPlayer player = Program.SnookerPlayerList.Find(pl => pl.IdPlayer == IdWinner);
				if(player != null)
					return player.DisplayName();
				else
					return string.Empty;
			}
			set { winnerName = value; }
		}
	}

	public class GamerInfo
	{
		public string UserName;
		public int IdEvent;
		//Dictionary of Date&Point
		public List<Quiz> QuizList;

		public GamerInfo(string userName, int idEvent, List<Quiz> lstQuiz)
		{
			UserName = userName;
			IdEvent = idEvent;
			QuizList = lstQuiz;
		}
	}
}
