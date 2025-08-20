namespace Infrastructure.EmailService
{
	public interface IEmailService
	{
		Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
		Task SendTemplatedEmailAsync<T>(string to, string subject, string templatePath, T model, bool isHtml = false, CancellationToken cancellationToken = default);
		Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<EmailAttachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default);
	}

	public record EmailAttachment(string FileName, byte[] Content, string ContentType);
}