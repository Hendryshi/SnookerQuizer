using System;
using System.Collections.Generic;
using System.IO;
using SnookerQuizer.Helper;
using SnookerQuizer.Helper.Config;
using SnookerQuizer.Model;

namespace SnookerQuizer.CoreProcess
{
	class TestFunction
	{
		public static void SendMail()
		{
			string HTMLBody = File.ReadAllText(@"C:\Users\SHI YEJIA\Desktop\StripoTemplate.html");
			
			MailHelper.SendMail(Game.UserEmail, "Result", HTMLBody);
		}

		public static void SaveGamer()
		{
			List<Quiz> lstQuiz = new List<Quiz>();
			List<GamePoint> lstPoint = new List<GamePoint>();

			lstQuiz.Add(new Quiz() { IdWinner = 376, GamePoint = 12  });
			lstQuiz.Add(new Quiz() { IdWinner = 376});
			lstPoint.Add(new GamePoint() { dtPoint = DateTime.Today, valPoint = 15 });

			GamerInfo gi1 = new GamerInfo("Yejia", 1014, lstQuiz, lstPoint);

			XMLHelper.SaveToXML(gi1, string.Format(Program.GamerXml, Program.IdEvent, gi1.UserName));
		}

	}
}
