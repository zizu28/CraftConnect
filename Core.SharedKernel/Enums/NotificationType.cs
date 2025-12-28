namespace Core.SharedKernel.Enums
{
	public enum NotificationType
	{
		// User Management
		Welcome = 1,
		EmailVerification = 2,
		PasswordReset = 3,
		PasswordChanged = 4,
		AccountDeleted = 5,

		// Booking
		BookingCreated = 10,
		BookingConfirmed = 11,
		BookingCancelled = 12,
		BookingCompleted = 13,
		BookingReminder = 14,

		// Payment
		PaymentReceived = 20,
		PaymentFailed = 21,
		RefundProcessed = 22,
		InvoiceGenerated = 23,

		// System
		SystemAlert = 30,
		SecurityAlert = 31
	}
}
