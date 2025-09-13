using Core.EventServices;
using Core.Logging;
using Core.SharedKernel.IntegrationEvents.InvoiceIntegrationEvents;
using Core.SharedKernel.ValueObjects;
using Infrastructure.BackgroundJobs;
using Infrastructure.EmailService.GmailService;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class MarkAsPaidCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<MarkAsPaidCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<MarkAsPaidCommand>
	{
		public async Task Handle(MarkAsPaidCommand request, CancellationToken cancellationToken)
		{
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (invoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				invoice.MarkAsPaid(request.PaymentId, new Money(request.AmountPaid, request.Currency));
				var domainEvents = invoice.DomainEvents.ToList();

				foreach(var domainEvent in domainEvents)
				{
					switch (domainEvent)
					{
						case InvoicePartiallyPaidIntegrationEvent partiallyPaidEvent:
							await unitOfWork.ExecuteInTransactionAsync(async () =>
							{
								await invoiceRepository.UpdateAsync(invoice, cancellationToken);
								await messageBroker.PublishAsync(partiallyPaidEvent, cancellationToken);
							}, cancellationToken);
							break;

						default:
							await unitOfWork.ExecuteInTransactionAsync(async () =>
							{
								await invoiceRepository.UpdateAsync(invoice, cancellationToken);
								await messageBroker.PublishAsync(domainEvent, cancellationToken);
							}, cancellationToken);
							break;
					}
				}
				invoice.ClearEvents();
				logger.LogInformation($"Invoice with ID {request.InvoiceId} marked as paid.");
				backgroundJob.Enqueue<IGmailService>(
					"InvoicePayment",
					payment => payment.SendEmailAsync(
						request.RecipientEmail,
						invoice.Status.ToString() == "Paid" ? "INVOICE FULL PAYMENT" : "INVOICE PARTIAL PAYMENT",
						$"Invoice with ID {request.InvoiceId} has a status of {invoice.Status} payment",
						false,
						CancellationToken.None));
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error marking invoice as paid.");
				throw;
			}
		}
	}
}
