using BookingManagement.Domain.Entities;
using Core.SharedKernel.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingManagement.Infrastructure.EntityTypeConfigurations
{
	public class CraftsmanProposalConfiguration : IEntityTypeConfiguration<CraftsmanProposal>
	{
		public void Configure(EntityTypeBuilder<CraftsmanProposal> builder)
		{
			builder.HasKey(x => x.Id);

			builder.HasIndex(x => x.ProjectId);

			builder.HasIndex(x => x.CraftsmanId);

			builder.Property(x => x.CoverLetter)
				.HasMaxLength(2000)
				.IsRequired();

			builder.Property(x => x.Status)
				.HasConversion(c => c.ToString(), c => Enum.Parse<ProposalStatus>(c, true))
				.IsRequired();

			builder.OwnsOne(x => x.Price, money =>
			{
				money.Property(m => m.Amount)
					.HasColumnName("PriceAmount")
					.HasColumnType("decimal(18,2)")
					.IsRequired();

				money.Property(m => m.Currency)
					.HasColumnName("PriceCurrency")
					.HasMaxLength(3)
					.IsRequired();
			});

			builder.OwnsOne(x => x.ProposedTimeline, timeline =>
			{
				timeline.Property(t => t.Start)
					.HasColumnName("StartDate")
					.IsRequired();

				timeline.Property(t => t.End)
					.HasColumnName("EndDate")
					.IsRequired();
			});

			builder.Ignore(x => x.DomainEvents);
		}
	}
}