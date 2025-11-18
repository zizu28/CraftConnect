using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using System.Text.Json.Serialization;

namespace ReportingAnalyticsManagement.Domain.Entities
{
	public class AnalyticsSnapshot : AggregateRoot
	{
		public DateTime Timestamp { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public Period Period { get; private set; }
		public int TotalUsers { get; private set; }
		public int NewSignups { get; private set; }
		public int ActiveUsers { get; private set; }
		public int NewProjects { get; private set; }
		public Money TotalRevenue { get; private set; } = new(0, "");
		public UserBreakdown UserBreakdown { get; private set; } = new(0, 0);
		public ProjectActivity ProjectActivity { get; private set; } = new(0, 0);

		private AnalyticsSnapshot() { }

		public static AnalyticsSnapshot Generate(DateTime timeStamp, Period period, int totalUsers,
			int newSignups, int activeUsers, int newProjects, Money totalRevenue, int craftsmanCount,
			int customerCount, int completedProjects)
		{
			return new AnalyticsSnapshot
			{
				Timestamp = timeStamp,
				Period = period,
				TotalUsers = totalUsers,
				NewSignups = newSignups,
				ActiveUsers = activeUsers,
				TotalRevenue = totalRevenue,
				UserBreakdown = new(craftsmanCount, customerCount),
				ProjectActivity = new(newProjects, completedProjects)
			};
		}
	}
}
