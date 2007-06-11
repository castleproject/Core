// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using MicroKernel;
	using Windsor;

	public class WindsorServiceHostFactory : ServiceHostFactory
	{
		private static IWindsorContainer globalContainer;
		private readonly IWindsorContainer container;

		public WindsorServiceHostFactory()
			: this(globalContainer)
		{
		}

		public WindsorServiceHostFactory(IWindsorContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container",
				                                "Container was null, did you forgot to call WindsorServiceHostFactory.RegisterContainer() ?");
			}
			this.container = container;
		}

		private IWindsorContainer Container
		{
			get { return container; }
		}

		public static void RegisterContainer(IWindsorContainer container)
		{
			globalContainer = container;
		}

		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			Type maybeType = Type.GetType(constructorString, false);
			string constructorStringType;
			IHandler handler;
			if (maybeType != null)
			{
				handler = Container.Kernel.GetHandler(maybeType);
				constructorStringType = "type";
			}
			else
			{
				handler = Container.Kernel.GetHandler(constructorString);
				constructorStringType = "name";
			}
			if (handler == null)
				throw new InvalidOperationException(
					string.Format("Could not find a component with {0} {1}, did you forget to register it?", constructorStringType, constructorString));

			return CreateServiceHost(handler.ComponentModel.Implementation, baseAddresses);
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return new WindsorServiceHost(container, serviceType, baseAddresses);
		}
	}
}