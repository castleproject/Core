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

namespace Castle.Applications.MindDump.Web
{
	using System;
	using System.Web;

	using Castle.Windsor;

	using Castle.CastleOnRails.WindsorExtension;

	using Castle.Applications.MindDump.Services;
	using Castle.Applications.MindDump.Web.Controllers;
	using Castle.Applications.MindDump.Web.Controllers.Filters;


	public class MindDumpHttpApplication : HttpApplication, IContainerAccessor
	{
		private static WindsorContainer container;

		public void Application_OnStart() 
		{
			container = new MindDumpContainer();
			container.AddFacility( "rails", new RailsFacility() );

			AddFiltersAndControllers(container);
			SubcribeForEvents(container);
		}

		private void AddFiltersAndControllers(WindsorContainer container)
		{
			container.AddComponent( "auth.filter", typeof(AuthenticationCheckFilter) );
			container.AddComponent( "auth.attempt.filter", typeof(AuthenticationAttemptFilter) );
			container.AddComponent( "intro.controller", typeof(IntroController) );
			container.AddComponent( "account.controller", typeof(AccountController) );
			container.AddComponent( "blogs.controller", typeof(BlogController) );
			container.AddComponent( "maintenance.controller", typeof(MaintenanceController) );
		}

		private void SubcribeForEvents(WindsorContainer container)
		{
			container.AddComponent( "blog.creator.subscriber", typeof(BlogControllerCreatorSubscriber) );
			
			IMindDumpEventPublisher channel = (IMindDumpEventPublisher) 
				container[ typeof(IMindDumpEventPublisher) ];

			channel.AddSubcriber( (IMindDumpEventSubscriber) 
				container[ typeof(BlogControllerCreatorSubscriber) ] );
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
