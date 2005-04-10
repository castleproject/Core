namespace SampleSite.Controllers.FiltersArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="FiltersArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
