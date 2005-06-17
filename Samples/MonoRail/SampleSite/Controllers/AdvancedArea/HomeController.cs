namespace SampleSite.Controllers.AdvancedArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="AdvancedArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
