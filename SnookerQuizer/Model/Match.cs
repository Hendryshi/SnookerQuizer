using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SnookerQuizer.Helper.Config;
using System.Globalization;

namespace SnookerQuizer.Model
{
	public enum PointRule
	{
		ExactScore = 50,
		SixteenFinal = 10,
		EightFinal = 20,
		QuarterFinal = 30,
		SemiFinal = 50,
		Final = 100
	}

	public enum Round
	{
		Round1 = 7,
		Round2 = 8,
		Round3 = 13,
		Round4 = 14,
		Round5 = 15
	}


	public class Match
	{
		[JsonProperty("ID")]
		public int IdMatch;
		[JsonProperty("EventID")]
		public int IdEvent;
		[JsonProperty("Round")]
		public int IdRound;
		[JsonProperty("Number")]
		public int IdNumber;
		public int WorldSnookerID;
		[JsonProperty("Player1ID")]
		public int IdPlayer1;
		public int Score1;
		[JsonProperty("Player2ID")]
		public int IdPlayer2;
		public int Score2;
		[JsonProperty("WinnerID")]
		public int IdWinner;
		[JsonProperty("Unfinished")]
		public bool UnFinished;
		public int Status; //0: unfinised 1: ongoing 2:finised
		[JsonProperty("StartDate")]
		public DateTime? DtStart;
		[JsonProperty("EndDate")]
		public DateTime? DtEnd;
		[JsonProperty("ScheduledDate")]
		public DateTime? DtSchedule;
		public string Sessions;
		private string score;
		private string player1Name;
		private string player2Name;
		private string winnerName;
		public string Note;

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
					return player.DisplayChineseName();
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
					return player.DisplayChineseName();
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
					return player.DisplayChineseName();
				else
					return string.Empty;
			}
			set { winnerName = value; }
		}

		public SnookerPlayer GetPlayer(int idPlayer)
		{
			return Program.SnookerPlayerList.Find(pl => pl.IdPlayer == idPlayer);
		}

		public string GetWorldSnookerResult()
		{
			return string.Format(SnookerAPI.GetWorldSnookerResult, Program.WorldSnookerId, WorldSnookerID);
		}

		public string GetHeadToHeadInfo()
		{
			return string.Format(SnookerAPI.GetHeadToHead, GetPlayer(IdPlayer1)?.DisplayHeadToHeadName(), GetPlayer(IdPlayer2)?.DisplayHeadToHeadName());
		}

		public List<DateTime> GetSessionDateList()
		{
			List<DateTime> lstSessionDate = new List<DateTime>();
			if(!string.IsNullOrEmpty(Sessions))
			{
				foreach(string session in Sessions.Split(';'))
				{
					if(!string.IsNullOrEmpty(session))
					{
						DateTime dtSession;
						if(DateTime.TryParseExact(session.Trim(), "dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtSession))
							lstSessionDate.Add(dtSession);
					}
				}
			}
			return lstSessionDate;
		}
	}

	[XmlRoot("SnookerPlayer")]
	public class SnookerPlayer
	{
		[JsonProperty("ID")]
		public int IdPlayer;
		public string FirstName;
		public string MiddleName;
		public string LastName;
		public string Nationality;
		public string ShortName;
		public string Photo;
		public string WorldSnookerPhoto;
		public int RankPosition;

		public string DisplayName()
		{
			return (FirstName + " " + LastName).Trim();
		}

		public string DisplayHeadToHeadName()
		{
			string name = (FirstName + "-" + LastName).Trim();

			if(Nationality == "China")
				name = (LastName + "-" + FirstName).Trim();

			name = name.Replace("Allister", "ali");

			return name.Replace("'", "").Replace("ü", "yu");
		}

		public string DisplayChineseName(bool showRank = true)
		{
			TranslatePlayer player = Program.TranslatePlayerList.Find(p => p.IdPlayer == IdPlayer);
			string name = DisplayName();
			if(player != null)
				name = string.IsNullOrEmpty(player.ChineseName) ? player.Name : player.ChineseName ;

			return name + (RankPosition != 0 && showRank ? " (" + RankPosition + ")" : "");
		}
	}

	public class RankingInfo
	{
		public int PlayerID;
		public int Position;
		public int Season;
		public int Sum;
	}

	public class TranslatePlayer
	{
		public int IdPlayer;
		public string Name;
		public string ChineseName = "";
	}

}
