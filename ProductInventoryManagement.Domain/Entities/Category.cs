using Core.SharedKernel.Domain;

namespace ProductInventoryManagement.Domain.Entities;

public class Category : AggregateRoot
{
	public string Name { get; private set; }
	public string Description { get; private set; }
	public Guid? ParentCategoryId { get; private set; }
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTime LastModified { get; private set; }

	private Category() { }

	public Category(string name, string description, Guid? parentCategoryId = null)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Category name cannot be empty.", nameof(name));
		}

		Name = name;
		Description = description;
		ParentCategoryId = parentCategoryId;
		LastModified = DateTime.UtcNow;
	}

	public void UpdateDetails(string name, string description)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Category name cannot be empty.", nameof(name));
		}

		Name = name;
		Description = description;
		LastModified = DateTime.UtcNow;
	}
}