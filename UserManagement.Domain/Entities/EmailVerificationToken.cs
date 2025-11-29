using System.Security.Cryptography;

namespace UserManagement.Domain.Entities
{
	public class EmailVerificationToken
	{
		public Guid EmailVerificationTokenId { get; set; }
		public string TokenValue { get; set; } = string.Empty;
		public Guid UserId { get; set; }
		public User User { get; set; }
		public DateTime CreatedOnUtc { get; set; }
		public DateTime ExpiresOnUtc { get; set; }

		public static string GenerateToken()
		{
			var randomBytes = new byte[32];
			using(var rng = RandomNumberGenerator.Create())
			{
				rng.GetBytes(randomBytes);
			}
			return Convert.ToBase64String(randomBytes);
		}
	}
}
