using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SnookerQuizer.Helper
{
	class MailHelper
	{
		public static void SendMail(string recipient, string subject, string message)
		{
			
			SmtpClient client = new SmtpClient("smtp-mail.outlook.com");

			string _sender = "yejia.shi@hotmail.com";
			string _password = "Paulshi28";

			client.Port = 587;
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.UseDefaultCredentials = false;
			System.Net.NetworkCredential credentials =
				new System.Net.NetworkCredential(_sender, _password);
			client.EnableSsl = true;
			client.Credentials = credentials;

			try
			{
				MailMessage mail = new MailMessage(_sender.Trim(), recipient.Trim());
				mail.Subject = subject;
				mail.Body = message;

				ContentType mimeType = null;
				mimeType = new System.Net.Mime.ContentType("text/html");
				mimeType.CharSet = "UTF-8";
				AlternateView alternate = AlternateView.CreateAlternateViewFromString(message, mimeType);

				mail.AlternateViews.Add(alternate);

				client.Send(mail);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw ex;
			}
		}
	}
}
