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

namespace Castle.Facilities.Remoting
{
	using System;
	using System.Runtime.Remoting;
	using Castle.Core;

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
		Component,
		RecoverableComponent
	}

	/// <summary>
	/// Inspects the model looking for remote component configuration. If found, 
	/// do the component Remoting configuration.
	/// </summary>
	public class RemotingInspector : IContributeComponentModelConstruction
	{
		private readonly RemotingRegistry remoteRegistry;
		private readonly RemotingRegistry localRegistry;
		private readonly ITypeConverter converter;
		private readonly String baseUri;
		private readonly bool isServer, isClient;

	    private const string UriExtension = ".rem";

		/// <summary>
		/// Initializes a new instance of the <see cref="RemotingInspector"/> class.
		/// </summary>
		/// <param name="converter">The converter.</param>
		/// <param name="isServer">if set to <c>true</c> is a server.</param>
		/// <param name="isClient">if set to <c>true</c> is a client.</param>
		/// <param name="baseUri">The base URI.</param>
		/// <param name="remoteRegistry">The remote registry.</param>
		/// <param name="localRegistry">The local registry.</param>
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

			RemotingStrategy server = RemotingStrategy.None;
			RemotingStrategy client = RemotingStrategy.None;

			if (remoteserverAttValue == null && remoteclientAttValue == null)
			{
				return;
			}

			if (remoteserverAttValue != null)
			{
				server = (RemotingStrategy) 
					converter.PerformConversion(remoteserverAttValue, typeof(RemotingStrategy));
			}

			if (remoteclientAttValue != null)
			{
				client = (RemotingStrategy) 
					converter.PerformConversion(remoteclientAttValue, typeof(RemotingStrategy));
			}

			DoSemanticCheck(server, model, client);

			ConfigureServerComponent(server, model.Implementation, model);

			ConfigureClientComponent(client, model.Service, model);
		}

		private void ConfigureServerComponent(RemotingStrategy server, Type type, ComponentModel model)
		{
			if (server == RemotingStrategy.None) return;

			String uri = ConstructServerURI(server, model);
			
			switch (server)
			{
				case RemotingStrategy.Singleton:
				{
					CheckURIIsNotNull(uri, model.Name);

					RemotingConfiguration.RegisterWellKnownServiceType(type, uri, WellKnownObjectMode.Singleton);
					
					break;
				}
				case RemotingStrategy.SingleCall:
				{
					CheckURIIsNotNull(uri, model.Name);

					RemotingConfiguration.RegisterWellKnownServiceType(type, uri, WellKnownObjectMode.SingleCall);
					
					break;
				}
				case RemotingStrategy.ClientActivated:
				{
					RemotingConfiguration.RegisterActivatedServiceType(type);
					
					break;
				}
				case RemotingStrategy.Component:
				{
					localRegistry.AddComponentEntry(model);
					
					break;
				}
				case RemotingStrategy.RecoverableComponent:
				{
					CheckURIIsNotNull(uri, model.Name);

					ValidateLifeStyle(model);

					localRegistry.AddComponentEntry(model);
					
					model.ExtendedProperties.Add("remoting.uri", uri);
					model.ExtendedProperties.Add("remoting.afinity", true);
						
					model.CustomComponentActivator = typeof(RemoteMarshallerActivator);
					
					break;
				}
			}
		}

		private static void ValidateLifeStyle(ComponentModel model)
		{
			if (model.LifestyleType != LifestyleType.Singleton && 
			    model.LifestyleType != LifestyleType.Undefined)
			{
				throw new FacilityException(String.Format("Component {0} is marked as a 'RecoverableComponent' but is using a lifestyle " + 
					"different than Singleton. Unfortunately Singleton is the only lifestyle supported for this of remoting component configuration", model.Name));
			}
		}

		private void ConfigureClientComponent(RemotingStrategy client, Type type, ComponentModel model)
		{
			if (client == RemotingStrategy.None) return;

			ResetDependencies(model);

			String uri = ConstructClientURI(client, model);

			bool skipRemotingRegistration = Convert.ToBoolean(model.Configuration.Attributes["skipRemotingRegistration"]);
			
			switch (client)
			{
				case RemotingStrategy.Singleton:
				case RemotingStrategy.SingleCall:
				{
					if (!skipRemotingRegistration)
					{
						RemotingConfiguration.RegisterWellKnownClientType(type, uri);
					}

					model.ExtendedProperties.Add("remoting.uri", uri);
					model.CustomComponentActivator = typeof(RemoteActivator);
					
					break;
				}
				case RemotingStrategy.ClientActivated:
				{
					CheckHasBaseURI();

					if (!skipRemotingRegistration)
					{
						RemotingConfiguration.RegisterActivatedClientType(type, baseUri);
					}

					model.ExtendedProperties.Add("remoting.appuri", baseUri);
					model.CustomComponentActivator = typeof(RemoteClientActivatedActivator);
			
					break;
				}
				case RemotingStrategy.Component:
				{
					model.ExtendedProperties["remoting.remoteregistry"] = remoteRegistry;
					model.CustomComponentActivator = typeof(RemoteActivatorThroughRegistry);
					
					break;
				}
				case RemotingStrategy.RecoverableComponent:
				{
					CheckHasBaseURI();
						
					String remoteUri = SetUriExtensionIfNeeded(uri);
						
					model.ExtendedProperties.Add("remoting.uri", remoteUri);
					model.ExtendedProperties.Add("remoting.remoteregistry", remoteRegistry);
					model.CustomComponentActivator = typeof(RemoteActivatorThroughConnector);
					
					break;
				}
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

		private String ConstructClientURI(RemotingStrategy client, ComponentModel model)
		{
			if (client == RemotingStrategy.ClientActivated) return null;

			String value = model.Configuration.Attributes["uri"];

			String uriText;

			if (client != RemotingStrategy.None && baseUri != null && value == null)
			{
				uriText = BuildUri(model);
			}
			else
			{
				uriText = value;
			}

			return uriText;
		}

		private String BuildUri(ComponentModel model)
		{
			String cpntUri;

			// if the remoted component is a generic component then ensure a unique uri is built 
			// for the requested service
			if (model.Service.IsGenericType)
			{
				cpntUri = model.Service.Name;
				foreach (Type genericArgument in model.Service.GetGenericArguments())
				{
					cpntUri += genericArgument.FullName;
				}
			}
			else
			{
				cpntUri = model.Name; //default;
			}

			String uriText;

			if (baseUri != null)
			{
				if (baseUri.EndsWith("/"))
				{
					uriText = SetUriExtensionIfNeeded(String.Format("{0}{1}", baseUri, cpntUri));
				}
				else
				{
					uriText = SetUriExtensionIfNeeded(String.Format("{0}/{1}", baseUri, cpntUri));
				}
			}
			else
			{
				uriText = SetUriExtensionIfNeeded(cpntUri);
			}

			return uriText;
		}

		private static string SetUriExtensionIfNeeded(string uri)
		{
			if (!uri.EndsWith(".rem"))
			{
				return uri + UriExtension;
			}

			return uri;
		}

		private String ConstructServerURI(RemotingStrategy server, ComponentModel model)
		{
			if (server == RemotingStrategy.ClientActivated) return null;

			String value = model.Configuration.Attributes["uri"];

			String uriText;

			if (value == null)
			{
				uriText = BuildUri(model);
			}
			else
			{
				uriText = SetUriExtensionIfNeeded(value);
			}

			return uriText;
		}

		/// <summary>
		/// Client components are not created by the container
		/// so there's no point collecting constructor dependencies
		/// </summary>
		/// <param name="model"></param>
		private void ResetDependencies(ComponentModel model)
		{
			model.Dependencies.Clear();
			model.Constructors.Clear();
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
