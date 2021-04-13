using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SnookerQuizer.Helper
{
	public class XMLHelper
	{
		public static void SaveListToXML<T>(List<T> list, string xmlName)
		{
			string savePath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			XmlSerializer ser = new XmlSerializer(typeof(List<T>));
			var writer = new FileStream(savePath, FileMode.Create);

			ser.Serialize(writer, list);
			writer.Dispose();
		}

		public static List<T> LoadXMLToList<T>(string xmlName)
		{
			string loadPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			var reader = new StreamReader(loadPath);
			XmlSerializer deserializer = new XmlSerializer(typeof(List<T>));
			return (List<T>)deserializer.Deserialize(reader);
		}

		public static void SaveToXML<T>(T obj, string xmlName)
		{
			string savePath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			XmlSerializer ser = new XmlSerializer(typeof(T));
			var writer = new FileStream(savePath, FileMode.Create);

			ser.Serialize(writer, obj);
			writer.Dispose();
		}

		public static T LoadXML<T>(string xmlName)
		{
			string loadPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName);
			var reader = new StreamReader(loadPath);
			XmlSerializer deserializer = new XmlSerializer(typeof(T));
			return (T)deserializer.Deserialize(reader);
		}

		public static bool XMLExist(string xmlName)
		{
			return File.Exists(Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlName));
		}

		public static List<string> GetXMLList(string xmlFolder)
		{
			List<string> xmlNameList = new List<string>();
			string loadPath = Path.Combine(ConfigurationManager.AppSettings["DataRootFolder"], xmlFolder);

			foreach(string filePath in System.IO.Directory.GetFiles(loadPath, "*.xml", SearchOption.TopDirectoryOnly))
				xmlNameList.Add(Path.GetFileName(filePath));
			
			return xmlNameList;
		}
	}
}
