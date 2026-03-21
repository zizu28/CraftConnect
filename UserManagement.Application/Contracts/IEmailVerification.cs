using UserManagement.Domain.Entities;

namespace UserManagement.Application.Contracts
{
	public interface IEmailVerification
	{
		Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);
	}
}
