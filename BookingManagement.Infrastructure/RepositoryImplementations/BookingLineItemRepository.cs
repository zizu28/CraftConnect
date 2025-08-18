using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BookingManagement.Infrastructure.RepositoryImplementations
{
	public class BookingLineItemRepository(ApplicationDbContext dbContext) : IBookingLineItemRepository
	{
		public async Task AddAsync(BookingLineItem entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "BookingLineItem cannot be null.");
			}
			await dbContext.BookingLineItems.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await dbContext.BookingLineItems.FindAsync(new object[] { id }, cancellationToken);
			if (entity == null)
			{
				throw new KeyNotFoundException($"BookingLineItem with ID {id} not found.");
			}
			dbContext.BookingLineItems.Remove(entity);
		}

		public async Task<BookingLineItem> FindBy(Expression<Func<BookingLineItem, bool>> predicate, CancellationToken cancellationToken = default)
		{
			if (predicate == null)
			{
				throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null.");
			}
			var bookingLineItem = await dbContext.BookingLineItems.FirstOrDefaultAsync(predicate, cancellationToken)
				?? throw new KeyNotFoundException("No BookingLineItem found matching the provided criteria.");
			return bookingLineItem;
		}

		public async Task<IEnumerable<BookingLineItem>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.BookingLineItems.AsNoTracking().ToListAsync(cancellationToken);
		}

		public async Task<BookingLineItem> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var entity = await dbContext.BookingLineItems.FindAsync([id], cancellationToken)
				?? throw new KeyNotFoundException($"BookingLineItem with ID {id} not found.");
			return entity;
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public Task UpdateAsync(BookingLineItem entity, CancellationToken cancellationToken = default)
		{
			if (entity == null)
			{
				throw new ArgumentNullException(nameof(entity), "BookingLineItem cannot be null.");
			}
			dbContext.Entry(entity).State = EntityState.Detached;
			dbContext.BookingLineItems.Update(entity);
			return Task.CompletedTask;
		}
	}
}
