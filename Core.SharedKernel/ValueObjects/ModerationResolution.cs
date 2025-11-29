using Core.SharedKernel.Enums;

namespace Core.SharedKernel.ValueObjects
{
	public record ModerationResolution
	{
		public Guid ModeratorId { get; private set; }
		public ModerationAction ActionTaken { get; private set; }
		public string Notes { get; private set; } = string.Empty;
		public DateTime Timestamp { get; private set; }

		private ModerationResolution()
		{
			ModeratorId = Guid.Empty;
			Notes = string.Empty;
			Timestamp = DateTime.MinValue;
		}

		public ModerationResolution(Guid moderatorId, ModerationAction actionTaken, string notes,
			DateTime timestamp)
		{
			ArgumentNullException.ThrowIfNullOrEmpty(notes);
			if(moderatorId == Guid.Empty) throw new ArgumentNullException(nameof(moderatorId));
			if(timestamp == DateTime.MinValue) throw new ArgumentNullException(nameof(timestamp));
			ModeratorId = moderatorId;
			ActionTaken = actionTaken;
			Notes = notes;
			Timestamp = timestamp;
		}
	}
}
