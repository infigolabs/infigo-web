using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using UMServer.Settings;

namespace UMServer.Services
{
	public class EmailService : IEmailService
	{
		public EmailSettings EmailSettings { get; private set; }

		public EmailService(IConfiguration configuration)
		{
			EmailSettings = new EmailSettings
			{
				Email = configuration["Moderator:EmailSettings:Email"],
				FirstName = configuration["Moderator:EmailSettings:FirstName"],
				LastName = configuration["Moderator:EmailSettings:LastName"],
				Password = configuration["Moderator:EmailSettings:Password"],
				Host = configuration["Moderator:EmailSettings:Host"]
			};
		}

		/// <summary>
		/// Send email to provided address.
		/// </summary>
		/// <param name="toAddress"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public void Send(string toAddress, string subject, string body)
		{
			MailMessage message = new MailMessage();
			SmtpClient smtp = new SmtpClient();
			message.From = new MailAddress(EmailSettings.Email);
			message.To.Add(new MailAddress(toAddress));
			message.Subject = subject;
			message.Body = body;
			message.IsBodyHtml = true;
			smtp.Port = 587;
			smtp.Host = EmailSettings.Host;
			smtp.EnableSsl = true;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new NetworkCredential(EmailSettings.Email, EmailSettings.Password);
			smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtp.Send(message);
		}
	}
}
