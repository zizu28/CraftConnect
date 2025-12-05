using BookingManagement.Application.Contracts;
using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingManagement.Infrastructure.RepositoryImplementations
{
	public class CustomerProjectRepository(ApplicationDbContext context) : Repository<CustomerProject>(context), ICustomerProjectRepository
	{
		public async Task<IEnumerable<CustomerProject>> GetProjectsByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
		{
			return await _context.CustomerProjects
				.Where(p => p.CustomerId == customerId)
				.OrderByDescending(p => p.Timeline.Start)
				.ToListAsync(cancellationToken);
		}

		public async Task<CustomerProject?> GetProjectWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await _context.CustomerProjects
				.Include(p => p.Skills)
				.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
		}

		public async Task<IEnumerable<CustomerProject>> GetProjectsByStatusAsync(ServiceRequestStatus status, CancellationToken cancellationToken = default)
		{
			return await _context.CustomerProjects
				.Where(p => p.Status == status)
				.ToListAsync(cancellationToken);
		}

		public async Task<IEnumerable<CustomerProject>> SearchOpenProjectsAsync(
			string? searchTerm,
			List<string>? skills,
			int pageNumber,
			int pageSize,
			CancellationToken cancellationToken = default)
		{
			var query = _context.CustomerProjects
				.Where(p => p.Status == ServiceRequestStatus.Open);

			if (!string.IsNullOrWhiteSpace(searchTerm))
			{
				query = query.Where(p => p.Title.Contains(searchTerm) || p.Description.Contains(searchTerm));
			}

			if (skills != null && skills.Count > 0)
			{
				query = query.Where(p => p.Skills.Any(s => skills.Contains(s.Name)));
			}

			return await query
				.OrderByDescending(p => p.Timeline.Start)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync(cancellationToken);
		}
	}
}