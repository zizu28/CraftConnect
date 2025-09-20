using AutoMapper;
using Core.Logging;
using Infrastructure.Persistence.UnitOfWork;
using MediatR;
using Newtonsoft.Json;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Commands.PaymentCommands;
using PayStack.Net;

namespace PaymentManagement.Application.CQRS.Handlers.CommandHandlers.PaymentCommandHandlers
{
	public class VerifyPaymentCommandHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<VerifyPaymentCommandHandler> logger,
		PayStackApi payStackApi,
		IMapper mapper,
		IUnitOfWork unitOfWork) : IRequestHandler<VerifyPaymentCommand, (string Message, bool Status)>
	{
		public async Task<(string Message, bool Status)> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
		{
			var payment = await paymentRepository.GetByIdAsync(request.PaymentDTO.PaymentId, cancellationToken);
			if ( payment == null)
			{
				logger.LogWarning($"Payment with Id {request.PaymentDTO.PaymentId} not found.");
				throw new KeyNotFoundException($"Payment with Id {request.PaymentDTO.PaymentId} not found.");
			}

			TransactionVerifyResponse response = payStackApi.Transactions.Verify(request.Reference);
			try
			{
				logger.LogInformation($"Reference:: {request.Reference}");
				if (response.Status)
				{
					logger.LogInformation($"Response from PayStack:: {JsonConvert.SerializeObject(response)}");
					request.PaymentDTO.Status = response.Data.Status;
					request.PaymentDTO.ModifiedAt = response.Data.TransactionDate;
					mapper.Map(request.PaymentDTO, payment);
					await unitOfWork.ExecuteInTransactionAsync(async () =>
					{
						await paymentRepository.UpdateAsync(payment);
					}, cancellationToken);
				}
				logger.LogInformation($"Response from PayStack:: {JsonConvert.SerializeObject(response)}");
				logger.LogInformation($"Response from PayStack:: {JsonConvert.SerializeObject(response)}");
				request.PaymentDTO.Status = response.Data.Status;
				request.PaymentDTO.ModifiedAt = response.Data.TransactionDate;
				mapper.Map(request.PaymentDTO, payment);
				await unitOfWork.ExecuteInTransactionAsync(async () =>
				{
					await paymentRepository.UpdateAsync(payment);
				}, cancellationToken);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An error occurred while verifying the payment.");
				throw;
			}
			return response.Status ? ("Payment verified successfully.", true) : ("Payment verification failed.", false);
		}
	}
}
