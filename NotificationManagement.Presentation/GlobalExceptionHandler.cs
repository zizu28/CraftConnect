using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace NotificationManagement.Presentation;

public class GlobalExceptionHandler : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
	{
		httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
		httpContext.Response.ContentType = "application/json";

		var contextFeatures = httpContext.Features.Get<IExceptionHandlerFeature>();
		if (contextFeatures != null)
		{
			httpContext.Response.StatusCode = contextFeatures.Error switch
			{
				// Validation errors
				ValidationException => StatusCodes.Status400BadRequest,
				
				// Bad request errors
				ArgumentNullException or ArgumentException => StatusCodes.Status400BadRequest,
				
				// Not found errors
				KeyNotFoundException => StatusCodes.Status404NotFound,
				
				// Authorization errors
				UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
				
				// Conflict/Business logic errors
				InvalidOperationException => StatusCodes.Status409Conflict,
				
				// Not implemented
				NotImplementedException => StatusCodes.Status501NotImplemented,
				
				// Timeout
				TimeoutException => StatusCodes.Status408RequestTimeout,
				
				// Default to 500
				_ => StatusCodes.Status500InternalServerError
			};

			await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
			{
				Status = httpContext.Response.StatusCode,
				Title = GetErrorTitle(httpContext.Response.StatusCode),
				Detail = contextFeatures.Error.Message,
				Type = contextFeatures.Error.GetType().Name,
				Instance = httpContext.Request.Path
			}, cancellationToken);
		}

		return true;
	}

	private static string GetErrorTitle(int statusCode) => statusCode switch
	{
		400 => "Bad Request",
		401 => "Unauthorized",
		404 => "Not Found",
		408 => "Request Timeout",
		409 => "Conflict",
		500 => "Internal Server Error",
		501 => "Not Implemented",
		_ => "Error"
	};
}
