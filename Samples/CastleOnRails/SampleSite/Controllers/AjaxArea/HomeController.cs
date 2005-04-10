namespace SampleSite.Controllers.AjaxArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails("Home", Area="AjaxArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
