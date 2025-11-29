
namespace Core.APIGateway.DelegatingHandlers
{
	public class CorrelationIdDelegatingHandler : DelegatingHandler
	{
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var correlationId = request.Headers.Contains("X-Correlation-ID")
				? request.Headers.GetValues("X-Correlation-ID").First()
				: Guid.NewGuid().ToString();

			if (request.Headers.Contains("X-Correlation-ID"))
				request.Headers.Remove("X-Correlation-ID");

			request.Headers.Add("X-Correlation-ID", correlationId);
			Console.WriteLine($"[Gateway] Processing Request. TraceID: {correlationId}");

			return base.SendAsync(request, cancellationToken);
		}
	}
}
