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

namespace Castle.Applications.PestControl.Web
{
	using System;
	using System.Web;
	using System.Web.Security;

	using Castle.Applications.PestControl.Model;
	using Castle.Applications.PestControl.Web.Controllers;
	using Castle.Applications.PestControl.Web.Controllers.Filters;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Model.Resource;
	using Castle.MonoRail.WindsorExtension;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	/// <summary>
	/// Summary description for PestControlApplication.
	/// </summary>
	public class PestControlApplication : HttpApplication, IContainerAccessor
	{
		private static WindsorContainer container;

		{
			DefaultConfigurationStore store = new DefaultConfigurationStore();
			XmlInterpreter interpreter = new XmlInterpreter(new ConfigResource());
			interpreter.ProcessResource(interpreter.Source, store);
			container = new PestControlContainer(interpreter);

			container.AddFacility("rails", new RailsFacility());

			this.AddFiltersAndControllers(container);
		}

		private void AddFiltersAndControllers(WindsorContainer container)
		{
		}

		{
			container.Dispose();
		}

		public void FormsAuthentication_OnAuthenticate(Object sender, FormsAuthenticationEventArgs e)
		{


			FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cookie.Value);


		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
		}

		#endregion
	}
}
