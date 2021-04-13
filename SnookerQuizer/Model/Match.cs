using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SnookerQuizer.Helper;

namespace SnookerQuizer.Model
{
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

		public string Score
		{
			get { return string.Format("{0} - {1}", Score1, Score2); }
			set { score = value; }
		}

		public string Player1Name
		{
			get
			{
				SnookerPlayer player = Program.SnookerPlayerList.Find(pl => pl.IdPlayer == IdPlayer1);
				if(player != null)
					return player.DisplayName();
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
					return player.DisplayName();
				else
					return string.Empty;
			}
			set { player1Name = value; }
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

	[XmlRoot("SnookerPlayer")]
	public class SnookerPlayer
	{
		[JsonProperty("ID")]
		public int IdPlayer;
		public string FirstName;
		public string MiddleName;
		public string LastName;
		public string ShortName;
		public string Photo;

		public string DisplayName()
		{
			return (FirstName + " " + LastName).Trim();
		}
	}

}
