namespace TempWeb.Controllers
{
	using Castle.MonoRail.Framework;

	[DynamicActionProvider(typeof(WizardActionProvider))]
	public class AccCreationWizard : Controller, IWizardController
	{
		public void OnWizardStart()
		{
		}

		public bool OnBeforeStep(string wizardName, string stepName, IWizardStepPage step)
		{
			return true;
		}

		public void OnAfterStep(string wizardName, string stepName, IWizardStepPage step)
		{
		}

		public IWizardStepPage[] GetSteps(IEngineContext context)
		{
			return new IWizardStepPage[] { new Introduction() };
		}

		public class Introduction : WizardStepPage
		{
			public void Help()
			{
				
			}
		}
	}
}
