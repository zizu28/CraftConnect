using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class AddLineItemCommandHandler(
		IInvoiceRepository invoiceRepository,
		ILoggingService<AddLineItemCommandHandler> logger,
		IUnitOfWork unitOfWork) : IRequestHandler<AddLineItemCommand>
	{
		public async Task Handle(AddLineItemCommand request, CancellationToken cancellationToken)
		{
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (invoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				invoice.AddLineItem(request.Description, request.UnitPrice, request.Quantity, request.ItemCode);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(invoice);
				}, cancellationToken);
				logger.LogInformation($"Line item added to invoice with ID {request.InvoiceId}.");
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error adding line item to invoice with ID {request.InvoiceId}: {ex.Message}");
				throw;
			}
		}
	}
}
