using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class UpdateDueDateCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<UpdateDueDateCommandHandler> logger) : IRequestHandler<UpdateDueDateCommand>
	{
		public async Task Handle(UpdateDueDateCommand request, CancellationToken cancellationToken)
		{
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (invoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				invoice.UpdateDueDate(request.UpdatedDueDate);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(invoice, cancellationToken);
				}, cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error updating due date for Invoice ID {request.InvoiceId}");
				throw;
			}
		}
	}
}
