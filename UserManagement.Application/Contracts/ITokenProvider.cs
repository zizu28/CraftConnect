using UserManagement.Domain.Entities;

namespace UserManagement.Application.Contracts
{
	public interface ITokenProvider
	{
		Task<string> GenerateRefreshToken(User user);
		string GenerateAccessToken(Guid userId, string emailAddress, string role);
		string GenerateCustomerEmailConfirmationToken(Customer customer);
		string GenerateCraftmanEmailConfirmationToken(Craftman craftman);
		Task RemoveOldRefreshTokens(Guid userId);
	}
}
