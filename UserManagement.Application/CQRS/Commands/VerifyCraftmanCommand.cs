using Core.SharedKernel.Enums;
using Core.SharedKernel.ValueObjects;
using MediatR;

namespace UserManagement.Application.CQRS.Commands
{
	public class VerifyCraftmanCommand : IRequest<Unit>
	{
		public Guid CraftmanId { get; set; }
		public Email Email { get; set; }
		public Profession Profession { get; set; }
		public IdentityDocument IdentityDocument { get; set; }
	}
}
