using System.Runtime.Remoting.Messaging;
using Castle.MicroKernel.ComponentActivator;
using Castle.MicroKernel.Lifestyle;

namespace Castle.MVC.LifestyleManager
{
	/// <summary>
	/// PerRequestLifestyleManager.
	/// Create only one instance of the controller per request
	/// </summary>
	public class PerRequestLifestyleManager: AbstractLifestyleManager
	{
		private const string PERREQUESTLIFESTYLEMANAGER = "_PERREQUESTLIFESTYLEMANAGER_";

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override object Resolve()
		{
			string componentName = PERREQUESTLIFESTYLEMANAGER+(this.ComponentActivator as AbstractComponentActivator).Model.Name;
			if( CallContext.GetData(componentName) == null)
			{
				CallContext.SetData(componentName, base.Resolve ());
			}

			return CallContext.GetData(componentName);
		}

		/// <summary>
		/// 
		/// </summary>
		public override void Dispose()
		{
		}
	}
}
