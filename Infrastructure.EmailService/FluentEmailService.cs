using FluentEmail.Core;
using Attachment = FluentEmail.Core.Models.Attachment;

namespace Infrastructure.EmailService
{
	public class FluentEmailService(
		IFluentEmail fluentEmail) : IEmailService
	{
		private readonly IFluentEmail _fluentEmail = fluentEmail;

		public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var email = _fluentEmail.
				To(to)
				.Subject(subject)
				.Body(body, isHtml);

			var response = await email.SendAsync(cancellationToken);
			if (!response.Successful)
			{
				throw new EmailSendException($"Failed to send email to {to}. Errors: {string.Join(",", response.ErrorMessages)}");
			}
		}

		public async Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<EmailAttachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var email = _fluentEmail.
				To(to)
				.Subject(subject)
				.Body(body, isHtml);

			foreach(var attachment in attachments)
			{
				email.Attach(new Attachment
				{
					Filename = attachment.FileName,
					Data = new MemoryStream(attachment.Content),
					ContentType = attachment.ContentType
				});
			}

			var response = await email.SendAsync(cancellationToken);
			if (!response.Successful)
			{
				throw new EmailSendException($"Failed to send email with attachments to {to}. Errors: {string.Join(",", response.ErrorMessages)}");
			}
		}

		public async Task SendTemplatedEmailAsync<T>(string to, string subject, string templatePath, T model, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var email = _fluentEmail.
				To(to)
				.Subject(subject)
				.UsingTemplateFromFile(templatePath, model);

			var response = await email.SendAsync(cancellationToken);
			if (!response.Successful)
			{
				throw new EmailSendException($"Failed to send templated email to {to}. Errors: {string.Join(",", response.ErrorMessages)}");
			}
		}
	}
}