using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class UpdateLineItemCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<UpdateLineItemCommandHandler> logger) : IRequestHandler<UpdateLineItemCommand>
	{
		public async Task Handle(UpdateLineItemCommand request, CancellationToken cancellationToken)
		{
			var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (existingInvoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				existingInvoice.UpdateLineItem(request.LineItemId, request.Description, request.UnitPrice, request.Quantity);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(existingInvoice, cancellationToken);
				}, cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Error updating line item {LineItemId} in invoice {InvoiceId}", request.LineItemId, request.InvoiceId);
				throw;
			}
		}
	}
}
