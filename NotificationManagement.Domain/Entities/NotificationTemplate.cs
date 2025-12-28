using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NotificationManagement.Domain.Entities
{
	public class NotificationTemplate : AggregateRoot
	{
		public string Name { get; private set; }
		public string Code { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationType Type { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationChannel Channel { get; private set; }

		public string SubjectTemplate { get; private set; }
		public string BodyTemplate { get; private set; }
		public string? Description { get; private set; }

		public bool IsActive { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public NotificationPriority DefaultPriority { get; private set; }
		public int DefaultMaxRetries { get; private set; }

		public List<string> RequiredVariables { get; private set; } = [];
		public int Version { get; private set; }

		public DateTime CreatedAt { get; private set; }
		public Guid CreatedBy { get; private set; }
		public DateTime? UpdatedAt { get; private set; }

		[Timestamp]
		public byte[] RowVersion { get; private set; }

		private NotificationTemplate() { }

		// BEHAVIORS

		public static NotificationTemplate Create(
			string name,
			string code,
			NotificationType type,
			NotificationChannel channel,
			string subjectTemplate,
			string bodyTemplate,
			List<string> requiredVariables,
			Guid createdBy)
		{
			return new NotificationTemplate
			{
				Id = Guid.NewGuid(),
				Name = name,
				Code = code.ToUpperInvariant(),
				Type = type,
				Channel = channel,
				SubjectTemplate = subjectTemplate,
				BodyTemplate = bodyTemplate,
				RequiredVariables = requiredVariables,
				IsActive = true,
				DefaultPriority = NotificationPriority.Normal,
				DefaultMaxRetries = 3,
				Version = 1,
				CreatedAt = DateTime.UtcNow,
				CreatedBy = createdBy
			};
		}

		public void UpdateContent(string subjectTemplate, string bodyTemplate)
		{
			SubjectTemplate = subjectTemplate;
			BodyTemplate = bodyTemplate;
			Version++;
			UpdatedAt = DateTime.UtcNow;
		}

		public void Activate() => IsActive = true;
		public void Deactivate() => IsActive = false;

		public (string subject, string body) Render(Dictionary<string, string> variables)
		{
			var subject = SubjectTemplate;
			var body = BodyTemplate;

			foreach (var (key, value) in variables)
			{
				subject = subject.Replace($"{{{{{key}}}}}", value);
				body = body.Replace($"{{{{{key}}}}}", value);
			}

			return (subject, body);
		}
	}
}
