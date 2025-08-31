using Core.SharedKernel.Domain;
using System.ComponentModel.DataAnnotations;

namespace ProductInventoryManagement.Domain.Entities;

public class Category : AggregateRoot
{
	public string Name { get; private set; }
	public string Description { get; private set; }
	public Guid? ParentCategoryId { get; private set; }
	[Timestamp]
	public byte[] RowVersion { get; set; }
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

	public Category AddCategory(string name, string description, Guid? parentCategoryId = null)
	{
		if(string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("Category name cannot be empty.", nameof(name));
		}
		if(parentCategoryId == Id)
		{
			throw new ArgumentException("A category cannot be its own parent.", nameof(parentCategoryId));
		}
		if(parentCategoryId != null && parentCategoryId == ParentCategoryId)
		{
			throw new ArgumentException("This category already has the specified parent category.", nameof(parentCategoryId));
		}
		return new Category(name, description, parentCategoryId);
	}

	public void UpdateDetails(Guid parentCategoryId, string name, string description)
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
}