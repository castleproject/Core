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

namespace TestScaffolding
{
	using System;
	using System.Web;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using TestScaffolding.Model;

	public class MyHttpApplication : HttpApplication
	{
		protected void Application_Start(Object sender, EventArgs e)
		{
			IConfigurationSource source = ActiveRecordSectionHandler.Instance;

			ActiveRecordStarter.Initialize( 
				source, 
				typeof(Blog), typeof(Category), typeof(Person), 
				typeof(Customer), typeof(Account), 
				typeof(AccountPermission), typeof(ProductLicense) );
			
//			ActiveRecordStarter.DropSchema();
//			
//			ActiveRecordStarter.CreateSchema();
		}
	}
}
