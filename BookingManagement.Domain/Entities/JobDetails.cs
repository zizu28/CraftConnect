using Core.SharedKernel.Domain;

namespace BookingManagement.Domain.Entities
{
	public class JobDetails : Entity
	{
		public Guid BookingId { get; private set; }
		public string Description { get; private set; }

		private JobDetails()
		{
		}
		internal JobDetails(Guid bookingId, string description)
		{
			Id = Guid.NewGuid();
			BookingId = bookingId;
			Description = description;
		}

		internal void UpdateDescription(string description)
		{
			Description = description;
		}
	}
}
