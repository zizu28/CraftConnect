using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PaymentManagement.Domain.Entities;

namespace PaymentManagement.Infrastructure.EntityTypeConfigurations
{
	public class RefundConfiguration : IEntityTypeConfiguration<Refund>
	{
		public void Configure(EntityTypeBuilder<Refund> builder)
		{
			throw new NotImplementedException();
		}
	}
}
