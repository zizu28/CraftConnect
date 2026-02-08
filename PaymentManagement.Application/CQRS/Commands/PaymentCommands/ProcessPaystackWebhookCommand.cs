using Core.SharedKernel.Enums;
using MediatR;

namespace PaymentManagement.Application.CQRS.Commands.PaymentCommands
{
	public class ProcessPaystackWebhookCommand : IRequest<Unit>
	{
		public string RecipientEmail { get; set; }
		public string Status { get; set; }
		public Data Data { get; set; }
	}

	public class Data()
	{
		public int Id { get; set; }
		public string Status { get; set; }
	}
}
