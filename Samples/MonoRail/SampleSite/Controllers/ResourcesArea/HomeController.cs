namespace SampleSite.Controllers.ResourcesArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="ResourcesArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
