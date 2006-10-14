namespace SimpleBindingSample.Controllers
{
	using System;

	using Castle.MonoRail.Framework;


	public class ContactController : SmartDispatcherController
	{
		/// <summary>
		/// Renders some sample forms
		/// </summary>
		public void Index()
		{
		}
		
		public void PostMessage(String name, String email, int age, String countryCode)
		{
			// We add the values to 
			// the PropertyBag so we can use it on the 'PostMessage' view
			
			// This is a good practice, but for this case it is not
			// required as NVelocity view have access to query/post entries
			
			PropertyBag["name"] = name;
			PropertyBag["email"] = email;
			PropertyBag["age"] = age;
			PropertyBag["country"] = countryCode;
		}
		
		public void PostMessages(String[] name)
		{
			PropertyBag["name"] = name;
		}
	}
}
