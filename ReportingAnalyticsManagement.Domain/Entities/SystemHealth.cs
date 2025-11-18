using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using System.Text.Json.Serialization;

namespace ReportingAnalyticsManagement.Domain.Entities
{
	public class SystemHealth : AggregateRoot
	{
		public string ServerUptime { get; private set; } = string.Empty;
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public DbConnectionStatus DbConnectionStatus { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public MessageQueueStatus MessageQueueStatus { get; private set; }
		public int ApiAvgResponseTimeMs { get; private set; }
		public int BackgroundJobsPending { get; private set; }
		public int BackgroundJobsFailed { get; private set; }
		public DateTime LastChecked { get; private set; }

		private SystemHealth() { }

		public void UpdateDbStatus(DbConnectionStatus newStatus, int responseTime)
		{
			DbConnectionStatus = newStatus;
			ApiAvgResponseTimeMs = responseTime;
		}

		public void UpdateApiStatus(int responseTime)
		{
			ApiAvgResponseTimeMs = responseTime;
		}

		public void UpdateJobStatus(int pending, int failed)
		{
			BackgroundJobsPending = pending;
			BackgroundJobsFailed = failed;
		}
	}
}
