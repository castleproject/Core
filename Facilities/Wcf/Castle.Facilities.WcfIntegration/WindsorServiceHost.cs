using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Castle.MicroKernel;
using Castle.Windsor;

namespace Castle.Facilities.WcfIntegration
{
	public class WindsorServiceHost : ServiceHost
	{
		private readonly IWindsorContainer container;

		public WindsorServiceHost(IWindsorContainer container, Type serviceType, params Uri[] baseAddresses) 
			: base(serviceType, baseAddresses)
		{
			this.container = container;
		}

		protected override void OnOpening()
		{
			Description.Behaviors.Add(new WindsorDependencyInjectionServiceBehavior(container));
			IHandler[] serviceBehaviorHandlers = container.Kernel.GetHandlers(typeof (IServiceBehavior));
			foreach (IHandler handler in serviceBehaviorHandlers)
			{
				this.Description.Behaviors.Add((IServiceBehavior)handler.Resolve(CreationContext.Empty));
			}
			IHandler[] endPointBehaviors = container.Kernel.GetHandlers(typeof(IEndpointBehavior));
			foreach (IHandler handler in endPointBehaviors)
			{
				foreach (ServiceEndpoint endpoint in this.Description.Endpoints)
				{
					endpoint.Behaviors.Add((IEndpointBehavior) handler.Resolve(CreationContext.Empty));
				}
			}
			base.OnOpening();
		}
	}
}