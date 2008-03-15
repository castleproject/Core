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
    using System.Reflection;
    using System.ServiceModel;

    public class WcfClientModel : WcfEndpoint
    {
		public WcfClientModel()
		{
		}

		public WcfClientModel(Type contract)
			: base(contract)
		{
		}

		public WcfClientModel(string endpointName)
		{
			EndpointName = endpointName;
		}

		public WcfClientModel(Type contract, string endpointName)
			: this(contract)
		{
			EndpointName = endpointName;
		}


		internal CreateChannel GetChannelBuilder()
		{
			return GetChannelBuilder(Contract);
		}

        internal CreateChannel GetChannelBuilder(Type contract)
        {
            object target;
			contract = contract ?? Contract;
            Type type = typeof(ChannelFactory<>).MakeGenericType(new Type[] { contract });

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

	public class WcfClientModel<Contract> : WcfClientModel
	{
		public WcfClientModel()
			: base(typeof(Contract))
		{
		}

		public WcfClientModel(string endpointName) : this()
		{
			EndpointName = endpointName;
		}
	}
}

