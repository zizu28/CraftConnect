using BookingManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace BookingManagement.Infrastructure.EntityTypeConfigurations
{
	public class CustomerProjectConfiguration : IEntityTypeConfiguration<CustomerProject>
	{
		public void Configure(EntityTypeBuilder<CustomerProject> builder)
		{
			builder.HasKey(x => x.Id);

			builder.HasIndex(x => x.CustomerId);
			builder.HasIndex(x => x.SelectedCraftsmanId);

			builder.Property(x => x.Title)
				.HasMaxLength(200)
				.IsRequired();

			builder.Property(x => x.Description)
				.HasMaxLength(4000)
				.IsRequired();

			builder.Property(x => x.Status)
				.HasConversion<string>()
				.IsRequired();

			builder.OwnsOne(x => x.Budget, money =>
			{
				money.Property(m => m.Amount)
					.HasColumnName("BudgetAmount")
					.HasColumnType("decimal(18,2)")
					.IsRequired();

				money.Property(m => m.Currency)
					.HasColumnName("BudgetCurrency")
					.HasMaxLength(3)
					.IsRequired();
			});

			builder.OwnsOne(x => x.Timeline, timeline =>
			{
				timeline.Property(t => t.Start)
					.HasColumnName("StartDate");

				timeline.Property(t => t.End)
					.HasColumnName("EndDate");
			});

			builder.OwnsMany(x => x.Skills, skill =>
			{
				skill.ToTable("ProjectSkills");
				skill.WithOwner().HasForeignKey("ProjectId");

				skill.Property(s => s.Name).HasMaxLength(100).IsRequired();
				skill.Property(s => s.YearsOfExperience);
			});

			builder.Property(x => x.MilestoneIds)
				.HasConversion(
					v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
					v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()
				);

			builder.Property(x => x.AttachmentIds)
				.HasConversion(
					v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
					v => JsonSerializer.Deserialize<List<Guid>>(v, (JsonSerializerOptions?)null) ?? new List<Guid>()
				);

			builder.Ignore(x => x.DomainEvents);
		}
	}
}