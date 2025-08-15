using Hangfire;
using Infrastructure.EmailService;
using Infrastructure.EmailService.GmailService;

namespace Infrastructure.BackgroundJobs
{
	public class QueuedEmailService(IBackgroundJobClient jobClient)
	{

		public Task SendEmailAsync(string to, string subject, string body, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var jobId = jobClient.Enqueue<IGmailService>(x =>
			x.SendEmailAsync(to, subject, body, isHtml, cancellationToken));
			return Task.FromResult(jobId);
		}

		public Task SendEmailWithAttachmentsAsync(string to, string subject, string body, IEnumerable<EmailAttachment> attachments, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var jobId = jobClient.Enqueue<IEmailService>(x =>
			x.SendEmailWithAttachmentsAsync(to, subject, body, attachments, isHtml, cancellationToken));
			return Task.FromResult(jobId);
		}

		public Task SendTemplatedEmailAsync<T>(string to, string subject, string templatePath, T model, bool isHtml = false, CancellationToken cancellationToken = default)
		{
			var jobId = jobClient.Enqueue<IEmailService>(x =>
			x.SendTemplatedEmailAsync(to, subject, templatePath, model, isHtml, cancellationToken));
			return Task.FromResult(jobId);
		}
	}
}
