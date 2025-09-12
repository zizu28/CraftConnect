using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class DeleteInvoiceCommandHandler(
		IInvoiceRepository invoiceRepository,
		ILoggingService<DeleteInvoiceCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<DeleteInvoiceCommand, Unit>
	{
		public async Task<Unit> Handle(DeleteInvoiceCommand request, CancellationToken cancellationToken)
		{
			var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (existingInvoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				await invoiceRepository.DeleteAsync(existingInvoice.Id, cancellationToken);
				await messageBroker.PublishAsync(
					new InvoiceCancelledIntegrationEvent(
						existingInvoice.Id,
						existingInvoice.InvoiceNumber,
						existingInvoice.IssuedTo,
						request.Reason), cancellationToken);
			}, cancellationToken);

			backgroundJob.Enqueue<IEmailService>(
				"InvoiceDelete",
				invoice => invoice.SendEmailAsync(
					existingInvoice.Recipient.Email.Address.ToString(),
					"INVOICE DELETED",
					$"Invoice with ID {request.InvoiceId} has been deleted successfully.",
					false,
					CancellationToken.None));
			logger.LogInformation($"Invoice with ID {request.InvoiceId} deleted successfully.");
			return Unit.Value;
		}
	}
}
