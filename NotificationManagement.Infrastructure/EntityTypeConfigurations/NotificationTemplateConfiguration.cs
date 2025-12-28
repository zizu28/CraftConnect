using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationManagement.Domain.Entities;

namespace NotificationManagement.Infrastructure.EntityTypeConfigurations;

public class NotificationTemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
{
	public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
	{
		builder.ToTable("NotificationTemplates");

		builder.HasKey(t => t.Id);

		builder.Property(t => t.Id)
			.ValueGeneratedNever();

		builder.Property(t => t.Name)
			.IsRequired()
			.HasMaxLength(100);

		builder.Property(t => t.Code)
			.IsRequired()
			.HasMaxLength(50);

		builder.Property(t => t.Type)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(t => t.Channel)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(t => t.SubjectTemplate)
			.IsRequired()
			.HasMaxLength(200);

		builder.Property(t => t.BodyTemplate)
			.IsRequired();

		builder.Property(t => t.Description)
			.HasMaxLength(500);

		builder.Property(t => t.DefaultPriority)
			.IsRequired()
			.HasConversion<string>();

		builder.Property(t => t.RequiredVariables)
			.HasColumnType("jsonb");

		builder.Property(t => t.CreatedAt)
			.IsRequired();

		builder.Property(t => t.RowVersion)
			.IsRowVersion();

		// Unique constraint on Code
		builder.HasIndex(t => t.Code)
			.IsUnique();

		// Index for active templates
		builder.HasIndex(t => t.IsActive);
	}
}
