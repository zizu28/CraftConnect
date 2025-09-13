using Core.Logging;
using Core.SharedKernel.ValueObjects;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.InvoiceCommands;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.InvoiceCommandHandlers
{
	public class ApplyDiscountCommandHandler(
		IInvoiceRepository invoiceRepository,
		IUnitOfWork unitOfWork,
		ILoggingService<ApplyDiscountCommandHandler> logger) : IRequestHandler<ApplyDiscountCommand>
	{
		public async Task Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
		{
			var existingInvoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (existingInvoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			try
			{
				existingInvoice.ApplyDiscount(new Money(request.DiscountAmount, request.Currency));
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await invoiceRepository.UpdateAsync(existingInvoice, cancellationToken);
				}, cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error applying discount to invoice with ID {request.InvoiceId}.");
				throw;
			}
		}
	}
}
