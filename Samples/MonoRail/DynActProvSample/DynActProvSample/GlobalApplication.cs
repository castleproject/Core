namespace DynActProvSample
{
	using System;
	using System.Web;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;
	using DynActProvSample.Models;

	public class GlobalApplication : HttpApplication
	{
		public GlobalApplication()
		{
		}

		public void Application_OnStart()
		{
			ActiveRecordStarter.Initialize(
				ActiveRecordSectionHandler.Instance, typeof(Category));
			
			// ActiveRecordStarter.CreateSchema();
		}

		public void Application_OnEnd() 
		{
		}
	}
}
