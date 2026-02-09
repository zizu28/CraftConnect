using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace PaymentManagement.Application.Services
{
	/// <summary>
	/// Service for verifying Paystack webhook signatures to ensure authenticity
	/// </summary>
	public interface IPaystackWebhookVerifier
	{
		/// <summary>
		/// Verifies that a webhook request came from Paystack
		/// </summary>
		/// <param name="signature">The signature from x-paystack-signature header</param>
		/// <param name="payload">The raw JSON payload from the request body</param>
		/// <returns>True if signature is valid, false otherwise</returns>
		bool VerifySignature(string signature, string payload);
	}

	/// <summary>
	/// Implementation of Paystack webhook signature verification
	/// See: https://paystack.com/docs/payments/webhooks#verify-webhook-signature
	/// </summary>
	public class PaystackWebhookVerifier(IConfiguration configuration) : IPaystackWebhookVerifier
	{
		private readonly string _secretKey = configuration["Paystack:SecretKey"]!;

		public bool VerifySignature(string signature, string payload)
		{
			if (string.IsNullOrWhiteSpace(signature))
				return false;

			if (string.IsNullOrWhiteSpace(payload))
				return false;

			try
			{
				using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_secretKey));
				var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
				var computedSignature = Convert.ToHexStringLower(hash);

				// Compare the computed signature with the provided signature
				// Use timing-safe comparison to prevent timing attacks
				return CryptographicOperations.FixedTimeEquals(
					Encoding.UTF8.GetBytes(computedSignature),
					Encoding.UTF8.GetBytes(signature.ToLower())
				);
			}
			catch
			{
				return false;
			}
		}
	}
}
