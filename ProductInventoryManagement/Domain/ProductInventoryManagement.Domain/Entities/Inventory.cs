using Core.SharedKernel.Domain;

namespace ProductInventoryManagement.Domain.Entities
{
	public class Inventory : Entity
	{
		public int Quantity { get; private set; }
		public DateTime LastUpdated { get; private set; } = DateTime.UtcNow;

		private Inventory() { }

		public Inventory(int initialQuantity)
		{
			if (initialQuantity < 0)
			{
				throw new ArgumentException("Initial quantity cannot be negative.");
			}
			Quantity = initialQuantity;
			LastUpdated = DateTime.UtcNow;
		}

		public void UpdateQuantity(int quantity)
		{
			if (quantity < 0)
			{
				throw new ArgumentException("Quantity cannot be negative.");
			}
			Quantity = quantity;
			LastUpdated = DateTime.UtcNow;
		}

		public void Deduct(int quantity)
		{
			if (quantity <= 0 || quantity > Quantity)
			{
				throw new ArgumentException("Invalid quantity to deduct.");
			}
			Quantity -= quantity;
			LastUpdated = DateTime.UtcNow;
		}

		public void Add(int quantity)
		{
			if (quantity <= 0)
			{
				throw new ArgumentException("Quantity to add cannot be zero or negative.");
			}
			Quantity += quantity;
			LastUpdated = DateTime.UtcNow;
		}
	}
}