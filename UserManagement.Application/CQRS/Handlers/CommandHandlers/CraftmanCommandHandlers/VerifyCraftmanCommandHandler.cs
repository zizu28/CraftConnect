using Core.Logging;
using Core.SharedKernel.Enums;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using UserManagement.Application.Contracts;
using UserManagement.Application.CQRS.Commands;

namespace UserManagement.Application.CQRS.Handlers.CommandHandlers.CraftmanCommandHandlers
{
	public class VerifyCraftmanCommandHandler(
		ILoggingService<VerifyCraftmanCommandHandler> logger,
		ICraftsmanRepository craftsmanRepository,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<VerifyCraftmanCommand, Unit>
	{
		public async Task<Unit> Handle(VerifyCraftmanCommand request, CancellationToken cancellationToken)
		{
			var craftman = await craftsmanRepository.GetByIdAsync(request.CraftmanId, cancellationToken)
				?? throw new KeyNotFoundException($"Craftman with Id {request.CraftmanId} not found.");
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				if (craftman.Status == VerificationStatus.Unverified)
				{
					craftman.VerifyCraftman(request.Email, request.Profession, request.IdentityDocument);
				}
				await craftsmanRepository.UpdateAsync(craftman, cancellationToken);
				logger.LogInformation("Craftman with Id {CraftmanId} verified successfully", craftman.Id);
			}, cancellationToken);

			backgroundJob.Enqueue<IGmailService>("verify-craftman",
				verify => verify.SendEmailAsync(
					craftman.Email.Address,
					"Verification Status",
					$"Dear {craftman.Username}, you have been successfully verified",
					true,
					CancellationToken.None
				)
			);

			return Unit.Value;
		}
	}
}
