namespace ActiveRecordIntegrationSample
{
	using System;
	using System.Reflection;
	using System.Web;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;

	public class GlobalApplication : HttpApplication
	{
		public GlobalApplication()
		{
		}

		public void Application_OnStart()
		{
			IConfigurationSource config = ActiveRecordSectionHandler.Instance;
			
			Assembly assm = Assembly.Load("Common.Models");
			
			ActiveRecordStarter.Initialize(assm, config);
			
			// If you want to create the schema, uncomment the next line
			// ActiveRecordStarter.CreateSchema();
		}

		public void Application_OnEnd() 
		{
		}
	}
}
