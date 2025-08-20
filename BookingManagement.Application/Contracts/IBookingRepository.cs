using BookingManagement.Domain.Entities;
using System.Linq.Expressions;

namespace BookingManagement.Application.Contracts
{
	public interface IBookingRepository : IRepository<Booking>
	{
		Task<bool> ExistsAsync(Guid bookingId, CancellationToken cancellationToken);
		Task<Booking> GetBookingByDetails(string details, CancellationToken cancellationToken = default);
	}
}
