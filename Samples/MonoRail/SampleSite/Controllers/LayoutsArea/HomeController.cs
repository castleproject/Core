namespace SampleSite.Controllers.LayoutsArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="LayoutsArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
