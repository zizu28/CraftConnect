using FluentEmail.Core;
using FluentEmail.Core.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Infrastructure.EmailService
{
	public class DkimEmailService(IFluentEmail fluentEmail, DkimSigner signer) : IEmailService
	{
		private readonly IFluentEmail _fluentEmail = fluentEmail;
		private readonly DkimSigner _signer = signer;

		public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var email = _fluentEmail
				.To(to)
				.Subject(subject)
				.Body(body, isHtml);

			MimeMessage mimeMessage = (MimeMessage)email;
			var signedMessage = _signer.Sign(mimeMessage);

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync("smtp.<mydomain>.com", port: 586, SecureSocketOptions.StartTls, cancellationToken);
			await smtp.SendAsync(signedMessage, cancellationToken);
			smtp.Disconnect(true, cancellationToken);
		}

		public async Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<EmailAttachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var email = _fluentEmail
				.To(to)
				.Subject(subject)
				.Body(body, isHtml)
				.Attach(attachments.Select(attachment => new Attachment
				{
					Filename = attachment.FileName,
					Data = new MemoryStream(attachment.Content),
					ContentType = attachment.ContentType
				}));

			var mimeMessage = (MimeMessage)email;
			var signedMessage = _signer.Sign(mimeMessage);

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync("smtp.<mydomain>.com", port: 586, SecureSocketOptions.StartTls, cancellationToken);
			await smtp.SendAsync(signedMessage, cancellationToken);
			smtp.Disconnect(true, cancellationToken);
		}

		public async Task SendTemplatedEmailAsync<T>(string to, string subject, string templatePath, T model, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var email = _fluentEmail.
				To(to)
				.Subject(subject)
				.UsingTemplateFromFile(templatePath, model);

			var mimeMessage = (MimeMessage)email;
			var signedMessage = _signer.Sign(mimeMessage);

			using var smtp = new SmtpClient();
			await smtp.ConnectAsync("smtp.<mydomain>.com", port: 586, SecureSocketOptions.StartTls, cancellationToken);
			await smtp.SendAsync(signedMessage, cancellationToken);
			smtp.Disconnect(true, cancellationToken);
		}
	}
}
