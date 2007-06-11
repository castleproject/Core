using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	public class UnitOfworkEndPointBehavior : IEndpointBehavior
	{
		public void Validate(ServiceEndpoint endpoint)
		{
		}

		public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			foreach (DispatchOperation operation in endpointDispatcher.DispatchRuntime.Operations)
			{
				operation.CallContextInitializers.Add(new UnitOfWorkCallContextInitializer());
			}
		}

		public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}
	}

	public class UnitOfWorkCallContextInitializer : ICallContextInitializer
	{
		public object BeforeInvoke(InstanceContext instanceContext, IClientChannel channel, Message message)
		{
			UnitOfWork.initialized = true;
			return null;
		}

		public void AfterInvoke(object correlationState)
		{
			UnitOfWork.initialized = false;
		}
	}

	public static class UnitOfWork
	{
		public static bool initialized = false;
	}
}