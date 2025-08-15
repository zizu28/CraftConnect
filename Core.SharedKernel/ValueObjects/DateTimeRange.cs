namespace Core.SharedKernel.ValueObjects
{
	public record DateTimeRange(DateTime Start, DateTime End)
	{
		public bool IsValid => Start < End;
		public TimeSpan Duration => End - Start;
	}
}
