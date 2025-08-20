using Hangfire;
using System.Linq.Expressions;

namespace Infrastructure.BackgroundJobs
{
	public class HangfireJobService(IBackgroundJobClient jobClient, IRecurringJobManager recurringJob) : IBackgroundJobService
	{
		public string ContinueWith<T>(string parentJobId, string continuationJobName, Expression<Func<T, Task>> methodCall)
		{
			return jobClient.ContinueJobWith(parentJobId, methodCall);
		}

		public bool Delete(string jobId)
		{
			return jobClient.Delete(jobId);
		}

		public string Enqueue<T>(string jobName, Expression<Func<T, Task>> methodCall)
		{
			return jobClient.Enqueue(jobName, methodCall);
		}

		public void Recurring<T>(string jobName, Expression<Func<T, Task>> methodCall, string cronExpression)
		{
			recurringJob.AddOrUpdate(jobName, methodCall, cronExpression);
		}

		public string Schedule<T>(string jobName, Expression<Func<T, Task>> methodCall, TimeSpan delay)
		{
			return jobClient.Schedule(jobName, methodCall, delay);
		}
	}
}
