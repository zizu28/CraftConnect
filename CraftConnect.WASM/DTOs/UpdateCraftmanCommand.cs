using UserManagement.Domain.Entities;

namespace CraftConnect.WASM.DTOs
{
	public class UpdateCraftmanCommand
	{
		public Guid CraftmanId { get; set; }
		public CraftsmanProfileUpdateDTO CraftmanDTO { get; set; }
	}
}
