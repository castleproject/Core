using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Castle.Facilities.WcfIntegration
{
	public class WindsorServiceHostFactory : ServiceHostFactory
	{
		public static void RegisterContainer(IWindsorContainer container)
		{
			WindsorServiceHostFactory.globalContainer = container;
		}

		private readonly IWindsorContainer container;
		private static IWindsorContainer globalContainer;

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

		private IWindsorContainer Container
		{
			get
			{
				return container;
			}
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return new WindsorServiceHost(container, serviceType, baseAddresses);
		}
	}
}