using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using SnookerQuizer.Model;
using Serilog;

namespace SnookerQuizer.Helper
{
	public class HttpHelper
	{
		public static List<T> GetData<T>(string url, string urlParam = "")
		{
			List<T> result = null;
			HttpClient client = new HttpClient();
			try
			{
				Log.Information(string.Format("Start getting data from API: {0}", url));
				client.BaseAddress = new Uri(url);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				// List data response.
				HttpResponseMessage response = client.GetAsync(urlParam).Result;

				if(response.IsSuccessStatusCode)
				{
					result = response.Content.ReadAsAsync<IEnumerable<T>>().Result.ToList();
					Log.Information(string.Format("Successfully getting data from the API. Count returned: {0}", result.Count()));
				}
				else
					Log.Error("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);

				return result;
			}
			catch(Exception ex)
			{
				Log.Fatal(ex, string.Format("Error when getting data from url: [{0}], urlParam: [{1}]", url, urlParam));
				return result;
			}
			finally
			{
				client.Dispose();
			}
		}

		public static List<SnookerPlayer> GetPlayersInEvent(int idEvent = 0)
		{
			if(idEvent == 0)
				idEvent = Program.IdEvent;

			Log.Information(string.Format("API: Getting all the players in the event {0}", idEvent));
			List<SnookerPlayer> players = HttpHelper.GetData<SnookerPlayer>(string.Format(Config.SnookerAPI.GetPlayersInEvent, idEvent));

			if(players != null && players.Count > 0)
				return players.OrderBy(c => c.IdPlayer).ToList();
			else
				throw new Exception("No Snooker player has been retrived from the API. Please Check !");
		}

		public static List<Match> GetMatchsInEvent(int idEvent = 0)
		{
			if(idEvent == 0)
				idEvent = Program.IdEvent;

			Log.Information(string.Format("API: Getting all the matchs in the event {0}", idEvent));
			List<Match> matchs = HttpHelper.GetData<Match>(string.Format(Config.SnookerAPI.GetMatchsInEvent, idEvent));

			if(matchs != null && matchs.Count > 0)
				return matchs.OrderBy(c => c.IdRound).ThenBy(c => c.IdNumber).ToList();
			else
				throw new Exception("No match has been retrived from the API. Please Check !");
		}

		public static Match GetMatch(int idRound, int idNumber, int idEvent = 0)
		{
			if(idEvent == 0)
				idEvent = Program.IdEvent;

			Log.Information(string.Format("API: Getting the specific match: Event [{0}] Round [{1}] Number [{2}]", idEvent, idRound, idNumber));
			List<Match> matchs = HttpHelper.GetData<Match>(string.Format(Config.SnookerAPI.GetMatch, idEvent, idRound, idNumber));

			if(matchs != null && matchs.Count > 0)
				return matchs.First();
			else
				throw new Exception("No match has been retrived from the API. Please Check !");
		}

		public static SnookerPlayer GetPlayer(int idPlayer)
		{
			Log.Information(string.Format("API: Getting the specific player: {0}", idPlayer));
			List<SnookerPlayer> players = HttpHelper.GetData<SnookerPlayer>(string.Format(Config.SnookerAPI.GetPlayer, idPlayer));
			
			if(players != null && players.Count > 0)
				return players.First();
			else
				throw new Exception("No Snooker Player has been retrived from the API. Please Check !");
		}

		public static List<RankingInfo> GetPlayerRank(int idSeason = 2020)
		{
			Log.Information(string.Format("API: Getting all the player ranking info for season {0}", idSeason));
			List<RankingInfo> players = HttpHelper.GetData<RankingInfo>(string.Format(Config.SnookerAPI.GetPlayersRank, idSeason));

			if(players != null && players.Count > 0)
				return players.OrderBy(c => c.PlayerID).ToList();
			else
				throw new Exception("No Ranking info has been retrived from the API. Please Check !");
		}

	}
}
