namespace SampleSite.Controllers.MoreArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="MoreArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
