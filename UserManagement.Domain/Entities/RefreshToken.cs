namespace UserManagement.Domain.Entities
{
	public class RefreshToken
	{
		public Guid Id { get; set; }
		public string Token { get; set; } = string.Empty;
		public DateTime ExpiresOnUtc { get; set; }
		public Guid UserId { get; set; }
		public User User { get; set; }
	}
}
