#if DOTNET2
namespace Castle.MonoRail.TestSupport
{
	using Castle.MonoRail.Framework;

	public class GenericBaseWizardStepTest<W, C> : GenericBaseControllerTest<C> where W : WizardStepPage where C : Controller
	{
		protected W wizardStep;

		protected bool RunIsPreConditionSatisfied()
		{
			object[] args = new object[] { Context };
			return (bool)ReflectionHelper.RunInstanceMethod(typeof(WizardStepPage), wizardStep, "IsPreConditionSatisfied", ref args);
		}

		protected void RunRenderWizardView()
		{
			ReflectionHelper.RunInstanceMethod(typeof(WizardStepPage), wizardStep, "RenderWizardView");
		}
	}
}
#endif