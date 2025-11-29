
namespace CraftConnect.WebUI.DelegateHandlers
{
	public class JwtAuthorizationDelegatingHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return base.SendAsync(request, cancellationToken);
		}
	}
}
