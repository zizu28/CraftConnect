using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums; // For ServiceRequestStatus

namespace BookingManagement.Application.Contracts
{
	public interface ICustomerProjectRepository : IRepository<CustomerProject>
	{
		Task<IEnumerable<CustomerProject>> GetProjectsByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
		Task<IEnumerable<CustomerProject>> SearchOpenProjectsAsync(
			string? searchTerm,
			List<string>? skills,
			int pageNumber,
			int pageSize,
			CancellationToken cancellationToken = default);
		Task<IEnumerable<CustomerProject>> GetProjectsByStatusAsync(ServiceRequestStatus status, CancellationToken cancellationToken = default);
		Task<CustomerProject?> GetProjectWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
	}
}