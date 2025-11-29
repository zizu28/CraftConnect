namespace Core.SharedKernel.ValueObjects
{
	public record UserBreakdown
	{
		public int CraftsmanCount { get; private set; }
		public int CustomerCount { get; private set; }

		private UserBreakdown()
		{
			CraftsmanCount = 0;
			CustomerCount = 0;
		}

		public UserBreakdown(int craftsmanCount, int customerCount)
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(craftsmanCount, 0);
			ArgumentOutOfRangeException.ThrowIfLessThan(customerCount, 0);
			CraftsmanCount = craftsmanCount;
			CustomerCount = customerCount;
		}
	}

	public record ProjectActivity
	{
		public int NewProjects { get; private set; }
		public int CompletedProjects { get; private set; }

		private ProjectActivity()
		{
			NewProjects = 0;
			CompletedProjects = 0;
		}

		public ProjectActivity(int newProjects, int completedProjects)
		{
			ArgumentOutOfRangeException.ThrowIfLessThan(newProjects, 0); 
			ArgumentOutOfRangeException.ThrowIfLessThan(completedProjects, 0);
			NewProjects = newProjects;
			CompletedProjects = completedProjects;
		}
	}
}
