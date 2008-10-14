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
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using System.ServiceModel;
	using System.Threading;
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.Facilities.WcfIntegration.Rest;
	using Castle.MicroKernel;

	public class WcfServiceExtension : IDisposable
	{
		private readonly IKernel kernel;

		internal static IKernel GlobalKernel;

		private readonly IDictionary<ServiceHost, ICollection<IHandler>> waitingOn =
			new Dictionary<ServiceHost, ICollection<IHandler>>();

		#region ServiceHostBuilder Delegate Fields 
	
		private delegate ServiceHost CreateServiceHostDelegate(
			IKernel kernel, IWcfServiceModel serviceModel, ComponentModel model,
			Uri[] baseAddresses);

		private static readonly MethodInfo createServiceHostMethod =
			typeof(WcfServiceExtension).GetMethod("CreateServiceHostInternal",
				BindingFlags.NonPublic | BindingFlags.Static, null,
				new Type[] { typeof(IKernel), typeof(IWcfServiceModel),
					typeof(ComponentModel), typeof(Uri[]) }, null
				);

		private static readonly Dictionary<Type, CreateServiceHostDelegate> 
			createServiceHostCache = new Dictionary<Type, CreateServiceHostDelegate>();

		private static ReaderWriterLock locker = new ReaderWriterLock();

		#endregion

		public WcfServiceExtension(IKernel kernel)
		{
			this.kernel = kernel;

			AddDefaultServiceHostBuilders();
			DefaultServiceHostFactory.RegisterContainer(kernel);

			kernel.ComponentModelCreated += Kernel_ComponentModelCreated;
			kernel.ComponentRegistered += Kernel_ComponentRegistered;
			kernel.ComponentUnregistered += Kernel_ComponentUnregistered;
		}

		public WcfServiceExtension AddServiceHostBuilder<T, M>()
			where T : IServiceHostBuilder<M>
			where M : IWcfServiceModel
		{
			AddServiceHostBuilder<T, M>(true);
			return this;
		}

		private void Kernel_ComponentModelCreated(ComponentModel model)
		{
			BehaviorDependencies dependencies = null;

			foreach (IWcfServiceModel serviceModel in ResolveServiceModels(model))
			{
				if (dependencies == null)
				{
					dependencies = new BehaviorDependencies(model, kernel)
						.Apply(new WcfServiceBehaviors())
						.Apply(new WcfEndpointBehaviors(WcfBehaviorScope.Services)
						);
				}

				if (serviceModel != null)
				{
					dependencies.Apply(serviceModel.Behaviors);
					
					foreach (IWcfEndpoint endpoint in serviceModel.Endpoints)
					{
						dependencies.Apply(endpoint.Behaviors);
					}
				}
			}
		}

		private void Kernel_ComponentRegistered(string key, IHandler handler)
		{
			ComponentModel model = handler.ComponentModel;

			foreach (IWcfServiceModel serviceModel in ResolveServiceModels(model))
			{ 
				if (!serviceModel.IsHosted)
				{
					CreateServiceHostWhenHandlerIsValid(handler, serviceModel, model);
				}
			}
		}

		private void Kernel_ComponentUnregistered(string key, IHandler handler)
		{
			IList<ServiceHost> serviceHosts = handler.ComponentModel
				.ExtendedProperties[WcfConstants.ServiceHostsKey] as IList<ServiceHost>;

			if (serviceHosts != null)
			{
				foreach (ServiceHost serviceHost in serviceHosts)
				{
					WcfUtils.ReleaseCommunicationObject(serviceHost);
				}
			}
		}

		private void AddDefaultServiceHostBuilders()
		{
			AddServiceHostBuilder<DefaultServiceHostBuilder, DefaultServiceModel>(false);
#if DOTNET35
			AddServiceHostBuilder<RestServiceHostBuilder, RestServiceModel>(false);
#endif
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

		private IEnumerable<IWcfServiceModel> ResolveServiceModels(ComponentModel model)
		{
			bool foundOne = false;

			if (model.Implementation.IsClass && !model.Implementation.IsAbstract)
			{
				foreach (IWcfServiceModel serviceModel in 
					WcfUtils.FindDependencies<IWcfServiceModel>(model.CustomDependencies))
				{
					foundOne = true;
					yield return serviceModel;
				}

				if (!foundOne && model.Configuration != null &&
					"true" == model.Configuration.Attributes[WcfConstants.ServiceHostEnabled])
				{
					yield return new DefaultServiceModel();
				}
			}
		}

		#region CreateServiceHost Members

		public static ServiceHost CreateServiceHost(IKernel kernel, IWcfServiceModel serviceModel,
													ComponentModel model, params Uri[] baseAddresses)
		{
			CreateServiceHostDelegate createServiceHost;

			try
			{
				locker.AcquireReaderLock(Timeout.Infinite);

				Type serviceModelType = serviceModel.GetType();

				if (!createServiceHostCache.TryGetValue(serviceModelType, out createServiceHost))
				{
					locker.UpgradeToWriterLock(Timeout.Infinite);

					if (!createServiceHostCache.TryGetValue(serviceModelType, out createServiceHost))
					{
						createServiceHost = (CreateServiceHostDelegate)
							Delegate.CreateDelegate(typeof(CreateServiceHostDelegate),
								createServiceHostMethod.MakeGenericMethod(serviceModelType));
						createServiceHostCache.Add(serviceModelType, createServiceHost);
					}
				}
			}
			finally
			{
				locker.ReleaseLock();
			}

			return createServiceHost(kernel, serviceModel, model, baseAddresses);
		}

		internal static ServiceHost CreateServiceHostInternal<M>(IKernel kernel, 
		                                                         IWcfServiceModel serviceModel, 
			                                                     ComponentModel model,
																 params Uri[] baseAddresses)
			where M : IWcfServiceModel
		{
			IServiceHostBuilder<M> serviceHostBuilder = kernel.Resolve<IServiceHostBuilder<M>>();
			return serviceHostBuilder.Build(model, (M)serviceModel, baseAddresses);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion

		private void CreateServiceHostWhenHandlerIsValid(IHandler handler, IWcfServiceModel serviceModel,
			                                             ComponentModel model)
		{
			if (handler.CurrentState == HandlerState.Valid)
			{
				CreateAndOpenServiceHost(serviceModel, model);
			}
			else
			{
				HandlerDelegate onStateChanged = null;
				onStateChanged = delegate(IHandler valid, ref bool stateChanged)
				{
					if (handler.CurrentState == HandlerState.Valid && onStateChanged != null)
					{
						kernel.HandlerRegistered -= onStateChanged;
						onStateChanged = null;

						CreateAndOpenServiceHost(serviceModel, model);
					}
				};
				kernel.HandlerRegistered += onStateChanged;
			}
		}

		private void CreateAndOpenServiceHost(IWcfServiceModel serviceModel, ComponentModel model)
		{
			ServiceHost serviceHost = CreateServiceHost(kernel, serviceModel, model);
		
			IList<ServiceHost> serviceHosts = 
				model.ExtendedProperties[WcfConstants.ServiceHostsKey] as IList<ServiceHost>;

			if (serviceHosts == null)
			{
				serviceHosts = new List<ServiceHost>();
				model.ExtendedProperties[WcfConstants.ServiceHostsKey] = serviceHosts;
			}

			serviceHosts.Add(serviceHost);

			serviceHost.Open();
		}
	}
}
