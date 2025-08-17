using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingManagement.Infrastructure.RepositoryImplementations
{
	public class BookingRepository(ApplicationDbContext dbContext) : IBookingRepository
	{
		public async Task AddAsync(Booking entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			await dbContext.Bookings.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var booking = await dbContext.Bookings.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"Booking with ID {id} not found.");
			dbContext.Bookings.Remove(booking);
		}

		public async Task<Booking> FindBy(Expression<Func<Booking, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			var booking = await dbContext.Bookings.FirstOrDefaultAsync(predicate, cancellationToken);
			return booking!;
		}

		public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return (await dbContext.Bookings
				//.Include(b => b.LineItems)
				.AsNoTracking()
				.ToListAsync(cancellationToken));
		}

		public async Task<Booking> GetBookingByDetails(string details, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(details);
			var booking = await dbContext.Bookings
				.Include(b => b.LineItems)
				.AsNoTracking()
				.FirstOrDefaultAsync(b => b.Details.Description.Contains(details), cancellationToken);
			return booking!;
		}

		public async Task<Booking> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var booking = await FindBy(b => b.Id == id, cancellationToken);
			return booking;
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public Task UpdateAsync(Booking entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			dbContext.Bookings.Update(entity);
			return Task.CompletedTask;
		}
	}
}
