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
	public class MarkAsOverDueCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<MarkAsOverDueCommandHandler> logger,
		IMessageBroker messageBroker,
		IBackgroundJobService backgroundJob) : IRequestHandler<MarkAsOverDueCommand>
	{
		public async Task Handle(MarkAsOverDueCommand request, CancellationToken cancellationToken)
		{
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (invoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				return;
			}
			try
			{
				invoice.MarkAsOverdue();
				var domainEvents = invoice.DomainEvents.ToList();
				var overdueEvent = domainEvents.OfType<InvoiceOverdueIntegrationEvent>().FirstOrDefault();

				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(invoice, cancellationToken);
					if (overdueEvent != null)
					{
						await messageBroker.PublishAsync(overdueEvent, cancellationToken);
					}
				}, cancellationToken);
				backgroundJob.Schedule<IGmailService>(
					"OverdueInvoice",
					overdue => overdue.SendEmailAsync(
						request.RecipientEmail,
						"INVOICE OVERDUE",
						$"Invoice with ID {request.InvoiceId} is overdue.",
						false,
						CancellationToken.None), invoice.DueDate.TimeOfDay);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error marking invoice {request.InvoiceId} as overdue.");
				throw;
			}
		}
	}
}
