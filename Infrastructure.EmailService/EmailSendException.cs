namespace Infrastructure.EmailService
{
	public class EmailSendException(string? message) : Exception(message)
	{
	}
}