using Infrastructure.Persistence.Data;
using Infrastructure.Persistence.Repository;
using PaymentManagement.Application.Contracts;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Infrastructure.RepositoryImplementations
{
	public class PaymentRepository(ApplicationDbContext dbContext) : BaseRepository<Payment>(dbContext), IPaymentRepository { }
}
