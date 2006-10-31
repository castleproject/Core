namespace GettingStartedSample
{
	using System;
	using System.Web;
	
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;
	
	using GettingStartedSample.Models;
	

	public class GlobalApplication : HttpApplication
	{
		public GlobalApplication()
		{
		}

		public void Application_OnStart()
		{
			ActiveRecordStarter.Initialize(ActiveRecordSectionHandler.Instance, 
			                               new Type[] { typeof(Supplier), typeof(Product) });
			
			// If you want to let ActiveRecord create the schema for you:
			// ActiveRecordStarter.CreateSchema();
		}

		public void Application_OnEnd() 
		{
		}
	}
}
