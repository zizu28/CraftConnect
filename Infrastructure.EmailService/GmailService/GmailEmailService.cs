//using Microsoft.Extensions.Configuration;
//using System.Net;
//using System.Net.Mail;

//namespace Infrastructure.EmailService.GmailService
//{
//	public class GmailEmailService(IConfiguration configuration) : IGmailService
//	{
//		public async Task ConfirmEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
//		{
//			var emailSettings = configuration.GetSection("SMTP");
//			var host = emailSettings["Host"];
//			var port = int.Parse(emailSettings["Port"] ?? "587");
//			var fromEmail = emailSettings["FromEmail"];
//			var password = emailSettings["Password"];

//			var smtpClient = new SmtpClient(host, port)
//			{
//				EnableSsl = true,
//				UseDefaultCredentials = false,
//				Credentials = new NetworkCredential(fromEmail, password)
//			};

//			var mailMessage = new MailMessage(fromEmail!, to, subject, body);
//			await smtpClient.SendMailAsync(mailMessage, cancellationToken);
//		}

//		public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
//		{
//			var emailSettings = configuration.GetSection("SMTP");
//			var host = emailSettings["Host"];
//			var port = int.Parse(emailSettings["Port"] ?? "587");
//			var fromEmail = emailSettings["FromEmail"];
//			var password = emailSettings["Password"];

//			var smtpClient = new SmtpClient(host, port)
//			{
//				EnableSsl = true,
//				UseDefaultCredentials = false,
//				Credentials = new NetworkCredential(fromEmail, password)
//			};

//			var mailMessage = new MailMessage(fromEmail!, to, subject, body)
//			{
//				IsBodyHtml = isHtml
//			};
//			await smtpClient.SendMailAsync(mailMessage, cancellationToken);
//		}
//	}
//}

using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.EmailService.GmailService
{
	public class GmailEmailService : IGmailService
	{
		private readonly string _host;
		private readonly int _port;
		private readonly string _fromEmail;
		private readonly string _password;

		public GmailEmailService(IConfiguration configuration)
		{
			var section = configuration.GetSection("SMTP");
			_host = section["Host"] ?? "smtp.gmail.com";
			_port = int.Parse(section["Port"] ?? "587");
			_fromEmail = section["FromEmail"]!;
			_password = section["Password"]!;
		}

		public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			using var smtpClient = new SmtpClient(_host, _port)
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(_fromEmail, _password),
				DeliveryMethod = SmtpDeliveryMethod.Network
			};

			using var mailMessage = new MailMessage
			{
				From = new MailAddress(_fromEmail, "CraftConnect Support"), // Add a Display Name
				Subject = subject,
				Body = body,
				IsBodyHtml = isHtml
			};

			mailMessage.To.Add(to);

			try
			{
				await smtpClient.SendMailAsync(mailMessage, cancellationToken);
			}
			catch (SmtpException ex)
			{
				throw new InvalidOperationException($"SMTP Error: {ex.Message} - Status: {ex.StatusCode}", ex);
			}
		}

		public Task ConfirmEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
		{
			return SendEmailAsync(to, subject, body, true, cancellationToken);
		}
	}
}
