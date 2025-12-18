namespace UserManagement.Domain.Entities
{
	public class RefreshToken
	{
		public Guid Id { get; set; }
		public string Token { get; set; } = string.Empty;
		public string RevokedReason { get; set; } = string.Empty;
		public bool IsRevoked { get; set; }
		public DateTime ExpiresOnUtc { get; set; }
		public DateTime RevokedOnUtc { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
	}
}
