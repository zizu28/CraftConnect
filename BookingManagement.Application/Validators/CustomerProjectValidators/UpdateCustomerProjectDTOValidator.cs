using BookingManagement.Application.DTOs.CustomerProjectDTOs;
using FluentValidation;

namespace BookingManagement.Application.Validators.CustomerProjectValidators
{
	public class UpdateCustomerProjectDTOValidator : AbstractValidator<UpdateCustomerProjectDTO>
	{
		public UpdateCustomerProjectDTOValidator()
		{
			RuleFor(x => x.ProjectId)
				.NotEmpty().WithMessage("Project ID is required.");
			RuleFor(x => x.Title)
				.NotEmpty().MaximumLength(100);
			RuleFor(x => x.Description)
				.NotEmpty().MinimumLength(20);
			When(x => x.Budget != null, () =>
			{
				RuleFor(x => x.Budget.Amount).GreaterThan(0);
			});
			When(x => x.Timeline != null, () =>
			{
				RuleFor(x => x.Timeline.End)
					.GreaterThan(x => x.Timeline.Start)
					.When(t => t.Timeline.Start != default && t.Timeline.End != default);
			});
		}
	}
}