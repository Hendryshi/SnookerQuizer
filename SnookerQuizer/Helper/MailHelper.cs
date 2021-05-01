using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using SnookerQuizer.Helper.Config;

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
				MailMessage mail = new MailMessage();
				mail.Subject = subject;
				mail.Body = message;
				mail.From = new MailAddress(_sender);

				if(Config.Game.UseFinalEmailTemplate)
				{
					string gamerChart = @"D:\WorkSpace\Data\SnookerQuizer\World Championship 2021\Template\GamerChart.png";
					mail.Attachments.Add(new Attachment(gamerChart));
					mail.Attachments[0].ContentDisposition.Inline = true;
					mail.Attachments[0].ContentId = "1.png";

					string payImage = @"D:\WorkSpace\Data\SnookerQuizer\World Championship 2021\Template\Pay.jpg";
					mail.Attachments.Add(new Attachment(payImage));
					mail.Attachments[1].ContentDisposition.Inline = true;
					mail.Attachments[1].ContentId = "2.png";
				}

				if(!string.IsNullOrEmpty(recipient))
				{
					string[] addressToSplit = recipient.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
					try
					{
						foreach(string add in addressToSplit)
							mail.To.Add(add);
					}
					catch(FormatException)
					{
						throw new Exception(string.Format("Incorrect \"To\" email address to format [{0}]", mail));
					}
				}

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
