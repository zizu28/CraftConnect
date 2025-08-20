using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace Infrastructure.BackgroundJobs
{
	public class HangfireAuthFilter : IDashboardAuthorizationFilter
	{
		public bool Authorize([NotNull] DashboardContext context)
		{
			return context.GetHttpContext().User.IsInRole("Admin");
		}
	}
}
