// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace BinderSample.Web 
{
	using System;
	using System.Web;
	using System.Configuration;
	
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;

	using Model;

	public class MyHttpApplication : System.Web.HttpApplication
	{
		public MyHttpApplication()
		{
		}	
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			IConfigurationSource source = 
				ConfigurationSettings.GetConfig("activerecord") as IConfigurationSource;

			ActiveRecordStarter.Initialize( source, typeof(Book), typeof(Publisher));

			ActiveRecordStarter.GenerateCreationScripts("../schema.sql");
		}
 
		protected void Session_Start(Object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(Object sender, EventArgs e)
		{
			HttpContext.Current.Items.Add( "nh.sessionscope", new SessionScope() );
		}

		protected void Application_EndRequest(Object sender, EventArgs e)
		{
			SessionScope scope = (SessionScope) HttpContext.Current.Items["nh.sessionscope"];

			try
			{
				if (scope != null) scope.Dispose();
			}
			catch(Exception ex)
			{
				HttpContext.Current.Trace.Warn( "Error", "Problems with the session:" + ex.Message, ex );
			}
		}

		protected void Application_AuthenticateRequest(Object sender, EventArgs e)
		{

		}

		protected void Application_Error(Object sender, EventArgs e)
		{

		}

		protected void Session_End(Object sender, EventArgs e)
		{

		}

		protected void Application_End(Object sender, EventArgs e)
		{

		}
	}
}

