namespace SampleSite.Controllers.AjaxArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class ObserverFormController : AbstractAjaxApplicationController
    {
        public void Index()
        {
        }

		public void Validate(String name, String age)
		{
			CancelView();

			if (name == null || name.Length == 0)
			{
				RenderText("<strong>Please enter your name</strong>");
			}
			else if (age == null || age.Length == 0)
			{
				RenderText("<strong>Please enter your age</strong>");
			}
		}
	}
}
