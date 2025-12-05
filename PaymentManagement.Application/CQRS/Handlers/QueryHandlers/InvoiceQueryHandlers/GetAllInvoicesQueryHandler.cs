using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.InvoiceQueries;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.InvoiceQueryHandlers
{
	public class GetAllInvoicesQueryHandler(
		IMapper mapper,
		ILoggingService<GetAllInvoicesQueryHandler> logger,
		IInvoiceRepository invoiceRepository) : IRequestHandler<GetAllInvoicesQuery, IEnumerable<InvoiceResponseDTO>>
	{
		public async Task<IEnumerable<InvoiceResponseDTO>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetAllInvoicesQuery");
			var invoices = await invoiceRepository.GetAllAsync(cancellationToken)
				?? throw new ArgumentNullException("Could not retrieve list of invoices from database or cache.");
			var invoiceDTOs = mapper.Map<IEnumerable<InvoiceResponseDTO>>(invoices);
			foreach(var invoiceDTO in invoiceDTOs)
			{
				invoiceDTO.IsSuccess = true;
				invoiceDTO.Message = $"Invoice with ID {invoiceDTO.Id} successfully retrieved.";
			}
			logger.LogInformation("Successfully handled GetAllInvoicesQuery");
			return invoiceDTOs;
		}
	}
}
