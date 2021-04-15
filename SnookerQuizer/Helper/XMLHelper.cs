using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Serilog;

namespace SnookerQuizer.Helper
{
	public class XMLHelper
	{
		public static void SaveListToXML<T>(List<T> list, string xmlName)
		{
			string savePath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			Log.Information(string.Format("Saving xml {0}", xmlName));
			using(Stream savestream = new FileStream(savePath, FileMode.Create))
			{
				XmlSerializer ser = new XmlSerializer(typeof(List<T>));
				ser.Serialize(savestream, list);
			}
		}

		public static List<T> LoadXMLToList<T>(string xmlName)
		{
			string loadPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			Log.Information(string.Format("Loading xml {0}", xmlName));
			using(StreamReader reader = new StreamReader(loadPath))
			{
				XmlSerializer deserializer = new XmlSerializer(typeof(List<T>));
				return (List<T>)deserializer.Deserialize(reader);
			}
		}

		public static void SaveToXML<T>(T obj, string xmlName)
		{
			string savePath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			Log.Information(string.Format("Saving xml {0}", xmlName));
			using(Stream savestream = new FileStream(savePath, FileMode.Create))
			{
				XmlSerializer ser = new XmlSerializer(typeof(T));
				ser.Serialize(savestream, obj);
			}
		}

		public static T LoadXML<T>(string xmlName)
		{
			string loadPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			Log.Information(string.Format("Loading xml {0}", xmlName));
			using(StreamReader reader = new StreamReader(loadPath))
			{
				XmlSerializer deserializer = new XmlSerializer(typeof(T));
				return (T)deserializer.Deserialize(reader);
			}
		}

		public static bool XMLExist(string xmlName)
		{
			return File.Exists(Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName));
		}

		public static void XMLCopy(string sourceName, string destName, bool isOverWritten = true)
		{
			File.Copy(Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], sourceName), Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], destName), isOverWritten);
		}

		public static List<string> GetXMLList(string searchFilter)
		{
			List<string> xmlNameList = new List<string>();
			string loadPath = ConfigurationManager.AppSettings["DataRootFolder"];

			foreach(string filePath in System.IO.Directory.GetFiles(loadPath, searchFilter, SearchOption.TopDirectoryOnly))
				xmlNameList.Add(Path.GetFileName(filePath));
			
			return xmlNameList;
		}
	}
}
