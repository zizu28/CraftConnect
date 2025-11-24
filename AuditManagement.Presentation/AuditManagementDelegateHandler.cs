using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;

namespace AuditManagement.Presentation
{
	public class AuditManagementDelegateHandler(
		IMemoryCache memoryCache,
		ILogger<AuditManagementDelegateHandler> logger,
		IOptions<AuthConfig> authConfig) : DelegatingHandler
	{
		private readonly IMemoryCache _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
		private readonly ILogger<AuditManagementDelegateHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		private readonly AuthConfig _authConfig = authConfig?.Value ?? throw new ArgumentNullException(nameof(authConfig));
		private readonly ConcurrentDictionary<string, SemaphoreSlim> _tokenLocks = new();
		private readonly TimeSpan _tokenRefreshThreshold = TimeSpan.FromMinutes(5);

		protected override async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			// Add performance tracking
			var stopwatch = System.Diagnostics.Stopwatch.StartNew();

			try
			{
				// Add authorization header with cached token
				await AddAuthorizationHeaderAsync(request, cancellationToken);

				// Add correlation ID for distributed tracing
				AddCorrelationId(request);

				// Add common headers for API optimization
				AddOptimizationHeaders(request);

				// Execute the request with retry policy
				var response = await ExecuteWithRetryPolicyAsync(request, cancellationToken);

				// Log performance metrics
				LogPerformanceMetrics(request, response, stopwatch.Elapsed);

				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error executing HTTP request to {RequestUri}", request.RequestUri);
				throw;
			}
			finally
			{
				stopwatch.Stop();
			}
		}

		private async Task AddAuthorizationHeaderAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// Skip for endpoints that don't require auth
			if (request.RequestUri != null && ShouldSkipAuth(request.RequestUri))
				return;

			var token = await GetOrRefreshTokenAsync(cancellationToken);
			if (!string.IsNullOrEmpty(token))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			}
		}

		private async Task<string> GetOrRefreshTokenAsync(CancellationToken cancellationToken)
		{
			const string cacheKey = "auth_token";

			if (_memoryCache.TryGetValue<string>(cacheKey, out var cachedToken) &&
				!string.IsNullOrEmpty(cachedToken))
			{
				_logger.LogDebug("Using cached authentication token");
				return cachedToken;
			}

			// Use semaphore to prevent multiple concurrent token requests
			var tokenLock = _tokenLocks.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1, 1));

			await tokenLock.WaitAsync(cancellationToken);
			try
			{
				// Double-check cache after acquiring lock
				if (_memoryCache.TryGetValue<string>(cacheKey, out cachedToken) &&
					!string.IsNullOrEmpty(cachedToken))
				{
					return cachedToken;
				}

				// Acquire new token
				var newToken = await AcquireNewTokenAsync(cancellationToken);
				if (!string.IsNullOrEmpty(newToken))
				{
					// Cache token with expiration slightly less than actual token expiry
					var cacheOptions = new MemoryCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = _authConfig.TokenExpiry - _tokenRefreshThreshold
					};
					_memoryCache.Set(cacheKey, newToken, cacheOptions);

					_logger.LogInformation("Successfully acquired and cached new authentication token");
					return newToken;
				}

				throw new InvalidOperationException("Failed to acquire authentication token");
			}
			finally
			{
				tokenLock.Release();
			}
		}

		private async Task<string> AcquireNewTokenAsync(CancellationToken cancellationToken)
		{
			try
			{
				// Implement your token acquisition logic here
				// This could be OAuth2 client credentials flow, JWT, etc.

				using var tokenRequest = new HttpRequestMessage(HttpMethod.Post, _authConfig.TokenEndpoint);
				tokenRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				var formData = new List<KeyValuePair<string, string>>
				{
					new("grant_type", "client_credentials"),
					new("client_id", _authConfig.ClientId),
					new("client_secret", _authConfig.ClientSecret),
					new("scope", _authConfig.Scopes)
				};

				tokenRequest.Content = new FormUrlEncodedContent(formData);

				using var response = await base.SendAsync(tokenRequest, cancellationToken);
				response.EnsureSuccessStatusCode();

				var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>(
					cancellationToken: cancellationToken);

				return tokenResponse?.AccessToken;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to acquire authentication token from {TokenEndpoint}",
					_authConfig.TokenEndpoint);
				throw;
			}
		}

		private void AddCorrelationId(HttpRequestMessage request)
		{
			var correlationId = Guid.NewGuid().ToString();
			request.Headers.Add("X-Correlation-ID", correlationId);
			request.Headers.Add("Request-Id", correlationId);
		}

		private void AddOptimizationHeaders(HttpRequestMessage request)
		{
			// Add compression support
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
			request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));

			// Add JSON accept header
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Add user agent for analytics
			request.Headers.TryAddWithoutValidation("User-Agent",
				"AuditManagement-Service/1.0");

			// Add caching headers for GET requests
			if (request.Method == HttpMethod.Get)
			{
				request.Headers.CacheControl = new CacheControlHeaderValue
				{
					MaxAge = TimeSpan.FromSeconds(30),
					// Only cache if explicitly allowed by server
				};
			}
		}

		private async Task<HttpResponseMessage> ExecuteWithRetryPolicyAsync(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			var retryCount = 0;
			var maxRetries = _authConfig.MaxRetries;

			while (true)
			{
				try
				{
					var response = await base.SendAsync(request, cancellationToken);

					// Only retry on server errors (5xx) or specific status codes
					if (!ShouldRetry(response.StatusCode) || retryCount >= maxRetries)
						return response;

					retryCount++;
					_logger.LogWarning("Request failed with status {StatusCode}. Retry attempt {RetryCount}/{MaxRetries}",
						response.StatusCode, retryCount, maxRetries);

					await Task.Delay(CalculateRetryDelay(retryCount), cancellationToken);

					// Clone the request for retry (HttpRequestMessage cannot be reused)
					request = await CloneHttpRequestMessageAsync(request);
				}
				catch (HttpRequestException ex) when (retryCount < maxRetries)
				{
					retryCount++;
					_logger.LogWarning(ex, "Request failed with exception. Retry attempt {RetryCount}/{MaxRetries}",
						retryCount, maxRetries);

					await Task.Delay(CalculateRetryDelay(retryCount), cancellationToken);

					// Clone the request for retry
					request = await CloneHttpRequestMessageAsync(request);
				}
			}
		}

		private bool ShouldRetry(HttpStatusCode statusCode)
		{
			return statusCode is HttpStatusCode.RequestTimeout or
				   HttpStatusCode.InternalServerError or
				   HttpStatusCode.BadGateway or
				   HttpStatusCode.ServiceUnavailable or
				   HttpStatusCode.GatewayTimeout;
		}

		private TimeSpan CalculateRetryDelay(int retryCount)
		{
			// Exponential backoff with jitter
			var exponentialDelay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
			var jitter = TimeSpan.FromMilliseconds(new Random().Next(0, 1000));
			return exponentialDelay + jitter;
		}

		private async Task<HttpRequestMessage> CloneHttpRequestMessageAsync(HttpRequestMessage original)
		{
			var clone = new HttpRequestMessage(original.Method, original.RequestUri);

			// Copy headers
			foreach (var header in original.Headers)
			{
				clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
			}

			// Copy content
			if (original.Content != null)
			{
				var contentBuffer = await original.Content.ReadAsByteArrayAsync();
				var contentStream = new MemoryStream(contentBuffer);

				clone.Content = new StreamContent(contentStream);

				// Copy content headers
				foreach (var header in original.Content.Headers)
				{
					clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
			}

			// Copy properties
			foreach (var prop in original.Properties)
			{
				clone.Properties.Add(prop);
			}

			return clone;
		}

		private static bool ShouldSkipAuth(Uri requestUri)
		{
			// Skip auth for token endpoint to avoid infinite recursion
			if (requestUri?.AbsoluteUri.Contains("token") == true)
				return true;

			// Add other endpoints that don't require auth
			var skipAuthPaths = new[] { "/health", "/metrics", "/status" };
			return skipAuthPaths.Any(path => requestUri?.AbsoluteUri.Contains(path) == true);
		}

		private void LogPerformanceMetrics(
			HttpRequestMessage request,
			HttpResponseMessage response,
			TimeSpan duration)
		{
			var logLevel = duration.TotalSeconds > 5 ? LogLevel.Warning : LogLevel.Debug;

			_logger.Log(logLevel,
				"HTTP {Method} {RequestUri} completed with status {StatusCode} in {DurationMs}ms",
				request.Method,
				request.RequestUri,
				(int)response.StatusCode,
				duration.TotalMilliseconds);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (var semaphore in _tokenLocks.Values)
				{
					semaphore.Dispose();
				}
				_tokenLocks.Clear();
			}

			base.Dispose(disposing);
		}
	}

	// Supporting classes
	public class AuthConfig
	{
		public string TokenEndpoint { get; set; } = string.Empty;
		public string ClientId { get; set; } = string.Empty;
		public string ClientSecret { get; set; } = string.Empty;
		public string Scopes { get; set; } = string.Empty;
		public TimeSpan TokenExpiry { get; set; } = TimeSpan.FromHours(1);
		public int MaxRetries { get; set; } = 3;
	}

	public class TokenResponse
	{
		public string AccessToken { get; set; } = string.Empty;
		public string TokenType { get; set; } = string.Empty;
		public int ExpiresIn { get; set; }
		public string Scope { get; set; } = string.Empty;
	}
}