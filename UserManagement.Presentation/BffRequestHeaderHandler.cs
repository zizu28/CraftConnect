
using System.Net.Http.Headers;

namespace UserManagement.Presentation
{
	public class BffRequestHeaderHandler(IHttpContextAccessor contextAccessor) : DelegatingHandler
	{
		private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var httpContext = _contextAccessor.HttpContext;
			if(httpContext != null)
			{
				if(httpContext.Request.Headers.TryGetValue("X-Access-Token", out var token))
				{
					request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
				}
				if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
				{
					request.Headers.Add("X-Correlation-ID", correlationId.ToString());
				}
			}
			return base.SendAsync(request, cancellationToken);
		}
	}
}
