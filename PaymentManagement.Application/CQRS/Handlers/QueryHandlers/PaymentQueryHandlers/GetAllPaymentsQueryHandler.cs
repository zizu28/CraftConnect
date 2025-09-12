using AutoMapper;
using Core.Logging;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.PaymentQueries;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.PaymentQueryHandlers
{
	public class GetAllPaymentsQueryHandler(
		IMapper mapper,
		ILoggingService<GetAllPaymentsQueryHandler> logger,
		IPaymentRepository paymentRepository) : IRequestHandler<GetAllPaymentsQuery, IEnumerable<PaymentResponseDTO>>
	{
		public async Task<IEnumerable<PaymentResponseDTO>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetAllPaymentsQuery");
			var payments = await paymentRepository.GetAllAsync(cancellationToken)
				?? throw new ArgumentNullException("Could not retrieve list of payments from database or cache.");
			var paymentDTOs = mapper.Map<IEnumerable<PaymentResponseDTO>>(payments);
			foreach(var paymentDTO in paymentDTOs)
			{
				paymentDTO.IsSuccess = true;
				paymentDTO.Message = $"Payment with ID {paymentDTO.Id} successfully retrieved.";
			}
			logger.LogInformation("Successfully handled GetAllPaymentsQuery");
			return paymentDTOs;
		}
	}
}
