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

namespace Castle.Facilities.WcfIntegration.Rest
{
	using System;
	using System.ServiceModel;
	using System.ServiceModel.Channels;
	using Castle.Core;
	using Castle.MicroKernel;

	/// <summary>
	/// Implementation of <see cref="IServiceHostBuilder{M}"/>. for restful services.
	/// </summary>
	public class RestServiceHostBuilder : AbstractServiceHostBuilder<RestServiceModel>
	{
		/// <summary>
		/// Constructs a new <see cref="RestServiceHostBuilder"/>.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public RestServiceHostBuilder(IKernel kernel)
			: base(kernel)
		{
		}

		#region AbstractServiceHostBuilder Members

		protected override Binding GetDefaultBinding(ServiceHost serviceHost, string address)
		{
			WebHttpBinding binding = new WebHttpBinding();
			if (address.StartsWith(Uri.UriSchemeHttps))
			{
				binding.Security.Mode = WebHttpSecurityMode.Transport;
			}
			return binding;
		}

		protected override ServiceHost CreateServiceHost(ComponentModel model, 
			                                             RestServiceModel serviceModel,
														 params Uri[] baseAddresses)
		{
			return CreateRestServiceHost(model.Implementation, 
				GetEffectiveBaseAddresses(serviceModel, baseAddresses));
		}

		protected override ServiceHost CreateServiceHost(ComponentModel model, 
			                                             params Uri[] baseAddresses)
		{
			return CreateRestServiceHost(model.Implementation, baseAddresses);
		}

		protected override ServiceHost CreateServiceHost(Type serviceType, 
			                                             params Uri[] baseAddresses)
		{
			return CreateRestServiceHost(serviceType, baseAddresses);
		}

		#endregion

		private RestServiceHost CreateRestServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			return new RestServiceHost(serviceType, baseAddresses);
		}
	}
}
