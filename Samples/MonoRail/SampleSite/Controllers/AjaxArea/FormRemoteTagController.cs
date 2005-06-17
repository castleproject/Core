namespace SampleSite.Controllers.AjaxArea
{
    using System;
	using System.Threading;
	using System.Collections;

    using Castle.MonoRail.Framework;
    
	[ControllerDetails(Area="AjaxArea")]
    public class FormRemoteTagController : AbstractAjaxApplicationController
    {
        public void Index()
        {
        }

		public void AddContact(String name, String email)
		{
			ObtainContacts().Add( new Contact(name, email) );

			PropertyBag.Add( "contacts", ObtainContacts() );

			RenderView("contactlist");
		}

		public void AddContact2(String name, String email)
		{
			// We pretend that this is a slow operation
			Thread.Sleep( 1000 * 3 );

			ObtainContacts().Add( new Contact(name, email) );

			PropertyBag.Add( "contacts", ObtainContacts() );

			RenderView("contactlist");
		}

		protected IList ObtainContacts()
		{
			LayoutName = null;

			if (!Session.Contains("contacts"))
			{
				Session["contacts"] = new ArrayList();
			}

			return Session["contacts"] as IList;
		}
	}

	public class Contact
	{
		private String name;
		private String email;

		public Contact(string name, string email)
		{
			this.name = name;
			this.email = email;
		}

		public string Name
		{
			get { return name; }
		}

		public string Email
		{
			get { return email; }
		}
	}
}
