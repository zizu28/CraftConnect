namespace CraftConnect.WASM.ViewModels
{
	public class OnboardingViewModel
	{
		public List<OnboardingStep> Steps { get; set; } = new List<OnboardingStep>();
		public int CurrentStepIndex { get; set; } = 0;
	}
}
