namespace SampleSite.Controllers.AjaxArea
{
    using System;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class LinkToFunctionController : AbstractAjaxApplicationController
    {
        public void Index()
        {
        }
    }
}
