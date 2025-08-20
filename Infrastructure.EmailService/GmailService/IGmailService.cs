namespace Infrastructure.EmailService.GmailService
{
	public interface IGmailService
	{
		Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default);
		Task ConfirmEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
	}
}
