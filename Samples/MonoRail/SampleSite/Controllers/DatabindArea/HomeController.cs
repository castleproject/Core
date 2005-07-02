namespace SampleSite.Controllers.DatabindArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="DatabindArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
