using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Exceptions;

namespace UserManagement.Presentation
{
	public class GlobalExceptionHandler : IExceptionHandler
	{
		public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
		{
			httpContext.Response.StatusCode = (int)StatusCodes.Status500InternalServerError;
			httpContext.Response.ContentType = "application/json";

			var contextFeatures = httpContext.Features.Get<IExceptionHandlerFeature>();
			if (contextFeatures != null)
			{
				httpContext.Response.StatusCode = contextFeatures.Error switch
				{
					NotFoundException => (int)StatusCodes.Status404NotFound,
					BadRequestException => (int)StatusCodes.Status400BadRequest,
					_ => (int)StatusCodes.Status500InternalServerError
				};
				await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
				{
					Status = httpContext.Response.StatusCode,
					Title = contextFeatures.Error.Message,
					Detail = contextFeatures.Error.StackTrace,
					Type = contextFeatures.Error.GetType().Name,
				}, cancellationToken);
			}
			return true;
		}
	}
}
