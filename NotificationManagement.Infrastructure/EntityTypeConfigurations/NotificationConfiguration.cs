using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Infrastructure.EntityTypeConfigurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
	public void Configure(EntityTypeBuilder<Notification> builder)
	{
		builder.ToTable("Notifications");

		builder.HasKey(n => n.Id);

		builder.Property(n => n.Id)
			.ValueGeneratedNever();

		builder.Property(n => n.RecipientId)
			.IsRequired();

		builder.Property(n => n.RecipientEmail)
			.IsRequired()
			.HasMaxLength(256);

		builder.Property(n => n.RecipientPhone)
			.HasMaxLength(20);

		builder.Property(n => n.Type)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(n => n.Channel)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(n => n.Priority)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(n => n.Status)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(n => n.Subject)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(n => n.Body)
			.IsRequired();

		builder.Property(n => n.Metadata)
			.HasColumnType("jsonb");

		builder.Property(n => n.ErrorMessage)
			.HasMaxLength(1000);

		builder.Property(n => n.CreatedAt)
			.IsRequired();

		builder.Property(n => n.RowVersion)
			.IsRowVersion();

		// Owned collection: NotificationLog
		builder.OwnsMany(n => n.Logs, log =>
		{
			log.ToTable("NotificationLogs");
			log.WithOwner().HasForeignKey("NotificationId");
			log.HasKey("Id");
			log.Property(l => l.Id).ValueGeneratedNever();
			log.Property(l => l.Status).HasConversion<string>();
			log.Property(l => l.Message).IsRequired().HasMaxLength(1000);
		});

		// Owned collection: NotificationAttachment
		builder.OwnsMany(n => n.Attachments, attachment =>
		{
			attachment.ToTable("NotificationAttachments");
			attachment.WithOwner().HasForeignKey("NotificationId");
			attachment.HasKey("Id");
			attachment.Property(a => a.Id).ValueGeneratedNever();
			attachment.Property(a => a.FileName).IsRequired().HasMaxLength(255);
			attachment.Property(a => a.ContentType).IsRequired().HasMaxLength(100);
		});

		// Indexes for common queries
		builder.HasIndex(n => n.RecipientId);
		builder.HasIndex(n  => n.Status);
		builder.HasIndex(n => n.CreatedAt);
		builder.HasIndex(n => new { n.Status, n.ScheduledFor });
	}
}
