using Core.SharedKernel.Domain;

namespace BookingManagement.Domain.Entities
{
	public class BookingLineItem : Entity
	{
		public Guid BookingId { get; private set; }
		public string Description { get; private set; }
		public decimal Price { get; private set; }
		public int Quantity { get; private set; }

		private BookingLineItem()
		{
		}

		internal BookingLineItem(Guid bookingId, string description, decimal price, int quantity)
		{
			Id = Guid.NewGuid();
			BookingId = bookingId;
			Description = description;
			Price = price;
			Quantity = quantity;
		}
	}
}
