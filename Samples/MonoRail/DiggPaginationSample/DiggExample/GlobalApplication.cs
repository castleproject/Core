namespace DiggExample
{
	using System;
	using System.Web;
	using DiggExample.Model;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;

	public class GlobalApplication : HttpApplication
	{
		public GlobalApplication()
		{
		}

		public void Application_OnStart()
		{
			ActiveRecordStarter.Initialize(ActiveRecordSectionHandler.Instance,
							   new Type[] { typeof(MyEntity) });

			ActiveRecordStarter.CreateSchema();
			using (new SessionScope())
			{
				for (int i = 0; i < 200; i++)
				{
					MyEntity e = new MyEntity();
					e.Name = "Entity " + (i + 1);
					e.Index = i;
					e.Create();
				}
			}

		}

		public void Application_OnEnd()
		{
			ActiveRecordStarter.DropSchema();
		}
	}
}
