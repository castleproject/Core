// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
	using System.ServiceModel.Description;
	using Castle.Facilities.WcfIntegration.Behaviors;

	public static class WcfEndpoint
	{
		public static ServiceEndpointModel FromEndpoint(ServiceEndpoint endpoint)
		{
			return new ContractEndpointModel().FromEndpoint(endpoint);
		}

		public static ConfigurationEndpointModel FromConfiguration(string endpointName)
		{
			return new ContractEndpointModel().FromConfiguration(endpointName);
		}

		public static BindingEndpointModel BoundTo(Binding binding)
		{
			return new ContractEndpointModel().BoundTo(binding);
		}

		public static BindingAddressEndpointModel At(string address)
		{
			return new ContractEndpointModel().At(address);
		}

		public static BindingAddressEndpointModel At(Uri address)
		{
			return new ContractEndpointModel().At(address);
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
	}

	#region Nested Class: WcfEndpointBase

	public abstract class WcfEndpointBase : IWcfEndpoint
	{
		private Type contract;
		private ICollection<IWcfExtension> extensions;

		protected WcfEndpointBase(Type contract)
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

		public ICollection<IWcfExtension> Extensions
		{
			get
			{
				if (extensions == null)
				{
					extensions = new List<IWcfExtension>();
				}
				return extensions;
			}
		}

		void IWcfEndpoint.Accept(IWcfEndpointVisitor visitor)
		{
			Accept(visitor);
		}

		protected abstract void Accept(IWcfEndpointVisitor visitor);

		#endregion
	}

	public abstract class WcfEndpointBase<T> : WcfEndpointBase
		where T : WcfEndpointBase<T>
	{
		protected WcfEndpointBase(Type contract)
			: base(contract)
		{
		}

		public T AddExtensions(params object[] extensions)
		{
			foreach (object extension in extensions)
			{
				Extensions.Add(WcfExplicitExtension.CreateFrom(extension));
			}
			return (T)this;
		}

		#region Logging

		public T LogMessages()
		{
			return AddExtensions(typeof(LogMessageEndpointBehavior));
		}

		public T LogMessages<F>()
			where F : IFormatProvider, new()
		{
			return LogMessages<F>(null);
		}

		public T LogMessages<F>(string format)
			where F : IFormatProvider, new() 
		{
			return LogMessages(new F(), format);
		}

		public T LogMessages(IFormatProvider formatter)
		{
			return LogMessages(formatter, null);
		}

		public T LogMessages(IFormatProvider formatter, string format)
		{
			return LogMessages().AddExtensions(new LogMessageFormat(formatter, format));
		}

		public T LogMessages(string format)
		{
			return LogMessages().AddExtensions(new LogMessageFormat(format));
		}

		#endregion
	}

	#endregion

	#region Nested Class: ContractModel

	public class ContractEndpointModel : WcfEndpointBase<ContractEndpointModel>
	{
		internal ContractEndpointModel()
			: this(null)
		{
		}

		internal ContractEndpointModel(Type contract)
			: base(contract)
		{
		}

		public ServiceEndpointModel FromEndpoint(ServiceEndpoint endpoint)
		{
			if (endpoint == null)
			{
				throw new ArgumentNullException("endpoint");
			}

			return new ServiceEndpointModel(Contract, endpoint);
		}

		public ConfigurationEndpointModel FromConfiguration(string endpointName)
		{
			if (string.IsNullOrEmpty(endpointName))
			{
				throw new ArgumentException("endpointName cannot be nul or empty");
			}
			return new ConfigurationEndpointModel(Contract, endpointName);
		}

		public BindingEndpointModel BoundTo(Binding binding)
		{
			if (binding == null)
			{
				throw new ArgumentNullException("binding");
			}
			return new BindingEndpointModel(Contract, binding);
		}

		public BindingAddressEndpointModel At(string address)
		{
			return new BindingEndpointModel(Contract, null).At(address);
		}

		public BindingAddressEndpointModel At(Uri address)
		{
			return new BindingEndpointModel(Contract, null).At(address);
		}

		protected override void Accept(IWcfEndpointVisitor visitor)
		{
			visitor.VisitContractEndpoint(this);
		}
	}

	#endregion

	#region Nested Class: ServiceEndpointModel

	public class ServiceEndpointModel : WcfEndpointBase<ServiceEndpointModel>
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
			visitor.VisitServiceEndpoint(this);
		}
	}

	#endregion

	#region Nested Class: ConfigurationEndpointModel

	public class ConfigurationEndpointModel : WcfEndpointBase<ConfigurationEndpointModel>
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
			visitor.VisitConfigurationEndpoint(this);
		}
	}

	#endregion

	#region Nested Class: BindingEndpointModel

	public class BindingEndpointModel : WcfEndpointBase<BindingEndpointModel>
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
			visitor.VisitBindingEndpoint(this);
		}
	}

	#endregion

	#region Nested Class: BindingAddressEndpointModel

	public class BindingAddressEndpointModel : WcfEndpointBase<BindingAddressEndpointModel>
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
			visitor.VisitBindingAddressEndpoint(this);
		}
	}

	#endregion
}

