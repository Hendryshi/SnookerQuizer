using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnookerQuizer.Model
{
	public class Match
	{
		public int IdMatch;
		public int IdEvent;
		public int IdRound;
		public int IdNum;
		
		[JsonProperty("Player1ID")]
		public int IdPlayer1;
		public int Score1;
		public int IdPlayer2;
		public int Score2;
		public int IdWinner;
		public int Status; //0: unfinised 1: ongoing 2:finised
		public DateTime? DtStart;
		public DateTime? DtEnd;
	}

	public class SnookerPlayer
	{
		[JsonProperty("ID")]
		public int IdPlayer;
		public string FirstName;
		public string MiddleName;
		public string LastName;
		public string ShortName;
		public string Photo;

		public string NameDisplay()
		{
			return FirstName == "TBD" ? FirstName : ShortName;
		}
	}


	public class DataObject
	{
		public string StartDate { get; set; }
		public string test { get; set; }
	}
}
