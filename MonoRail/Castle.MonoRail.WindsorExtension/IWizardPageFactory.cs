namespace Castle.MonoRail.WindsorExtension
{
	using System;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// This interface allow a wizard controller
	/// to request wizard steps from the IKernel without
	/// directly refering to it
	/// </summary>
	public interface IWizardPageFactory
	{
		/// <summary>
		/// Requests a <see cref="WizardStepPage"/> by
		/// the key the component was registered on the 
		/// controller
		/// </summary>
		/// <param name="key">The key used to register the component</param>
		/// <returns>The step page instance</returns>
		WizardStepPage CreatePage(String key);

		/// <summary>
		/// Requests a <see cref="WizardStepPage"/> by
		/// the key the component was registered on the 
		/// controller
		/// </summary>
		/// <param name="stepPageType"></param>
		/// <returns>The step page instance</returns>
		WizardStepPage CreatePage(Type stepPageType);
	}
}
