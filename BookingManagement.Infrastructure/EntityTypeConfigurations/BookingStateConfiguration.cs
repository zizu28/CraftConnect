using BookingManagement.Application.SAGA;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Infrastructure.EntityTypeConfigurations
{
	public class BookingStateConfiguration : IEntityTypeConfiguration<BookingToPaymentState>
	{
		public void Configure(EntityTypeBuilder<BookingToPaymentState> builder)
		{
			builder.ToTable("BookingToPaymentState", "saga");

			// Primary key
			builder.HasKey(x => x.CorrelationId);

			// Indexes for queries
			builder.HasIndex(x => x.BookingId);
			builder.HasIndex(x => x.PaymentId);
			builder.HasIndex(x => x.CurrentState);
			builder.HasIndex(x => x.CreatedAt);

			// Concurrency
			builder.Property(x => x.RowVersion)
				.IsRowVersion();

			// Required fields
			builder.Property(x => x.CurrentState)
				.IsRequired()
				.HasMaxLength(64);

			builder.Property(x => x.CustomerEmail)
				.IsRequired()
				.HasMaxLength(256);

			builder.Property(x => x.Currency)
				.IsRequired()
				.HasMaxLength(3);

			builder.Property(x => x.ServiceDescription)
				.HasMaxLength(500);

			// Optional fields
			builder.Property(x => x.PaymentReference)
				.HasMaxLength(100);

			builder.Property(x => x.PaymentAuthorizationUrl)
				.HasMaxLength(500);

			builder.Property(x => x.FailureReason)
				.HasMaxLength(1000);

			builder.Property(x => x.Metadata)
				.HasColumnType("nvarchar(max)");

			// Decimal precision
			builder.Property(x => x.Amount)
				.HasPrecision(18, 2);
		}
	}
}
