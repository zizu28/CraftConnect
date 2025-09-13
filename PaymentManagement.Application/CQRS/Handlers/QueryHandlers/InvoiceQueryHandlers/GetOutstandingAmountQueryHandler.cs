using Core.Logging;
using Core.SharedKernel.ValueObjects;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.InvoiceQueries;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.InvoiceQueryHandlers
{
	public class GetOutstandingAmountQueryHandler(
		IInvoiceRepository invoiceRepository,
		ILoggingService<GetOutstandingAmountQueryHandler> logger) : IRequestHandler<GetOutstandingAmountQuery, Money>
	{
		public async Task<Money> Handle(GetOutstandingAmountQuery request, CancellationToken cancellationToken)
		{
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (invoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			var outstandingAmount = invoice.GetOutstandingAmount();
			logger.LogInformation($"Outstanding amount for Invoice ID {request.InvoiceId} is {outstandingAmount.Amount} {outstandingAmount.Currency}.");
			return outstandingAmount;
		}
	}
}
