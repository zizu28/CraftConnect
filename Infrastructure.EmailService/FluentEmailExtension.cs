using FluentEmail.MailKitSmtp;
using Infrastructure.EmailService.GmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.EmailService
{
	public static class FluentEmailExtension
	{
		public static IServiceCollection AddFluentEmailService(this IServiceCollection services, IConfiguration config)
		{
			var from = config["SMTP:FromEmail"];
			var host = config["SMTP:Host"];
			var port = int.Parse(config["SMTP:Port"]!);
			services.AddFluentEmail(from)
				.AddSmtpSender(host, port)
				.AddRazorRenderer();

			services.AddScoped<IEmailService, FluentEmailService>();
			services.AddTransient<IGmailService, GmailEmailService>();
			//services.AddSingleton<MimeKit.Cryptography.DkimSigner>(provider => 
			//new MimeKit.Cryptography.DkimSigner("/path/to/private/key.pem", "yourdomain.com", "selector"));

			return services;
		}
	}
}
