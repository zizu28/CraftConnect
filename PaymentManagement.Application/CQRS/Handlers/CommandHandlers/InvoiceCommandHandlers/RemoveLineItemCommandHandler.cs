using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class RemoveLineItemCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<RemoveLineItemCommandHandler> logger) : IRequestHandler<RemoveLineItemCommand>
	{
		public async Task Handle(RemoveLineItemCommand request, CancellationToken cancellationToken)
		{
			var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (existingInvoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				existingInvoice.RemoveLineItem(request.LineItemId);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(existingInvoice);
				}, cancellationToken);
				logger.LogInformation($"Line item with ID {request.LineItemId} removed from invoice with ID {request.InvoiceId}.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error removing line item with ID {request.LineItemId} from invoice with ID {request.InvoiceId}.");
				throw;
			}
		}
	}
}
