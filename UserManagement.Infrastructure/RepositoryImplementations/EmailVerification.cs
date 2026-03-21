using Core.SharedKernel.Domain;
using Infrastructure.Persistence.Data;
using UserManagement.Application.Contracts;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class EmailVerification(ApplicationDbContext dbContext) : IEmailVerification
	{
		public async Task AddAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(token);
			await dbContext.EmailVerificationTokens.AddAsync(token, cancellationToken);
		}
	}
}
