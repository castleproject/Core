// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Threading;
	using System.Reflection;
	using System.Collections.Generic;
	using System.ServiceModel;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.Facilities.WcfIntegration.Internal;

	internal class WcfServiceExtension : IDisposable
	{
		private readonly IKernel kernel;

		#region ServiceHostBuilder Delegate Fields 
	
		private delegate ServiceHost CreateServiceHostDelegate(
			IKernel kernel, IWcfServiceModel serviceModel, Type serviceType, ComponentModel model);

		private static readonly MethodInfo createServiceHostMethod =
			typeof(WcfServiceExtension).GetMethod("CreateServiceHostInternal",
				BindingFlags.NonPublic | BindingFlags.Static, null,
				new Type[] { typeof(IKernel), typeof(IWcfServiceModel), typeof(Type),
					typeof(ComponentModel) }, null
				);

		private static readonly Dictionary<Type, CreateServiceHostDelegate> 
			createServiceHostCache = new Dictionary<Type, CreateServiceHostDelegate>();

		private static ReaderWriterLock locker = new ReaderWriterLock();

		#endregion

		public WcfServiceExtension(IKernel kernel)
		{
			this.kernel = kernel;

			WindsorServiceHostFactory.RegisterContainer(kernel);
			AddServiceHostBuilder<DefaultServiceHostBuilder, WcfServiceModel>(false);

			kernel.ComponentRegistered += Kernel_ComponentRegistered;
			kernel.ComponentUnregistered += Kernel_ComponentUnregistered;
		}
	
		private void Kernel_ComponentRegistered(string key, IHandler handler)
		{
			ComponentModel model = handler.ComponentModel;
			IWcfServiceModel serviceModel = ResolveServiceModel(model);

			if (serviceModel != null)
			{
				model.ExtendedProperties[WcfConstants.ServiceModelKey] = serviceModel;

				if (!serviceModel.IsHosted)
				{
					ServiceHost serviceHost = CreateServiceHost(kernel, serviceModel, model);
					model.ExtendedProperties[WcfConstants.ServiceHostKey] = serviceHost;
				}
			}
		}

		private void Kernel_ComponentUnregistered(string key, IHandler handler)
		{
			ComponentModel model = handler.ComponentModel;
			ServiceHost serviceHost =
				model.ExtendedProperties[WcfConstants.ServiceHostKey] as ServiceHost;

			if (serviceHost != null)
			{
				serviceHost.Close();
			}
		}

		internal void AddServiceHostBuilder<T, M>(bool force)
			where T : IServiceHostBuilder<M>
			where M : IWcfServiceModel
		{
			if (force || !kernel.HasComponent(typeof(IServiceHostBuilder<M>)))
			{
				kernel.AddComponent<T>(typeof(IServiceHostBuilder<M>));
			}
		}

		private IWcfServiceModel ResolveServiceModel(ComponentModel model)
		{
			IWcfServiceModel serviceModel = null;

			if (model.Implementation.IsClass && !model.Implementation.IsAbstract)
			{
				if (!WcfUtils.FindDependency<IWcfServiceModel>(
						model.CustomDependencies, out serviceModel) &&
					model.Configuration != null)
				{
					if ("true" == model.Configuration.Attributes[WcfConstants.ServiceHostEnabled])
					{
						serviceModel = new WcfServiceModel();
					}
				}
			}

			return serviceModel;
		}

		#region CreateServiceHost Members
	
		public static ServiceHost CreateServiceHost(IKernel kernel, IWcfServiceModel serviceModel,
													Type serviceType)
		{
			return CreateServiceHost(kernel, serviceModel, serviceType, null);
		}

		public static ServiceHost CreateServiceHost(IKernel kernel, IWcfServiceModel serviceModel,
													ComponentModel model)
		{
			return CreateServiceHost(kernel, serviceModel, null, model);
		}

		private static ServiceHost CreateServiceHost(IKernel kernel, IWcfServiceModel serviceModel, 
			                                         Type serviceType, ComponentModel model)
		{
			CreateServiceHostDelegate createServiceHost;

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);

				Type serviceModelType = serviceModel.GetType();

				if (!createServiceHostCache.TryGetValue(serviceModelType, out createServiceHost))
				{
					locker.UpgradeToWriterLock(Timeout.Infinite);

					createServiceHost = (CreateServiceHostDelegate)
						Delegate.CreateDelegate(typeof(CreateServiceHostDelegate),
							createServiceHostMethod.MakeGenericMethod(serviceModelType));
					createServiceHostCache.Add(serviceModelType, createServiceHost);
				}
			}
			finally
			{
				locker.ReleaseLock();
			}

			return createServiceHost(kernel, serviceModel, serviceType, model);
		}

		internal static ServiceHost CreateServiceHostInternal<M>(IKernel kernel, IWcfServiceModel serviceModel, 
			                                                     Type serviceType, ComponentModel model)
			where M : IWcfServiceModel
		{
			IServiceHostBuilder<M> serviceHostBuilder = kernel.Resolve<IServiceHostBuilder<M>>();
			if (model != null)
			{
				return serviceHostBuilder.Build(model, (M)serviceModel);
			}
			return serviceHostBuilder.Build(serviceType, (M)serviceModel);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}
