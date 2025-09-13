using Core.Logging;
using Core.SharedKernel.ValueObjects;
using MediatR;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.PaymentQueries;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.PaymentQueryHandlers
{
	public class GetAvailableRefundAmountQueryHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<GetAvailableRefundAmountQueryHandler> logger) : IRequestHandler<GetAvailableRefundAmountQuery, Money>
	{
		public async Task<Money> Handle(GetAvailableRefundAmountQuery request, CancellationToken cancellationToken)
		{
			var payment = await paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
			if(payment == null)
			{
				logger.LogWarning($"Payment with ID {request.PaymentId} not found.");
				throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");
			}
			var availableRefundAmount = payment.GetAvailableRefundAmount();
			logger.LogInformation($"The available refund amount for payment with ID {request.PaymentId} is {availableRefundAmount}");
			return availableRefundAmount;
		}
	}
}
