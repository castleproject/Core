namespace SampleSite.Controllers.HelpersArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="HelpersArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
