namespace SampleSite.Controllers.FiltersArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="FiltersArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
