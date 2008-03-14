namespace Castle.Facilities.WcfIntegration
{
    using System;
    using System.Reflection;
    using System.ServiceModel;

    public class WcfClientModel : WcfEndpoint
    {
		public WcfClientModel()
		{
		}

		public WcfClientModel(string endpointName)
		{
			EndpointName = endpointName;
		}

        internal CreateChannel GetChannelBuilder()
        {
            object target;
            Type type = typeof(ChannelFactory<>).MakeGenericType(new Type[] { Contract });

            if (!string.IsNullOrEmpty(EndpointName))
            {
                target = Activator.CreateInstance(type, new object[] { EndpointName });
            }
            else
            {
                EndpointAddress address = EndpointAddress;
                if (address == null)
                {
                    address = new EndpointAddress(Address);
                }
                target = Activator.CreateInstance(type, new object[] { Binding, address });
            }
            MethodInfo methodInfo = type.GetMethod("CreateChannel", new Type[0]);
            return (CreateChannel) Delegate.CreateDelegate(typeof(CreateChannel), target, methodInfo);
        }
    }
}

