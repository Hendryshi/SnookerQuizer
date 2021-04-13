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
	}
}
