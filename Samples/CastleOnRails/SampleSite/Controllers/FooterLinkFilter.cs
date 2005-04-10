namespace SampleSite.Controllers
{
	using System;

	using Castle.CastleOnRails.Framework;


	public class FooterLinkFilter : IFilter
	{
		public bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			String className = controller.GetType().FullName.Replace('.', '/');
			
			if (className.StartsWith("SampleSite"))
			{
				className = className.Substring( "SampleSite/".Length );
			}

			controller.PropertyBag.Add("controllerfile", className + ".cs");

			return true;
		}
	}
}
