using Core.SharedKernel.ValueObjects;
using System.Linq.Expressions;
using UserManagement.Domain.Entities;

namespace UserManagement.Application.Contracts
{
	public interface ICustomerRepository : IRepository<Customer>
	{
		Task<Customer> GetCustomerByAddressAsync(Expression<Func<UserAddress, bool>> predicate, CancellationToken cancellationToken = default);
	}
}
