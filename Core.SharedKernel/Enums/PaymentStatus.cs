namespace Core.SharedKernel.Enums
{
	public enum PaymentStatus
	{
		Pending,
		Processing,
		Authorized,
		Captured,
		Failed,
		Refunded,
		Cancelled,
		PartiallyRefunded,
		Completed,
	}
}
