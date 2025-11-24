using Microsoft.Extensions.Logging;

namespace Core.SharedKernel
{
	public abstract class BaseDelegatingHandler(
		ILogger<BaseDelegatingHandler> logger) : DelegatingHandler
	{
		private readonly ILogger<BaseDelegatingHandler> _logger = logger;
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			var correlationId = GetCorrelationId(request);
			var moduleName = GetType().Name.Replace("DelegatingHandler", "");

			await PreProcessAsync(request, correlationId);
			var response = await base.SendAsync(request, cancellationToken);
			await PostProcessAsync(request, response, correlationId);

			return response;
		}

		protected virtual Task PostProcessAsync(HttpRequestMessage request, HttpResponseMessage response, object correlationId)
		{
			return Task.CompletedTask;
		}

		protected virtual Task PreProcessAsync(HttpRequestMessage request, object correlationId)
		{
			request.Headers.Add("X-Module-Name", GetType().Name.Replace("DelegatingHandler", ""));
			return Task.CompletedTask;
		}

		protected string GetCorrelationId(HttpRequestMessage request)
		{
			return request.Headers.GetValues("X-Correlation-ID").FirstOrDefault() ?? "unknown";
		}

		protected async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
		{
			var clone = new HttpRequestMessage(request.Method, request.RequestUri);

			// Copy headers
			foreach (var header in request.Headers)
				clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

			// Copy content
			if (request.Content != null)
			{
				var bytes = await request.Content.ReadAsByteArrayAsync();
				clone.Content = new ByteArrayContent(bytes);

				foreach (var header in request.Content.Headers)
					clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			return clone;
		}
	}
}
