namespace Core.SharedKernel.Domain
{
	public interface IDomainEvent
	{
		public Guid Id { get; }
		public DateTime OccuredOn { get; }
	}
}
