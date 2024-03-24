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
				Email = configuration["EmailSettings:Email"],
				FirstName = configuration["EmailSettings:FirstName"],
				LastName = configuration["EmailSettings:LastName"],
				Password = configuration["EmailSettings:Password"],
				Host = configuration["EmailSettings:Host"],
				Port = int.Parse(configuration["EmailSettings:Port"])
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
			message.IsBodyHtml = false;
			smtp.Port = EmailSettings.Port;
			smtp.Host = EmailSettings.Host;
			smtp.EnableSsl = true;
			smtp.Timeout = 20000;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new NetworkCredential(EmailSettings.Email, EmailSettings.Password);
			smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtp.Send(message);
		}
	}
}
