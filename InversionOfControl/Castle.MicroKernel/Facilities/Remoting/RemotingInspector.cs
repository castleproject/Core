// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Runtime.Remoting;

	using Castle.Model;

	using Castle.MicroKernel;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;

	enum RemotingStrategy
	{
		None,
		Singleton,
		SingleCall,
		ClientActivated,
		Component
	}

	public class RemotingInspector : IContributeComponentModelConstruction
	{
		private readonly RemotingRegistry remoteRegistry;
		private readonly RemotingRegistry localRegistry;
		private readonly ITypeConverter converter;
		private readonly bool isServer;
		private readonly bool isClient;
		private readonly String baseUri;

		public RemotingInspector(ITypeConverter converter, bool isServer, bool isClient, 
			String baseUri, RemotingRegistry remoteRegistry, RemotingRegistry localRegistry)
		{
			this.converter = converter;
			this.isServer = isServer;
			this.isClient = isClient;
			this.baseUri = baseUri;
			this.remoteRegistry = remoteRegistry;
			this.localRegistry = localRegistry;
		}

		public void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (model.Configuration == null) return;

			String remoteserverAttValue = model.Configuration.Attributes["remoteserver"];
			String remoteclientAttValue = model.Configuration.Attributes["remoteclient"];
			// String sponsorIdAttValue = model.Configuration.Attributes["sponsorId"];

			RemotingStrategy server = RemotingStrategy.None;
			RemotingStrategy client = RemotingStrategy.None;

			if (remoteserverAttValue == null && remoteclientAttValue == null)
			{
				return;
			}

			if (remoteserverAttValue != null)
			{
				server = (RemotingStrategy) 
					converter.PerformConversion( remoteserverAttValue, typeof(RemotingStrategy) );
			}

			if (remoteclientAttValue != null)
			{
				client = (RemotingStrategy) 
					converter.PerformConversion( remoteclientAttValue, typeof(RemotingStrategy) );
			}

			DoSemanticCheck(server, model, client);

			ConfigureServerComponent(server, model.Implementation, model);

			ConfigureClientComponent(client, model.Service, model);
		}

		private void ConfigureServerComponent(RemotingStrategy server, Type type, ComponentModel model)
		{
			if (server == RemotingStrategy.None) return;

			String uri = ConstructServerURI(server, model.Name, model);

			if (server == RemotingStrategy.Singleton)
			{
				CheckURIIsNotNull(uri, model.Name);

				RemotingConfiguration.RegisterWellKnownServiceType(type, uri, WellKnownObjectMode.Singleton);
			}
			else if (server == RemotingStrategy.SingleCall)
			{
				CheckURIIsNotNull(uri, model.Name);

				RemotingConfiguration.RegisterWellKnownServiceType(type, uri, WellKnownObjectMode.SingleCall);
			}
			else if (server == RemotingStrategy.ClientActivated)
			{
				RemotingConfiguration.RegisterActivatedServiceType(type);
			}
			else if (server == RemotingStrategy.Component)
			{
				localRegistry.AddComponentEntry( model );
			}
		}

		private void ConfigureClientComponent(RemotingStrategy client, Type type, ComponentModel model)
		{
			if (client == RemotingStrategy.None) return;

			String uri = ConstructClientURI(client, model.Name, model);

			if (client == RemotingStrategy.Singleton || client == RemotingStrategy.SingleCall)
			{
				RemotingConfiguration.RegisterWellKnownClientType(type, uri);

				model.ExtendedProperties.Add("remoting.uri", uri);
				model.CustomComponentActivator = typeof(RemoteActivator);
			}
			else if (client == RemotingStrategy.ClientActivated)
			{
				CheckHasBaseURI();

				RemotingConfiguration.RegisterActivatedClientType(type, baseUri);

				model.ExtendedProperties.Add("remoting.appuri", baseUri);
				model.CustomComponentActivator = typeof(RemoteClientActivatedActivator);
			}
			else if (client == RemotingStrategy.Component)
			{
				model.ExtendedProperties.Add("remoting.remoteregistry", remoteRegistry);
				model.CustomComponentActivator = typeof(RemoteActivatorThroughRegistry);
			}
		}

		private void DoSemanticCheck(RemotingStrategy server, ComponentModel model, RemotingStrategy client)
		{
			if (server != RemotingStrategy.None && client != RemotingStrategy.None)
			{
				String message = String.Format("Component {0} cannot be a remote server and a client "+ 
					"at the same time", model.Name);

				throw new FacilityException(message);
			}

			if (server == RemotingStrategy.Component && !isServer)
			{
				String message = String.Format("Component {0} was marked with remoteserver='component', " + 
					"but you must enable the remoting facility with isServer='true' to serve components this way", model.Name);

				throw new FacilityException(message);
			}
	
			if (client == RemotingStrategy.Component && !isClient)
			{
				String message = String.Format("Component {0} was marked with remoteserver='component', " + 
					"but you must enable the remoting facility with isServer='true' to serve components this way", model.Name);

				throw new FacilityException(message);
			}
		}

		private String ConstructClientURI(RemotingStrategy client, String componentId, ComponentModel model)
		{
			if (client == RemotingStrategy.ClientActivated) return null;

			String value = model.Configuration.Attributes["uri"];

			String uriText = null;

			if (client != RemotingStrategy.None && baseUri != null && value == null)
			{
				if (baseUri.EndsWith("/"))
					uriText = String.Format("{0}{1}", baseUri, componentId);
				else
					uriText = String.Format("{0}/{1}", baseUri, componentId);
			}
			else
			{
				uriText = value;
			}

			return uriText;
		}

		private String ConstructServerURI(RemotingStrategy server, String componentId, ComponentModel model)
		{
			if (server == RemotingStrategy.ClientActivated) return null;

			String value = model.Configuration.Attributes["uri"];

			String uriText = null;

			if (value == null)
			{
				value = componentId;
			}
			else
			{
				uriText = value;
			}

			return uriText;
		}

		private void CheckHasBaseURI()
		{
			if (baseUri == null)
			{
				String message = "baseUri must be defined in order to use client activated objects";
				throw new FacilityException(message);
			}
		}

		private void CheckURIIsNotNull(String uri, String componentId)
		{
			if (uri == null)
			{
				String message = String.Format("Could not obtain (or infer) " + 
					"URI for remote component {0}", componentId);

				throw new FacilityException(message);
			}
		}
	}
}