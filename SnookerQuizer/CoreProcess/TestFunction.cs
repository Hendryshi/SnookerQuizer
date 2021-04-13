using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SnookerQuizer.Helper;

namespace SnookerQuizer.CoreProcess
{
	class TestFunction
	{
		public static void SendMail()
		{
			string HTMLBody = File.ReadAllText(@"C:\Users\SHI YEJIA\Desktop\WatchNoticeTemplate.htm");
			
			MailHelper.SendMail("yejia.shi@hotmail.com", "Result", HTMLBody);
		}
	}
}
