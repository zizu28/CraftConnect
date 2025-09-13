using Core.Logging;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.InvoiceQueries;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.InvoiceQueryHandlers
{
	public class GetDaysUntilDueQueryHandler(
		IInvoiceRepository invoiceRepository,
		ILoggingService<GetDaysUntilDueQueryHandler> logger) : IRequestHandler<GetDaysUntilDueQuery, int>
	{
		public async Task<int> Handle(GetDaysUntilDueQuery request, CancellationToken cancellationToken)
		{
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken);
			if (invoice == null)
			{
				logger.LogWarning($"Invoice with ID {request.InvoiceId} not found.");
				throw new KeyNotFoundException($"Invoice with ID {request.InvoiceId} not found.");
			}
			var daysUntilDue = invoice.GetDaysUntilDue();
			logger.LogInformation($"Days until due for invoice with ID {request.InvoiceId} is {daysUntilDue} days.");
			return daysUntilDue;
		}
	}
}
