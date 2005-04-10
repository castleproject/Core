namespace SampleSite.Controllers.AjaxArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class LinkToFunctionController : AbstractAjaxApplicationController
    {
        public void Index()
        {
        }
    }
}
