namespace Castle.MonoRail.WindsorExtension
{
	using System;

	using Castle.MicroKernel;
	using Castle.MonoRail.Framework;

	/// <summary>
	/// Default implementation of <see cref="IWizardPageFactory"/>
	/// which requests components from the <see cref="IKernel"/>
	/// </summary>
	public class DefaultWizardPageFactory : IWizardPageFactory
	{
		private readonly IKernel kernel;

		public DefaultWizardPageFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		/// <summary>
		/// Requests a <see cref="WizardStepPage"/> by
		/// the key the component was registered on the 
		/// controller
		/// </summary>
		/// <param name="key">The key used to register the component</param>
		/// <returns>The step page instance</returns>
		public WizardStepPage CreatePage(String key)
		{
			return (WizardStepPage) kernel[key];
		}

		/// <summary>
		/// Requests a <see cref="WizardStepPage"/> by
		/// the key the component was registered on the 
		/// controller
		/// </summary>
		/// <param name="stepPageType"></param>
		/// <returns>The step page instance</returns>
		public WizardStepPage CreatePage(Type stepPageType)
		{
			return (WizardStepPage) kernel[stepPageType];
		}
	}
}
