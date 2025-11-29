namespace CraftConnect.WebUI.DTOs
{
	public class UpstreamLoginResponse
	{
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
		public UserResponseDTO User { get; set; }
	}
}
