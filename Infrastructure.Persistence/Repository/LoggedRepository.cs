using Core.Logging;
using Core.SharedKernel.Contracts;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repository
{
	public abstract class LoggedRepository<T>(
		ApplicationDbContext dbContext, 
		ILoggingService<LoggedRepository<T>> logger) : BaseRepository<T>(dbContext) where T : class
	{
		private readonly ILoggingService<LoggedRepository<T>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

		public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				_logger.LogInformation("Getting {EntityType} by ID: {Id}", typeof(T).Name, id);
				var result = await base.GetByIdAsync(id, cancellationToken);
				
				stopwatch.Stop();
				_logger.LogInformation("Retrieved {EntityType} by ID: {Id} in {ElapsedMs}ms", 
					typeof(T).Name, id, stopwatch.ElapsedMilliseconds);
				
				return result;
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				_logger.LogError(ex, "Error getting {EntityType} by ID: {Id} after {ElapsedMs}ms", 
					typeof(T).Name, id, stopwatch.ElapsedMilliseconds);
				throw;
			}
		}

		public override async Task AddAsync(T entity, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				_logger.LogInformation("Adding new {EntityType}", typeof(T).Name);
				await base.AddAsync(entity, cancellationToken);
				
				stopwatch.Stop();
				_logger.LogInformation("Successfully added {EntityType} in {ElapsedMs}ms", 
					typeof(T).Name, stopwatch.ElapsedMilliseconds);
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				_logger.LogError(ex, "Error adding {EntityType} after {ElapsedMs}ms", 
					typeof(T).Name, stopwatch.ElapsedMilliseconds);
				throw;
			}
		}

		public override async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				_logger.LogInformation("Updating {EntityType}", typeof(T).Name);
				await base.UpdateAsync(entity, cancellationToken);
				
				stopwatch.Stop();
				_logger.LogInformation("Successfully updated {EntityType} in {ElapsedMs}ms", 
					typeof(T).Name, stopwatch.ElapsedMilliseconds);
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				_logger.LogError(ex, "Error updating {EntityType} after {ElapsedMs}ms", 
					typeof(T).Name, stopwatch.ElapsedMilliseconds);
				throw;
			}
		}

		public override async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var stopwatch = Stopwatch.StartNew();
			try
			{
				_logger.LogInformation("Deleting {EntityType} with ID: {Id}", typeof(T).Name, id);
				await base.DeleteAsync(id, cancellationToken);
				
				stopwatch.Stop();
				_logger.LogInformation("Successfully deleted {EntityType} with ID: {Id} in {ElapsedMs}ms", 
					typeof(T).Name, id, stopwatch.ElapsedMilliseconds);
			}
			catch (Exception ex)
			{
				stopwatch.Stop();
				_logger.LogError(ex, "Error deleting {EntityType} with ID: {Id} after {ElapsedMs}ms", 
					typeof(T).Name, id, stopwatch.ElapsedMilliseconds);
				throw;
			}
		}
	}
}