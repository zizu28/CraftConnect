namespace UserManagement.Application.Exceptions
{
	public class NotFoundException(string message) : ApplicationException(message)
	{
	}
}
