// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace TestSiteWindsor
{
	using System;
	using System.Web;

	using Castle.Windsor;
	using Castle.CastleOnRails.WindsorExtension;

	using TestSiteWindsor.Controllers;


	public class MyHttpApplication : HttpApplication, IContainerAccessor
	{
		private static WindsorContainer container;

		public void Application_OnStart() 
		{
			container = new WindsorContainer();

			container.AddFacility( "rails", new RailsFacility() );

			AddControllers(container);
		}

		private void AddControllers(WindsorContainer container)
		{
			container.AddComponent( "auth.filter", typeof(AuthenticationFilter) );
			container.AddComponent( "home", typeof(HomeController) );
			container.AddComponent( "registration", typeof(MyController) );
			container.AddComponent( "account", typeof(AccountController) );
		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}
