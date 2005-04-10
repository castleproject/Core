namespace SampleSite.Controllers.ControllersArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="ControllersArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
