namespace Core.SharedKernel.Domain
{
	public interface IIntegrationEvent
	{
		Guid EventId { get; }
		DateTime OccuredOn { get; }
	}

	public interface IIntegrationEvent<T> : IIntegrationEvent where T : class
	{
		T Data { get; }
	}
}
