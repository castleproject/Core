namespace SampleSite.Controllers.MoreArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="MoreArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
