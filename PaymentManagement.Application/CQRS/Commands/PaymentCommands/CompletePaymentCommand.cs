﻿using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class CompletePaymentCommand : IRequest<Unit>
	{
		public Guid PaymentId { get; set; }
		public string RecipientEmail { get; set; }
		public string externalTransactionId { get; set; }
	}
}
