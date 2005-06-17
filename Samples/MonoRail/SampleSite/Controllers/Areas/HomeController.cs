namespace SampleSite.Controllers.Areas
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="Areas")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
