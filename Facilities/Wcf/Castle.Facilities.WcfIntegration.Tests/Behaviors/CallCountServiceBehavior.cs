using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Castle.Facilities.WcfIntegration.Tests.Behaviors
{
	public class CallCountServiceBehavior : IServiceBehavior
	{
		public static int CallCount = 0;

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		public void AddBindingParameters(
			ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
			{
				foreach (EndpointDispatcher endPointDispatcher in channelDispatcher.Endpoints)
				{
					endPointDispatcher.DispatchRuntime.MessageInspectors.Add(new CallCountMessageInspector());
				}
			}
		}

		public class CallCountMessageInspector : IDispatchMessageInspector
		{
			public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
			{
				CallCount += 1;
				return null;
			}

			public void BeforeSendReply(ref Message reply, object correlationState)
			{
			}
		}
	}
}