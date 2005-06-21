using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Attributes;
// ${Copyrigth}

namespace TestSiteNVelocity.Controllers
{
	using System;

	
	[Resource("resx", "TestSiteNVelocity.Controllers.ResourceFile")]
	public class ResourcedController : SmartDispatcherController
	{
		public ResourcedController()
		{
		}

		public void GetResources()
		{
			
		}
	}
}
