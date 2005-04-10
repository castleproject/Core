namespace SampleSite.Controllers.LayoutsArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="LayoutsArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
