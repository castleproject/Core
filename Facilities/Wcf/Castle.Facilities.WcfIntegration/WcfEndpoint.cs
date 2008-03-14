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
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class WcfEndpoint
    {
        private string address;
        private Binding binding;
        private Type contract;
        private EndpointAddress endpointAddress;
        private string endpointName;
        private string via;

        public WcfEndpoint()
        {
        }

        public WcfEndpoint(Type contract)
        {
            this.contract = contract;
        }

        public string Address
        {
			get { return address; }
			set { address = value; }
        }

        public Binding Binding
        {
			get { return binding; }
			set { binding = value; }
        }

        public Type Contract
        {
			get { return contract; }
			set { contract = value; }
        }

        public EndpointAddress EndpointAddress
        {
			get { return endpointAddress; }
			set { endpointAddress = value; }
        }

        public string EndpointName
        {
			get { return endpointName; }
			set { endpointName = value; }
        }

        public string Via
        {
			get { return via; }
			set { via = value; }
        }
    }

	public class WcfEndpoint<Contract> : WcfEndpoint
	{
		public WcfEndpoint() : base(typeof(Contract))
		{
		}
	}
}

