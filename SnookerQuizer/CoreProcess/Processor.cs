using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SnookerQuizer.Helper;
using SnookerQuizer.Model;
using System.IO;

namespace SnookerQuizer.CoreProcess
{
	class Processor
	{
		public static List<SnookerPlayer> ImportPlayersInEvent()
		{
			List<SnookerPlayer> players = new List<SnookerPlayer>();
			if(!XMLHelper.XMLExist(Program.PlayersInEventXml))
			{
				players = HttpHelper.GetPlayersInEvent();
				XMLHelper.SaveListToXML(players, Program.PlayersInEventXml);
			}
			else
				players = XMLHelper.LoadXMLToList<SnookerPlayer>(Program.PlayersInEventXml);

			return players;
		}

		public static List<Match> ImportMatchsInEvent()
		{
			List<Match> matchs = HttpHelper.GetMatchsInEvent();
			if(!XMLHelper.XMLExist(Program.PlayersInEventXml))
				XMLHelper.SaveListToXML(matchs, Program.MatchsInEventXml);
			
			return matchs;
		}

		public static List<GamerInfo> ImportGamerList()
		{
			List<GamerInfo> gamerInfos = new List<GamerInfo>();
			foreach(string xmlName in XMLHelper.GetXMLList(Program.GamerFolder))
				gamerInfos.Add(XMLHelper.LoadXML<GamerInfo>(xmlName));
			
			return gamerInfos;
		}

		public static void CalculatePoint()
		{
			//Load the match during the last day
			//load the match of each user and compare
			//Update the game point and dtUpdate
			//Insert a new element of dictionary


		}

		public static void SaveDatasToLocal()
		{
			XMLHelper.SaveListToXML(Program.MatchList, Program.MatchsInEventXml);
		}
	}
}
