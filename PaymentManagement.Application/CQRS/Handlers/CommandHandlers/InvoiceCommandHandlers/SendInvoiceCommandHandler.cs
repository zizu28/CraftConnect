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
	public class SendInvoiceCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<SendInvoiceCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<SendInvoiceCommand>
	{
		public async Task Handle(SendInvoiceCommand request, CancellationToken cancellationToken)
		{
			var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (existingInvoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				existingInvoice.Send();
				var domainEvents = existingInvoice.DomainEvents.ToList();
				var sentEvent = domainEvents.OfType<InvoiceGeneratedIntegrationEvent>().FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(existingInvoice, cancellationToken);
					if (sentEvent != null)
					{
						await messageBroker.PublishAsync(sentEvent, cancellationToken);
					}
				}, cancellationToken);
				backgroundJob.Enqueue<IGmailService>(
					"InvoiceSent",
					invoice => invoice.SendEmailAsync(
						request.RecipientEmail,
						"INVOICE DELIVERED",
						$"Invoice with ID {request.InvoiceId} has been delivered to recipient with email {request.RecipientEmail}.",
						false,
						CancellationToken.None
					)
				);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error sending invoice with ID {request.InvoiceId} to {request.RecipientEmail}");
				throw;
			}
		}
	}
}
