// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration.Demo
{
	using System;
	using System.Web;
	using System.ServiceModel;
	using System.ServiceModel.Description;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;

	public class Global : HttpApplication
	{
		private static IWindsorContainer container;

		protected void Application_Start(object sender, EventArgs e)
		{
			ServiceDebugBehavior returnFaults = new ServiceDebugBehavior();
			returnFaults.IncludeExceptionDetailInFaults = true;
			returnFaults.HttpHelpPageEnabled = true;

			ServiceMetadataBehavior metadata = new ServiceMetadataBehavior();
			metadata.HttpGetEnabled = true;

			container = new WindsorContainer()
				.AddFacility<WcfFacility>()
				.Install(Configuration.FromXmlFile("windsor.xml"))
				.Register(
					Component.For<IServiceBehavior>().Instance(returnFaults),
					Component.For<IServiceBehavior>().Instance(metadata),
					Component.For<IAmUsingWindsor>()
					.Named("look no config")
					.ImplementedBy<UsingWindsorWithoutConfig>()
					.DependsOn(
						Property.ForKey("number").Eq(42))
					);
		}

		protected void Application_End(object sender, EventArgs e)
		{
			if (container != null)
			{
				container.Dispose();
			}
		}
	}
}
