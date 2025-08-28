using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents;
using Core.SharedKernel.ValueObjects;
using NodaTime;
using System.Text.Json.Serialization;

namespace BookingManagement.Domain.Entities
{
	public class Booking : AggregateRoot
	{
		public Guid CraftmanId { get; private set; }
		public Guid CustomerId { get; private set; }
		[JsonConverter(typeof(JsonStringEnumConverter))]
		public BookingStatus Status { get; private set; }
		public Address ServiceAddress { get; private set; }
		public JobDetails Details { get; private set; }
		public uint RowVersion { get; private set; }
		public LocalDateTime StartDate { get; private set; }
		public LocalDateTime EndDate { get; private set; }
		public LocalTime CreatedAt { get; private set; } = new LocalTime(
					DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second, DateTime.UtcNow.Millisecond);
		public List<BookingLineItem> LineItems { get; private set; } = [];

		private Booking()
		{
			
		}

		public static Booking Create(Guid customerId, Guid craftmanId, 
			Address address, LocalDateTime startDate, LocalDateTime endDate)
		{
			if(customerId == Guid.Empty || craftmanId == Guid.Empty || address == null)
			{
				throw new ArgumentException("Invalid booking parameters.");
			}
			var booking = new Booking()
			{
				Id = Guid.NewGuid(),
				CustomerId = customerId,
				CraftmanId = craftmanId,
				ServiceAddress = address,
				Status = BookingStatus.Pending,
				StartDate = startDate,
				EndDate = endDate
			};
			return booking;
		}

		public void AddLineItem(string description, decimal price, int quantity = 1)
		{
			if(Status != BookingStatus.Pending)
			{
				throw new InvalidOperationException("Cannot add line items to a booking that is not in a pending state.");
			}
			if(string.IsNullOrWhiteSpace(description) || price <= 0 || quantity <= 0)
			{
				throw new ArgumentException("Invalid line item parameters.");
			}
			var lineItem = new BookingLineItem(Id, description, price, quantity);
			LineItems.Add(lineItem);
		}

		public void AddJobDetails(string description)
		{
			if(Status != BookingStatus.Pending)
			{
				throw new InvalidOperationException("Cannot add job details to a booking that is not in a pending state.");
			}
			if(string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentException("Job description cannot be empty.");
			}
			Details ??= new JobDetails(Id, description);
		}

		public void ConfirmBooking()
		{
			if(Status != BookingStatus.Pending)
			{
				throw new InvalidOperationException("Booking can only be confirmed if it is in a pending state.");
			}
			if (LineItems.Count == 0)
			{
				throw new InvalidOperationException("Cannot confirm booking without line items.");
			}
			Status = BookingStatus.Confirmed;
		}

		public void CompleteBooking()
		{
			if(Status != BookingStatus.Confirmed)
			{
				throw new InvalidOperationException("Booking can only be completed if it is confirmed.");
			}
			Status = BookingStatus.Completed;
		}

		public void CancelBooking(CancellationReason reason)
		{
			if(Status == BookingStatus.Completed || Status == BookingStatus.Cancelled)
			{
				throw new InvalidOperationException("Cannot cancel a booking that is already completed or cancelled.");
			}
			Status = BookingStatus.Cancelled;
			AddIntegrationEvent(new BookingCancelledIntegrationEvent(Id, reason));
		}


		public void UpdateJobDetails(string newDescription)
		{
			if(Status == BookingStatus.Cancelled || Status == BookingStatus.Completed)
			{
				throw new InvalidOperationException("Cannot update job details of a completed or cancelled booking.");
			}
			if(string.IsNullOrWhiteSpace(newDescription))
			{
				throw new ArgumentException("Job description cannot be empty.");
			}
			Details = new JobDetails(Id, newDescription);
			AddIntegrationEvent(new BookingUpdatedIntegrationEvent(Id, CustomerId, CraftmanId, StartDate,
				EndDate, Status, CalculateTotalPrice(), new LocalDateTime(DateTime.UtcNow.Year,
				DateTime.UtcNow.Month, DateTime.UtcNow.Day, DateTime.UtcNow.Hour, DateTime.UtcNow.Minute)));
		}

		public decimal CalculateTotalPrice()
		{
			if(Status == BookingStatus.Cancelled)
			{
				return 0;
			}
			return LineItems.Sum(item => item.Price * item.Quantity);
		}
	}
}
