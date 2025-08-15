using Core.SharedKernel.Events;
using MassTransit;

namespace BookingManagement.Application.MessageConsumers
{
	public class UserRegisteredEventConsumer : IConsumer<UserRegisteredEvent>
	{
		public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
		{
			var message = context.Message;
		}
	}
}
