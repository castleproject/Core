// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions
{
	using System;
	using System.IO;
	using System.Collections;
	using System.Configuration;
	using System.Security.Policy;

	using Castle.ManagementExtensions.Default;

	/// <summary>
	/// Summary description for MServerFactory.
	/// </summary>
	public sealed class MServerFactory
	{
		public static readonly String CustomServerConfigurationKey = "MServerFactory";

		private static readonly Hashtable domains = Hashtable.Synchronized(
			new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default));

		private MServerFactory()
		{
		}

		/// <summary>
		/// Creates a <see cref="MServer"/> instance.
		/// </summary>
		/// <param name="createNewAppDomain">true if MServerFactory should create a dedicated
		/// AppDomain for the <see cref="MServer"/> instance.</param>
		/// <returns>A <see cref="MServer"/> instance.</returns>
		public static MServer CreateServer(bool createNewAppDomain)
		{
			return CreateServer(String.Empty, createNewAppDomain);
		}

		/// <summary>
		/// Creates a <see cref="MServer"/> instance.
		/// </summary>
		/// <param name="domain">The domain name</param>
		/// <param name="createNewAppDomain">true if MServerFactory should create a dedicated
		/// AppDomain for the <see cref="MServer"/> instance.</param>
		/// <returns>A <see cref="MServer"/> instance.</returns>
		public static MServer CreateServer(String domain, bool createNewAppDomain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}

			if (domains.Contains(domain))
			{
				throw new DomainAlreadyExistsException(domain);
			}

			String typeName = ConfigurationSettings.AppSettings[CustomServerConfigurationKey];
			Type serverType = null;

			if (typeName != null && typeName != String.Empty)
			{
				// TODO: Allow custom servers..
			}
			else
			{
				serverType = typeof(MDefaultServer);
			}

			if (createNewAppDomain)
			{
				// Lets create a seperated AppDomain for this server
				
				AppDomain currentDomain = AppDomain.CurrentDomain;

				String baseDir = new FileInfo(currentDomain.BaseDirectory).FullName;

				String configFile =  String.Format(
					"{0}/{1}.config", 
					baseDir, domain); 

				AppDomainSetup setup = new AppDomainSetup();

				setup.ApplicationName = domain;
				setup.ApplicationBase = currentDomain.SetupInformation.ApplicationBase;
				setup.PrivateBinPath = currentDomain.SetupInformation.PrivateBinPath;
				setup.ConfigurationFile = configFile;
				// setup.ShadowCopyFiles = "false";
				// setup.ShadowCopyDirectories = appBase;

				Evidence baseEvidence = currentDomain.Evidence;
				Evidence evidence = new Evidence(baseEvidence);

				AppDomain newDomain = AppDomain.CreateDomain(
					domain, evidence, setup);

				object remoteInstance = newDomain.CreateInstanceAndUnwrap(
					serverType.Assembly.FullName, serverType.FullName);

				// Register the domain

				domains.Add(domain, new DomainInfo( domain, remoteInstance as MServer, newDomain) );

				// As this already method "unwraps" the target object, its safe
				// to return it - in an "wrapped" object we should invoke the 
				// class's constructor

				return (MServer) remoteInstance;
			}
			else
			{
				object localInstance = Activator.CreateInstance(serverType);

				// Register the domain

				domains.Add(domain, new DomainInfo( domain, localInstance as MServer ) );

				return (MServer) localInstance;
			}
		}

		/// <summary>
		/// Releases a <see cref="MServer"/> instance. This method
		/// accepts a null argument.
		/// </summary>
		/// <param name="server">The <see cref="MServer"/> instance to be released.</param>
		public static void Release(MServer server)
		{
			if (server != null)
			{
				foreach(DomainInfo info in domains.Values)
				{
					if (info.Server == server)
					{
						domains.Remove( info.Name );

						if (info.DedicatedDomain != null)
						{
							AppDomain.Unload( info.DedicatedDomain );
						}

						break;
					}
				}
			}
		}
	}

	/// <summary>
	/// Holds registered domains information.
	/// </summary>
	class DomainInfo
	{
		public String Name;
		public AppDomain DedicatedDomain;
		public MServer Server;

		private DomainInfo(String name)
		{
			this.Name = name;
		}

		public DomainInfo(String name, MServer Server) : this(name)
		{
			this.Server = Server;
		}

		public DomainInfo(String name, MServer Server, AppDomain domain) : this(name, Server)
		{
			this.DedicatedDomain = domain;
		}
	}
}
