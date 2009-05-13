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

namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.Facilities.WcfIntegration.Internal;

	public class WindsorServiceHostFactory<M> : ServiceHostFactory
		where M : IWcfServiceModel
	{
		private readonly IKernel kernel;

		public WindsorServiceHostFactory()
			: this(WcfServiceExtension.GlobalKernel)
		{
		}

		public WindsorServiceHostFactory(IKernel kernel)
		{
			if (kernel == null)
			{
				string message = "Kernel was null, did you forgot to call DefaultServiceHostFactory.RegisterContainer() ?";
				throw new ArgumentNullException("kernel", message);
			}

			this.kernel = kernel;
		}

		protected IServiceHostBuilder<M> ServiceHostBuilder
		{
			get { return kernel.Resolve<IServiceHostBuilder<M>>(); }
		}

		public override ServiceHostBase CreateServiceHost(string constructorString, Uri[] baseAddresses)
		{
			Type maybeType = Type.GetType(constructorString, false);
			string constructorStringType;
			IHandler handler;
			if (maybeType != null)
			{
				handler = kernel.GetHandler(maybeType);
				constructorStringType = "type";
			}
			else
			{
				handler = kernel.GetHandler(constructorString);
				constructorStringType = "name";
			}
			if (handler == null)
			{
				throw new InvalidOperationException(
					string.Format("Could not find a component with {0} {1}, did you forget to register it?",
					constructorStringType, constructorString));
			}

			ComponentModel model = handler.ComponentModel;
			IWcfServiceModel serviceModel = SelectBestHostedServiceModel(model);

			if (serviceModel != null)
			{
				return WcfServiceExtension.CreateServiceHost(kernel, serviceModel, model, baseAddresses);
			}

			return ServiceHostBuilder.Build(handler.ComponentModel, baseAddresses);
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return ServiceHostBuilder.Build(serviceType, baseAddresses);
		}

		private IWcfServiceModel SelectBestHostedServiceModel(ComponentModel model)
		{
			IWcfServiceModel serviceModel = null;

			foreach (var candidateServiceModel in WcfUtils
						.FindDependencies<IWcfServiceModel>(model.CustomDependencies,
						WcfUtils.IsHosted))
			{
				if (candidateServiceModel is M)
				{
					serviceModel = candidateServiceModel;
					break;
				}
				else
				{
					serviceModel = serviceModel ?? candidateServiceModel;
				}
			}

			return serviceModel;
		}
	}
}