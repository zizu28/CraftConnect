using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.EmailService.GmailService
{
	public class GmailEmailService(IConfiguration configuration) : IGmailService
	{
		public async Task ConfirmEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
		{
			var emailSettings = configuration.GetSection("SMTP");
			var host = emailSettings["Host"];
			var port = int.Parse(emailSettings["Port"] ?? "587");
			var fromEmail = emailSettings["FromEmail"];
			var password = emailSettings["Password"];

			var smtpClient = new SmtpClient(host, port);
			smtpClient.EnableSsl = true;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential(fromEmail, password);

			var mailMessage = new MailMessage(fromEmail!, to, subject, body);
			await smtpClient.SendMailAsync(mailMessage, cancellationToken);
		}

		public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var emailSettings = configuration.GetSection("SMTP");
			var host = emailSettings["Host"];
			var port = int.Parse(emailSettings["Port"] ?? "587");
			var fromEmail = emailSettings["FromEmail"];
			var password = emailSettings["Password"];

			var smtpClient = new SmtpClient(host, port);
			smtpClient.EnableSsl = true;
			smtpClient.UseDefaultCredentials = false;
			smtpClient.Credentials = new NetworkCredential(fromEmail, password);

			var mailMessage = new MailMessage(fromEmail!, to, subject, body)
			{
				IsBodyHtml = isHtml
			};
			await smtpClient.SendMailAsync(mailMessage, cancellationToken);
		}
	}
}
