using Core.SharedKernel.ValueObjects;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UserManagement.Application.Contracts;
using UserManagement.Domain.Entities;

namespace UserManagement.Infrastructure.RepositoryImplementations
{
	public class CustomerRepository(ApplicationDbContext dbContext) : ICustomerRepository
	{
		public async Task AddAsync(Customer entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			await dbContext.Customers.AddAsync(entity, cancellationToken);
		}

		public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var customer = await dbContext.Customers.FindAsync([id], cancellationToken) 
				?? throw new KeyNotFoundException($"Customer with ID {id} not found.");
			dbContext.Customers.Remove(customer);
		}

		public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
		{
			return await dbContext.Customers.AnyAsync(c => c.Id == id, cancellationToken);
		}

		public async Task<Customer> FindBy(Expression<Func<Customer, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			var customer = await dbContext.Customers.FirstOrDefaultAsync(predicate, cancellationToken);
			return customer!;
		}

		public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
		{
			return await dbContext.Customers.AsNoTracking().ToListAsync(cancellationToken);
		}

		public Task<Customer> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(email);
			return FindBy(c => c.Email.Address == email, cancellationToken);
		}

		public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
		{
			var customer = await FindBy(c => c.Id == id, cancellationToken);
			return customer;
		}

		public async Task<Customer> GetCustomerByAddressAsync(Expression<Func<UserAddress, bool>> predicate, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			var customer = await dbContext.Customers
				.AsNoTracking()
				.FirstOrDefaultAsync(c => c.Address!.City != null &&
					c.Address.PostalCode != null &&
					c.Address.Street != null, cancellationToken);
			return customer ?? throw new KeyNotFoundException("Customer with specified address not found.");
		}

		public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			await dbContext.SaveChangesAsync(cancellationToken);
		}

		public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(entity);
			dbContext.Customers.Update(entity);
		}
	}
}
