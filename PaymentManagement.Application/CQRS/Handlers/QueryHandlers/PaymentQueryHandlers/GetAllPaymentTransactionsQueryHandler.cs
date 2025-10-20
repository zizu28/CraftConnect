using Core.Logging;
using MediatR;
using Microsoft.Extensions.Configuration;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Application.CQRS.Queries.PaymentQueries;
using PaymentManagement.Domain.Entities;
using PayStack.Net;

namespace PaymentManagement.Application.CQRS.Handlers.QueryHandlers.PaymentQueryHandlers
{
	public class GetAllPaymentTransactionsQueryHandler(
		IPaymentRepository paymentRepository,
		ILoggingService<GetAllPaymentTransactionsQueryHandler> logger,
		IConfiguration configuration) : IRequestHandler<GetAllPaymentTransactionsQuery, IEnumerable<PaymentTransaction>>
	{
		private readonly PayStackApi payStack = new(configuration["Paystack:SecretKey"]);
		public async Task<IEnumerable<PaymentTransaction>> Handle(GetAllPaymentTransactionsQuery request, CancellationToken cancellationToken)
		{
			var transactions = new List<PaymentTransaction>();
			var payments = await paymentRepository.GetAllAsync(cancellationToken);
			if (!payments.Any())
			{

				return Enumerable.Empty<PaymentTransaction>();
			}

			foreach(var payment in payments)
			{
				payment.Transactions
					.ToList()
					.ForEach(transaction => transactions.Add(transaction));
			}

			return transactions;
		}
	}
}
