namespace SampleSite.Controllers.AdvancedArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="AdvancedArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
