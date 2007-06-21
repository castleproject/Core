namespace !NAMESPACE!.Controllers
{
	using System.Collections.Generic;
	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class ContactController : SmartDispatcherController
	{
		public void aboutUs()
		{
			AddAgesToPropertyBag();
			AddCountriesToPropertyBag();

			// You can, for example, query the database for some
			// information about your company, and make it available
			// to the template using the "PropertyBag" property. 
		}

		private void AddAgesToPropertyBag()
		{
			List<string> ages = new List<string>();

			ages.Add("10 - 15");
			ages.Add("16 - 21");
			ages.Add("22 - 28");
			ages.Add("29 - 35");
			ages.Add("36+");

			PropertyBag["ages"] = ages;
		}

		private void AddCountriesToPropertyBag()
		{
			Dictionary<string, string> countries = new Dictionary<string, string>();

			countries.Add("BR", "Brazil");
			countries.Add("CA", "Canada");
			countries.Add("US", "United States");
			countries.Add("RU", "Russia");

			PropertyBag["countries"] = countries;
		}
	}
}
