namespace ViewComponentSample.Controllers
{
	using System;
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag.Add("items",
			                new String[]
			                	{
			                		"Item 1", "Item 2", "Item 3", "Item 4", "Item 5", 
			                		"Item 6", "Item 7", "Item 8", "Item 9", "Item 10", 
			                	});
		}
	}
}