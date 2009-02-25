
namespace Castle.Facilities.WcfIntegration
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.ServiceModel;
	using System.ServiceModel.Activation;
	using System.ServiceModel.Channels;
	using System.Threading;
	using Castle.Core;
	using Castle.Facilities.WcfIntegration.Internal;
	using Castle.Facilities.WcfIntegration.Rest;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;

	public class WcfServiceExtension : IDisposable
	{
		private IKernel kernel;
		private WcfFacility facility;
		private Binding defaultBinding;
		private TimeSpan? closeTimeout;
		private AspNetCompatibilityRequirementsMode? aspNetCompat;

		internal static IKernel GlobalKernel;

		#region ServiceHostBuilder Delegate Fields 
	
		private delegate ServiceHost CreateServiceHostDelegate(
			IKernel Kernel, IWcfServiceModel serviceModel, ComponentModel model,
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

		public Binding DefaultBinding
		{
			get { return defaultBinding ?? facility.DefaultBinding; }
			set { defaultBinding = value; }
		}

		public TimeSpan? CloseTimeout
		{
			get { return closeTimeout ?? facility.CloseTimeout; }
			set { closeTimeout = value; }
		}

		public bool OpenServiceHostsEagerly { get; set; }

		public AspNetCompatibilityRequirementsMode? AspNetCompatibility
		{
			get { return aspNetCompat; }
			set { aspNetCompat = value; }
		}

		internal void Init(WcfFacility facility)
		{
			this.facility = facility;
			kernel = facility.Kernel;

			ConfigureAspNetCompatibility();
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
			ExtensionDependencies dependencies = null;

			foreach (var serviceModel in ResolveServiceModels(model))
			{
				if (dependencies == null)
				{
					dependencies = new ExtensionDependencies(model, kernel)
						.Apply(new WcfServiceExtensions())
						.Apply(new WcfEndpointExtensions(WcfExtensionScope.Services)
						);
				}

				if (serviceModel != null)
				{
					dependencies.Apply(serviceModel.Extensions);
					
					foreach (var endpoint in serviceModel.Endpoints)
					{
						dependencies.Apply(endpoint.Extensions);
					}
				}
			}
		}

		private void Kernel_ComponentRegistered(string key, IHandler handler)
		{
			ComponentModel model = handler.ComponentModel;

			foreach (var serviceModel in ResolveServiceModels(model))
			{ 
				if (!serviceModel.IsHosted)
				{
					CreateServiceHostWhenHandlerIsValid(handler, serviceModel, model);
				}
			}
		}

		private void Kernel_ComponentUnregistered(string key, IHandler handler)
		{
			var serviceHosts = handler.ComponentModel
				.ExtendedProperties[WcfConstants.ServiceHostsKey] as IList<ServiceHost>;

			if (serviceHosts != null)
			{
				foreach (var serviceHost in serviceHosts)
				{
					foreach (var cleanUp in serviceHost.Extensions.FindAll<IWcfCleanUp>())
					{
						cleanUp.CleanUp();
					}
					WcfUtils.ReleaseCommunicationObject(serviceHost, CloseTimeout);
				}
			}
		}

		private void ConfigureAspNetCompatibility()
		{
			if (aspNetCompat.HasValue)
			{
				facility.Kernel.Register(
					Component.For<AspNetCompatibilityRequirementsAttribute>()
						.Instance(new AspNetCompatibilityRequirementsAttribute
						{
							RequirementsMode = aspNetCompat.Value
						})
					);
			}
		}

		private void AddDefaultServiceHostBuilders()
		{
			AddServiceHostBuilder<DefaultServiceHostBuilder, DefaultServiceModel>(false);
			AddServiceHostBuilder<RestServiceHostBuilder, RestServiceModel>(false);
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
				foreach (var serviceModel in WcfUtils.FindDependencies<IWcfServiceModel>(model.CustomDependencies))
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

		public static ServiceHost CreateServiceHost(IKernel Kernel, IWcfServiceModel serviceModel,
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

			return createServiceHost(Kernel, serviceModel, model, baseAddresses);
		}

		internal static ServiceHost CreateServiceHostInternal<M>(IKernel kernel, 
		                                                         IWcfServiceModel serviceModel, 
			                                                     ComponentModel model,
																 params Uri[] baseAddresses)
			where M : IWcfServiceModel
		{
			var serviceHostBuilder = kernel.Resolve<IServiceHostBuilder<M>>();
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
			if (serviceModel.ShouldOpenEagerly.GetValueOrDefault(OpenServiceHostsEagerly) ||
				handler.CurrentState == HandlerState.Valid)
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
			var serviceHost = CreateServiceHost(kernel, serviceModel, model);
			var serviceHosts = model.ExtendedProperties[WcfConstants.ServiceHostsKey] as IList<ServiceHost>;

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
