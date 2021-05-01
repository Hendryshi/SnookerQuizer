using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SnookerQuizer.Helper.Config
{
	internal class Common
	{
		public static string ReadSettingString(string keyName, string defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(string.IsNullOrEmpty(defaultValue))
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue;
			}
			return keyValue;
		}

		public static int ReadSettingInt(string keyName, int? defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(!defaultValue.HasValue)
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue.Value;
			}
			else
			{
				int resultValue;

				if(int.TryParse(keyValue, out resultValue))
					return resultValue;
				else
					throw new ArgumentException(string.Format("Key [{0}] has a bad format value: {1}", keyName, keyValue));
			}
		}

		public static short ReadSettingShort(string keyName, short? defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(!defaultValue.HasValue)
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue.Value;
			}
			else
			{
				short resultValue;

				if(short.TryParse(keyValue, out resultValue))
					return resultValue;
				else
					throw new ArgumentException(string.Format("Key [{0}] has a bad format value: {1}", keyName, keyValue));
			}
		}

		public static bool ReadSettingBool(string keyName, bool? defaultValue = null)
		{
			string keyValue = ConfigurationManager.AppSettings[keyName];
			if(string.IsNullOrEmpty(keyValue))
			{
				if(!defaultValue.HasValue)
					throw new ArgumentException(string.Format("Key [{0}] not found", keyName));
				else
					return defaultValue.Value;
			}
			else
			{
				bool resultValue;

				if(bool.TryParse(keyValue, out resultValue))
					return resultValue;
				else
					throw new ArgumentException(string.Format("Key [{0}] has a bad format value: {1}", keyName, keyValue));
			}
		}
	}

	public class Game
	{
		public static string UserList
		{
			get
			{
				return Common.ReadSettingString("Game.UserList", "");
			}
		}

		public static string UserEmail
		{
			get
			{
				return Common.ReadSettingString("Game.UserEmail", "");
			}
		}
		public static string TestEmail
		{
			get
			{
				return Common.ReadSettingString("Game.TestEmail", "");
			}
		}

		public static bool SendEmail
		{
			get
			{
				return Common.ReadSettingBool("Game.SendEmail", false);
			}
		}

		public static bool IsGameInit
		{
			get
			{
				return Common.ReadSettingBool("Game.Init", false);
			}
		}

		public static bool IsGameProcess
		{
			get
			{
				return Common.ReadSettingBool("Game.Process", false);
			}
		}

		public static bool IsTest
		{
			get
			{
				return Common.ReadSettingBool("Game.Test", false);
			}
		}

		public static bool CheckMail
		{
			get
			{
				return Common.ReadSettingBool("Game.CheckMail", false);
			}
		}

		public static bool UseFinalEmailTemplate
		{
			get
			{
				return Common.ReadSettingBool("Game.UseFinalEmailTemplate", false);
			}
		}
	}
	
	public class SnookerAPI
	{
		public static string GetEvent
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.EventUrl", "");
			}
		}

		public static string GetPlayer
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.PlayerUrl", "");
			}
		}

		public static string GetMatch
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.MatchUrl", "");
			}
		}

		public static string GetEventsInSeason
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.EventsInSeasonUrl", "");
			}
		}

		public static string GetMatchsInEvent
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.MatchsInEventUrl", "");
			}
		}

		public static string GetOnGoingMatch
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.OnGoingMatchUrl", "");
			}
		}

		public static string GetPlayersInEvent
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.PlayersInEventUrl", "");
			}
		}

		public static string GetPlayersInSeason
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.PlayersInSeasonUrl", "");
			}
		}

		public static string GetPlayersRank
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.PlayersRank", "");
			}
		}

		public static string GetWorldSnookerResult
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.WorldSnookerMatchResult", "");
			}
		}

		public static string GetHeadToHead
		{
			get
			{
				return Common.ReadSettingString("SnookerAPI.HeadToHead", "");
			}
		}
	}
}
