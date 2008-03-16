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
using Castle.Facilities.WcfIntegration.Internal;

namespace Castle.Facilities.WcfIntegration
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
	using System.ServiceModel.Description;

	public abstract class WcfEndpoint : IWcfEndpoint
	{
		private Type contract;

		protected WcfEndpoint(Type contract)
		{
			this.contract = contract;
		}

		#region IWcfEndpoint Members

		public Type Contract
		{
			get { return contract; }
		}

		Type IWcfEndpoint.Contract
		{
			get { return contract; }
			set { contract = value; }
		}

		void IWcfEndpoint.Accept(IWcfEndpointVisitor visitor)
		{
			Accept(visitor);
		}

		protected abstract void Accept(IWcfEndpointVisitor visitor);

		#endregion

		#region WcfEndpoint Builders

		public static ServiceEndpointModel FromEndpoint(ServiceEndpoint endpoint)
		{
			return new ContractEndpointModel().FromEndpoint(endpoint);
		}

		public static ConfigurationEndpointModel FromConfiguration(string endpointName)
		{
			return new ContractEndpointModel().FromConfiguration(endpointName);
		}

		public static BindingEndpointModel WithBinding(Binding binding)
		{
			return new ContractEndpointModel().WithBinding(binding);
		}

		public static ContractEndpointModel ForContract(Type contract)
		{
			return new ContractEndpointModel(contract);
		}

		public static ContractEndpointModel ForContract<Contract>()
			where Contract : class
		{
			return ForContract(typeof(Contract));
		}

		#endregion
	}

	#region Nested Class: ContractModel

	public class ContractEndpointModel
	{
		private readonly Type contract;

		internal ContractEndpointModel()
		{
		}

		internal ContractEndpointModel(Type contract)
		{
			this.contract = contract;
		}

		public ServiceEndpointModel FromEndpoint(ServiceEndpoint endpoint)
		{
			if (endpoint == null)
			{
				throw new ArgumentNullException("endpoint");
			}

			return new ServiceEndpointModel(contract, endpoint);
		}

		public ConfigurationEndpointModel FromConfiguration(string endpointName)
		{
			if (string.IsNullOrEmpty(endpointName))
			{
				throw new ArgumentException("endpointName cannot be nul or empty");
			}
			return new ConfigurationEndpointModel(contract, endpointName);
		}

		public BindingEndpointModel WithBinding(Binding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			return new BindingEndpointModel(contract, binding);
		}
	}

	#endregion

	#region Nested Class: ServiceEndpointModel

	public class ServiceEndpointModel : WcfEndpoint
	{
		private readonly ServiceEndpoint endpoint;

		internal ServiceEndpointModel(Type contract, ServiceEndpoint endpoint)
			: base(contract)
		{
			this.endpoint = endpoint;
		}

		public ServiceEndpoint ServiceEndpoint
		{
			get { return endpoint; }
		}

		protected override void Accept(IWcfEndpointVisitor visitor)
		{
			visitor.VisitServiceEndpointModel(this);
		}
	}

	#endregion

	#region Nested Class: ConfigurationEndpointModel

	public class ConfigurationEndpointModel : WcfEndpoint
	{
		private readonly string endpointName;

		internal ConfigurationEndpointModel(Type contract, string endpointName)
			: base(contract)
		{
			this.endpointName = endpointName;
		}

		public string EndpointName
		{
			get { return endpointName; }
		}

		protected override void Accept(IWcfEndpointVisitor visitor)
		{
			visitor.VisitConfigurationEndpointModel(this);
		}
	}

	#endregion

	#region Nested Class: BindingEndpointModel

	public class BindingEndpointModel : WcfEndpoint
	{
		private readonly Binding binding;

		internal BindingEndpointModel(Type contract, Binding binding)
			: base(contract)
		{
			this.binding = binding;
		}

		public Binding Binding
		{
			get { return binding; }
		}

		public BindingAddressEndpointModel At(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentException("address cannot be null or empty");
			}
			return new BindingAddressEndpointModel(Contract, Binding, address);
		}

		public BindingAddressEndpointModel At(Uri address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return new BindingAddressEndpointModel(Contract, Binding, address.AbsoluteUri);
		}

		public BindingAddressEndpointModel At(EndpointAddress address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			return new BindingAddressEndpointModel(Contract, Binding, address);
		}

		protected override void Accept(IWcfEndpointVisitor visitor)
		{
			visitor.VisitBindingEndpointModel(this);
		}
	}

	#endregion

	#region Nested Class: BindingAddressEndpointModel

	public class BindingAddressEndpointModel : WcfEndpoint
	{
		private readonly Binding binding;
		private readonly string address;
		private readonly EndpointAddress endpointAddress;
		private string via;

		internal BindingAddressEndpointModel(Type contract, Binding binding,
											 string address)
			: base(contract)
		{
			this.binding = binding;
			this.address = address;
		}

		internal BindingAddressEndpointModel(Type contract, Binding binding,
											 EndpointAddress address)
			: base(contract)
		{
			this.binding = binding;
			this.endpointAddress = address;
		}

		public Binding Binding
		{
			get { return binding; }
		}

		public string Address
		{
			get { return address ?? endpointAddress.Uri.AbsoluteUri; }
		}

		public EndpointAddress EndpointAddress
		{
			get { return endpointAddress; }
		}

		public Uri ViaAddress
		{
			get { return new Uri(via, UriKind.Absolute); }
		}

		public bool HasViaAddress
		{
			get { return !string.IsNullOrEmpty(via); }
		}

		public BindingAddressEndpointModel Via(string physicalAddress)
		{
			via = physicalAddress;
			return this;
		}

		protected override void Accept(IWcfEndpointVisitor visitor)
		{
			visitor.VisitBindingAddressEndpointModel(this);
		}
	}

	#endregion
}

