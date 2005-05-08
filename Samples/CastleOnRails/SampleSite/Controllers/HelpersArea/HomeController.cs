namespace SampleSite.Controllers.HelpersArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="HelpersArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
