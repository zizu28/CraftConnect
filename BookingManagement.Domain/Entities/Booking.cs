using Core.SharedKernel.Domain;
using Core.SharedKernel.Enums;
using Core.SharedKernel.IntegrationEvents;
using Core.SharedKernel.ValueObjects;
using System.ComponentModel.DataAnnotations;
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
		public string Details { get; private set; }

		[Timestamp]
		public byte[] RowVersion { get; private set; }
		public DateTime StartDate { get; private set; }
		public DateTime EndDate { get; private set; }
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		public List<BookingLineItem> LineItems { get; private set; } = [];

		private Booking()
		{
			
		}

		public static Booking Create(Guid customerId, Guid craftmanId, 
			Address address, string details, DateTime startDate, DateTime endDate)
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
				Details = details,
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
