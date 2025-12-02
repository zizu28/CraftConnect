using UserManagement.Application.DTOs.UserDTOs;

namespace UserManagement.Application.Responses
{
	public class UpstreamLoginResponse
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public UserResponseDTO User { get; set; }
	}
}
