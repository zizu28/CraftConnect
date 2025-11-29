namespace Core.SharedKernel.ValueObjects
{
	public record NotificationContext
	{
		public string Type { get; private set; } = string.Empty;
		public Guid RelatedId { get; private set; }

		private NotificationContext()
		{
			Type = string.Empty;
			RelatedId = Guid.Empty;
		}

		public NotificationContext(string type, Guid relatedId)
		{
			ArgumentNullException.ThrowIfNullOrEmpty(type);
			if(relatedId == Guid.Empty) throw new ArgumentNullException(nameof(relatedId));
			Type = type;
			RelatedId = relatedId;
		}
	}
}
