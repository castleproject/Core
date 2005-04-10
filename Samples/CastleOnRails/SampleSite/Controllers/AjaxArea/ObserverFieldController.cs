namespace SampleSite.Controllers.AjaxArea
{
    using System;
	using System.Collections;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class ObserverFieldController : AbstractAjaxApplicationController
    {
        public void Index()
        {

        }

		public void AddressFromZip(String value)
		{
			RenderText("28th St.");
		}

		public void GetStatesFromCountry(String value)
		{
			LayoutName = null;

			ArrayList states = new ArrayList();

			if ("br".Equals(value))
			{
				states.Add("Sao Paulo");
				states.Add("Rio de Janeiro");
				states.Add("Espirito Santo");
			}
			else
			{
				states.Add("London");
				states.Add("Seattle");
				states.Add("Tokio");
				states.Add("York");
			}

			PropertyBag.Add("states", states);
			RenderView("statelist");
		}
	}
}
