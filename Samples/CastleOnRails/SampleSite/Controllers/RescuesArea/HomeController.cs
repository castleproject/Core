namespace SampleSite.Controllers.RescuesArea
{
    using System;

    using Castle.CastleOnRails.Framework;
    
	[Rescue("lie")]
	[ControllerDetails("Home", Area="RescuesArea")]
    public class HomeController : AbstractApplicationController
    {
        public void Index()
        {
        }

		public void Error()
		{
			throw new ApplicationException();
		}

		[Rescue("sincere")]
		public void OtherError()
		{
			throw new ApplicationException();
		}
	}
}
