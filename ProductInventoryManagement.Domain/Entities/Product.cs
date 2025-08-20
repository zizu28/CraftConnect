using Core.SharedKernel.Domain;
using Core.SharedKernel.ValueObjects;

namespace ProductInventoryManagement.Domain.Entities
{
	public class Product : AggregateRoot
	{
		public string Name { get; private set; }
		public string Description { get; private set; }
		public decimal Price { get; private set; }
		public Guid CategoryId { get; private set; }
		public Guid CraftmanId { get; private set; }
		public Guid InventoryId { get; private set; }
		public Inventory Inventory { get; private set; }
		public List<Image> Images { get; private set; } = [];
		public bool IsActive { get; private set; }
		public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
		public DateTime LastModified { get; private set; }

		private Product() { }

		public Product(string name, string description, decimal price, Guid categoryId)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentException("Name and description cannot be empty.");
			}
			if (price <= 0)
			{
				throw new ArgumentException("Price must be greater than zero.");
			}
			Name = name;
			Description = description;
			Price = price;
			CategoryId = categoryId;
			IsActive = true;
			CreatedAt = DateTime.UtcNow;
			LastModified = DateTime.UtcNow;
		}

		public void ChangeCategory(Guid newCategoryId)
		{
			if (newCategoryId == Guid.Empty)
			{
				throw new ArgumentException("Category ID cannot be empty.");
			}
			CategoryId = newCategoryId;
			LastModified = DateTime.UtcNow;
		}

		public void UpdateDetails(string name, string description)
		{
			if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(description))
			{
				throw new ArgumentException("Name and description cannot be empty.");
			}
			Name = name;
			Description = description;
			LastModified = DateTime.UtcNow;
		}

		public void UpdatePrice(decimal price)
		{
			if (price <= 0)
			{
				throw new ArgumentException("Price must be greater than zero.");
			}
			Price = price;
			LastModified = DateTime.UtcNow;
		}

		public void InitializeInventory(int initialQuantity)
		{
			if (Inventory != null)
			{
				throw new InvalidOperationException("Inventory has already been initialized for this product.");
			}
			Inventory = new Inventory(initialQuantity);
			LastModified = DateTime.UtcNow;
		}

		public void UpdateInventory(int quantity)
		{
			if (Inventory == null)
			{
				throw new InvalidOperationException("Product does not have an associated inventory.");
			}
			Inventory.UpdateQuantity(quantity);
			LastModified = DateTime.UtcNow;
		}

		public void Activate()
		{
			IsActive = true;
			LastModified = DateTime.UtcNow;
		}

		public void Deactivate()
		{
			IsActive = false;
			LastModified = DateTime.UtcNow;
		}

		public void Reserve(int quantity)
		{
			if (Inventory == null)
			{
				throw new InvalidOperationException("Product does not have an associated inventory.");
			}
			Inventory.Deduct(quantity);
			LastModified = DateTime.UtcNow;
		}

		public void Release(int quantity)
		{
			if (Inventory == null)
			{
				throw new InvalidOperationException("Product does not have an associated inventory.");
			}
			Inventory.Add(quantity);
			LastModified = DateTime.UtcNow;
		}

		public void AddImage(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image), "Image cannot be null.");
			}
			Images.Add(image);
			LastModified = DateTime.UtcNow;
		}

		public void RemoveImage(Image image)
		{
			if (image == null)
			{
				throw new ArgumentNullException(nameof(image), "Image cannot be null.");
			}
			if (!Images.Remove(image))
			{
				throw new InvalidOperationException("Image not found in product images.");
			}
			LastModified = DateTime.UtcNow;
		}
	}
}