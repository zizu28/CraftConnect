using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.InvoiceIntegrationEvents;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class CancelInvoiceCommandHandler(
		IInvoiceRepository invoiceRepository,
		ILoggingService<CancelInvoiceCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob,
		IUnitOfWork unitOfWork) : IRequestHandler<CancelInvoiceCommand, Unit>
	{
		public async Task<Unit> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
		{
			var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (existingInvoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			existingInvoice.Cancel(request.Reason);
			var domainEvents = existingInvoice.DomainEvents.ToList();
			var cancelledEvent = domainEvents.OfType<InvoiceCancelledIntegrationEvent>().FirstOrDefault();
			await unitOfWork.ExecuteInTransactionAsync(async () =>
			{
				await invoiceRepository.UpdateAsync(existingInvoice, cancellationToken);
				if(cancelledEvent != null)
					await messageBroker.PublishAsync(cancelledEvent, cancellationToken);
				existingInvoice.ClearEvents();
			}, cancellationToken);

			backgroundJob.Enqueue<IGmailService>(
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
