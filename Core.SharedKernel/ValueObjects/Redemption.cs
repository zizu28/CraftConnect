namespace Core.SharedKernel.ValueObjects
{
	public record Redemption
	{
		public int Count { get; private set; }
		public int Limit { get; private set; }

		private Redemption()
		{
			Limit = 0;
			Count = 0;
		}
		public Redemption(int count, int limit)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(count);
			ArgumentOutOfRangeException.ThrowIfNegative(limit);
			Count = count;
			Limit = limit;
		}
	}
}
