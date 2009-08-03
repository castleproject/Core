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
	using System.ServiceModel.Description;
	using Castle.MicroKernel;

	internal class ServiceEndpointExtensions : IWcfExtensionVisitor
	{
		private readonly ServiceEndpoint endpoint;
		private readonly bool withContract;
		private readonly IKernel kernel;
		private IWcfBurden burden;

		public ServiceEndpointExtensions(ServiceEndpoint endpoint, bool withContract, IKernel kernel)
		{
			this.endpoint = endpoint;
			this.withContract = withContract;
			this.kernel = kernel;
		}

		public ServiceEndpointExtensions Install(ICollection<IWcfExtension> extensions, IWcfBurden burden)
		{
			this.burden = burden;

			foreach (var extension in extensions)
			{
				extension.Accept(this);
			}

			return this;
		}

		public ServiceEndpointExtensions Install(IWcfBurden burden, params IWcfExtension[] extensions)
		{
			return Install((ICollection<IWcfExtension>)extensions, burden);
		}

		#region IWcfExtensionVisitor Members

		void IWcfExtensionVisitor.VisitServiceExtension(IWcfServiceExtension extension)
		{
		}

		void IWcfExtensionVisitor.VisitChannelExtension(IWcfChannelExtension extension)
		{
		}

		void IWcfExtensionVisitor.VisitEndpointExtension(IWcfEndpointExtension extension)
		{
			extension.Install(endpoint, withContract, kernel, burden);
		}

		#endregion
	}
}
