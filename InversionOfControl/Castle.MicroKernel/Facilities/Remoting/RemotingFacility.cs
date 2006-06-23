// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Remoting
{
	using System;
	using System.IO;
	using System.Configuration;
	using System.Runtime.Remoting;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;

	public class RemotingFacility : AbstractFacility
	{
		private ITypeConverter converter;

		private bool isServer, isClient;
		private bool disconnectLocalRegistry;
		
		/// <summary>
		/// Used for client side (Expand explanation)
		/// </summary>
		private String baseUri;

		/// <summary>
		/// Used for server side. 
		/// Holds the local registry
		/// </summary>
		private RemotingRegistry localRegistry;

		/// <summary>
		/// Used for client side. 
		/// Holds a remote proxy to the server registry
		/// </summary>
		private RemotingRegistry remoteRegistry;

		/// <summary>
		/// Constructs a RemotingFacility
		/// </summary>
		public RemotingFacility()
		{
		}

		protected override void Init()
		{
			ObtainConverter();

			SetUpRemotingConfiguration();

			baseUri = FacilityConfig.Attributes["baseUri"];

			String isServerAttValue = FacilityConfig.Attributes["isServer"];
			String isClientAttValue = FacilityConfig.Attributes["isClient"];

			if ("true".Equals(isServerAttValue))
			{
				isServer = true;
				ConfigureServerFacility();
			}

			if ("true".Equals(isClientAttValue))
			{
				isClient = true;
				ConfigureClientFacility();
			}

			Kernel.ComponentModelBuilder.AddContributor(
				new RemotingInspector(converter, isServer, isClient, baseUri, remoteRegistry, localRegistry));
		}

		private void SetUpRemotingConfiguration()
		{
			String configurationFile = FacilityConfig.Attributes["remotingConfigurationFile"];

			if (configurationFile == null) return;

			if (!Path.IsPathRooted(configurationFile))
			{
				configurationFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configurationFile);
			}

			if (!File.Exists(configurationFile))
			{
				String message = String.Format("Remoting configuration file '{0}' does not exist", configurationFile);
				throw new ConfigurationException(message);
			}

			RemotingConfiguration.Configure(configurationFile);
		}

		private void ConfigureServerFacility()
		{
			Kernel.AddComponent("remoting.registry", typeof(RemotingRegistry));

			localRegistry = (RemotingRegistry) Kernel[ typeof(RemotingRegistry) ];

			String kernelUri = FacilityConfig.Attributes["registryUri"];

			if (kernelUri == null || kernelUri.Length == 0)
			{
				String message = "When the remote facility is configured as " + 
					"server you must supply the URI for the component registry using the attribute 'registryUri'";
				throw new ConfigurationException(message);
			}

			RemotingServices.Marshal(localRegistry, kernelUri, typeof(RemotingRegistry));

			disconnectLocalRegistry = true;
		}

		private void ConfigureClientFacility()
		{
			String remoteKernelUri = FacilityConfig.Attributes["remoteKernelUri"];

			if (remoteKernelUri == null || remoteKernelUri.Length == 0)
			{
				String message = "When the remote facility is configured as " + 
					"client you must supply the URI for the kernel using the attribute 'remoteKernelUri'";
				throw new ConfigurationException(message);
			}

			remoteRegistry = (RemotingRegistry) 
				RemotingServices.Connect(typeof(RemotingRegistry), remoteKernelUri);
		}

		private void ObtainConverter()
		{
			converter = (ITypeConverter) Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
		}

		public override void Dispose()
		{
			if (disconnectLocalRegistry) RemotingServices.Disconnect(localRegistry);

			base.Dispose();
		}
	}
}
