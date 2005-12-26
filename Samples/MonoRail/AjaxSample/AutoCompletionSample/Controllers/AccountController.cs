namespace MonoRail.AjaxSample.Controllers
{
    using System;
    using System.Collections;
    using Castle.MonoRail.Framework;
    
    
    public class AccountController : SmartDispatcherController
    {
        public void Index()
        {
        }

		public void GetSearchItems(String name)
		{
			IList items = GetRecords();
			IList matchItems = new ArrayList();

			name = name.ToLower();

			foreach (string item in items)
			{
				if (item.ToLower().StartsWith(name))
				{
					matchItems.Add(item);
				}
			}

			PropertyBag.Add("items", matchItems);

			RenderView("partialmatchlist");
		}

		private IList GetRecords()
		{
			ArrayList items = new ArrayList();

			items.Add("Ted");
			items.Add("Teddy");
			items.Add("Mark");
			items.Add("Alfred");
			
			return items;
		}
    }
}
