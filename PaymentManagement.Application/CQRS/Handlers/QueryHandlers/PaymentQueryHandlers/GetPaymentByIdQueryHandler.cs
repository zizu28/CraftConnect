using AutoMapper;
using Core.Logging;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.PaymentQueries;
using PaymentManagement.Application.DTOs.PaymentDTOs;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.PaymentQueryHandlers
{
	public class GetPaymentByIdQueryHandler(
		IMapper mapper,
		ILoggingService<GetPaymentByIdQueryHandler> logger,
		IPaymentRepository paymentRepository) : IRequestHandler<GetPaymentByIdQuery, PaymentResponseDTO>
	{
		public async Task<PaymentResponseDTO> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
		{
			logger.LogInformation("Handling GetPaymentByIdQuery for PaymentId: {PaymentId}", request.PaymentId);
			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken)
				?? throw new ArgumentNullException($"Payment with ID {request.PaymentId} not found.");
			var paymentDTO = mapper.Map<PaymentResponseDTO>(payment);
			paymentDTO.IsSuccess = true;
			paymentDTO.Message = $"Payment with ID {paymentDTO.Id} successfully retrieved.";
			logger.LogInformation("Successfully handled GetPaymentByIdQuery for PaymentId: {PaymentId}", request.PaymentId);
			return paymentDTO;
		}
	}
}
