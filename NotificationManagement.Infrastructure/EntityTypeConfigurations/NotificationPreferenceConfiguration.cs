using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Infrastructure.EntityTypeConfigurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
	public void Configure(EntityTypeBuilder<NotificationPreference> builder)
	{
		builder.ToTable("NotificationPreferences");

		builder.HasKey(p => p.Id);

		builder.Property(p => p.Id)
			.ValueGeneratedNever();

		builder.Property(p => p.UserId)
			.IsRequired();

		builder.Property(p => p.NotificationType)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(p => p.Frequency)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(p => p.CreatedAt)
			.IsRequired();

		builder.Property(p => p.RowVersion)
			.IsRowVersion();

		// Unique constraint: one preference per user per notification type
		builder.HasIndex(p => new { p.UserId, p.NotificationType })
			.IsUnique();

		// Index for user lookup
		builder.HasIndex(p => p.UserId);
	}
}
