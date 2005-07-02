namespace SampleSite.Controllers.ScaffoldArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails("Home", Area="ScaffoldArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }
    }
}
