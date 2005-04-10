namespace SampleSite.Controllers.AjaxArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class LinkToRemoteController : AbstractAjaxApplicationController
    {
        public void Index()
        {
        }

		public void ShowTime()
		{
			RenderText( "Currently it's <b>" + DateTime.Now.ToShortTimeString() + "</b>" );
		}

		public void CalcPower(int value)
		{
			// Disable Layout
			LayoutName = null;

			// Results
			PropertyBag.Add("result", (value * value).ToString());

			// Now its time to CalcPower.vm to do the rest
		}
	}
}
