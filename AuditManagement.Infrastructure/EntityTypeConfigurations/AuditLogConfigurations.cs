using AuditManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Net;

namespace AuditManagement.Infrastructure.EntityTypeConfigurations
{
	public class AuditLogConfigurations : IEntityTypeConfiguration<AuditLog>
	{
		public void Configure(EntityTypeBuilder<AuditLog> builder)
		{
			builder.HasKey(al => al.Id);
			builder.Property(al => al.Id).IsRequired();
			builder.Property(al => al.UserId).IsRequired();

			// EntityId configuration
			builder.Property(al => al.EntityId)
				.IsRequired(false); // Allow null for non-entity specific events

			// Timestamp configuration
			builder.Property(al => al.Timestamp)
				.IsRequired()
				.HasColumnType("datetime2")
				.HasPrecision(0) // No fractional seconds for audit precision
				.HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

			// EventType configuration
			builder.Property(al => al.EventType)
				.IsRequired()
				.HasMaxLength(100) // Reasonable length for event types
				.IsUnicode(false); // ASCII for better performance

			// Details configuration
			builder.Property(al => al.Details)
				.IsRequired()
				.HasMaxLength(4000) // Allow substantial detail text
				.IsUnicode(true); // Support international characters

			// IPAddress configuration with value converter
			builder.Property(al => al.IpAddress)
				.IsRequired()
				.HasConversion(
					ip => ip.ToString(), // Convert to string for storage
					ipString => IPAddress.Parse(ipString) // Convert back to IPAddress
				)
				.HasMaxLength(45) // IPv6 max length (including IPv4-mapped IPv6)
				.IsUnicode(false);

			// OldValue configuration
			builder.Property(al => al.OldValue)
				.IsRequired()
				.HasMaxLength(4000) // Match Details length
				.IsUnicode(true)
				.HasDefaultValue(string.Empty);

			// NewValue configuration
			builder.Property(al => al.NewValue)
				.IsRequired()
				.HasMaxLength(4000) // Match Details length
				.IsUnicode(true)
				.HasDefaultValue(string.Empty);

			// Indexes for query performance
			builder.HasIndex(al => al.UserId).IsUnique(false); // Non-unique since users have multiple logs

			builder.HasIndex(al => al.EntityId).IsUnique(false);

			builder.HasIndex(al => al.EventType).IsUnique(false);

			// Query filter for soft delete (if implemented in base Entity class)
			// builder.HasQueryFilter(al => !al.IsDeleted);

			// Configure owned types (if any exist in the future)
			// builder.OwnsOne(al => al.SomeOwnedType);

			// Configure relationships (if AuditLog has navigation properties)
			// builder.HasOne<SomeOtherEntity>()
			//     .WithMany()
			//     .HasForeignKey(al => al.SomeForeignKey);
		}
	}
}