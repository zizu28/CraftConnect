using AutoMapper;
using Core.Logging;
using Core.SharedKernel.DTOs;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.InvoiceQueries;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.InvoiceQueryHandlers
{
	public class GetInvoiceByIdQueryHandler(
		IMapper mapper,
		ILoggingService<GetInvoiceByIdQueryHandler> logger,
		IInvoiceRepository invoiceRepository) : IRequestHandler<GetInvoiceByIdQuery, InvoiceResponseDTO>
	{
		public async Task<InvoiceResponseDTO> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetInvoiceByIdQuery for ID: {InvoiceId}", request.InvoiceId);
			var invoice = await invoiceRepository.GetByIdAsync(request.InvoiceId, cancellationToken)
				?? throw new ArgumentNullException($"Could not retrieve invoice with ID {request.InvoiceId} from database or cache.");
			var invoiceDTO = mapper.Map<InvoiceResponseDTO>(invoice);
			invoiceDTO.IsSuccess = true;
			invoiceDTO.Message = $"Invoice with ID {invoiceDTO.Id} successfully retrieved.";
			logger.LogInformation("Successfully handled GetInvoiceByIdQuery for ID: {InvoiceId}", request.InvoiceId);
			return invoiceDTO;
		}
	}
}
