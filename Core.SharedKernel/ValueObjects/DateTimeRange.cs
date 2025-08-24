using NodaTime;

namespace Core.SharedKernel.ValueObjects
{
	public record DateTimeRange(LocalDateTime Start, LocalDateTime End)
	{
		public bool IsValid => Start < End;
		public Period Duration => Period.Between(Start, End);
	}
}
