using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Cryptography;

namespace Infrastructure.EmailService
{
	public class DkimSigner
	{
		private readonly MimeKit.Cryptography.DkimSigner _signer;
		public DkimSigner(IConfiguration config)
		{
			var domain = config["DKIM:Domain"];
			var selector = config["DKIM:Selector"];
			var privateKey = File.ReadAllText(config["DKIM:PivateKeyPath"]!);

			_signer = new MimeKit.Cryptography.DkimSigner(privateKey, domain, selector)
			{
				SignatureAlgorithm = DkimSignatureAlgorithm.RsaSha256,
				HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
				BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Relaxed,
				AgentOrUserIdentifier = $"@{domain}"
			};
		}

		public MimeMessage Sign(MimeMessage message)
		{
			_signer.Sign(message, headers: ["From", "To", "Subject"]);
			return message;
		}
	}
}
