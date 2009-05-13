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

namespace Castle.Facilities.DynamicLoader
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Security.Policy;
	using System.Web;
	using Castle.Core.Configuration;
	using Castle.Core.Logging;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// DynamicLoader facility.
	/// </summary>
	public class DynamicLoaderFacility : MarshalByRefObject, IFacility, IDisposable
	{
		private readonly DynamicLoaderRegistry registry = new DynamicLoaderRegistry();

		private ILogger log = NullLogger.Instance;

		private bool isWeb;

		private IKernel kernel;
		private IConfiguration facilityConfig;

		/// <summary>
		/// Initializes the facility.
		/// </summary>
		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			this.kernel = kernel;
			this.facilityConfig = facilityConfig;

			this.Init();
		}

		/// <summary>
		/// Terminates the facility.
		/// </summary>
		public void Terminate()
		{
			Dispose(true);

			kernel = null;
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="DynamicLoaderFacility"/> is reclaimed by garbage collection.
		/// </summary>
		~DynamicLoaderFacility()
		{
			Dispose(false);
		}

		/// <summary>
		/// Inits this instance.
		/// </summary>
		protected void Init()
		{
			if (kernel.HasComponent(typeof(ILoggerFactory)))
				log = ((ILoggerFactory) kernel.Resolve(typeof(ILoggerFactory), new Hashtable())).Create(GetType());

			log.Info("DynamicLoader is being initialized");

			kernel.ComponentModelBuilder.AddContributor(new DynamicLoaderInspector(registry));

			isWeb = IsTrue(facilityConfig.Attributes["isWeb"]);

			foreach(IConfiguration cfg in facilityConfig.Children)
			{
				switch(cfg.Name)
				{
					case "domain":
						CreateAppDomain(cfg);
						break;
					default:
						throw new FacilityException("Unrecognized configuration node: " + cfg.Name);
				}
			}
		}

		private AppDomain CreateAppDomain(IConfiguration domainNode)
		{
			IConfiguration cfg = domainNode.Children["config"];

			string domainId = domainNode.Attributes["id"];

			AppDomainSetup currentSetup = AppDomain.CurrentDomain.SetupInformation;
			AppDomainSetup setup = new AppDomainSetup();

			setup.ApplicationName = domainNode.Attributes["applicationName"];
			setup.ApplicationBase = NormalizeDirectoryPath(GetChildNodeValue(cfg, "applicationBase"));
			setup.ConfigurationFile = GetChildNodeValue(cfg, "configurationFile");

			setup.PrivateBinPath = GetChildNodeValue(cfg, "privateBinPath");
			if (!IsEmpty(setup.PrivateBinPath))
			{
				setup.PrivateBinPathProbe = Boolean.TrueString;
			}

			setup.ShadowCopyFiles = domainNode.Attributes["shadowCopyFiles"];

			if (IsTrue(setup.ShadowCopyFiles))
			{
				setup.ShadowCopyDirectories = null;

				setup.CachePath = cfg.Attributes["cachePath"];
				if (IsEmpty(setup.CachePath))
				{
					// fix for ASP.NET:
					// http://weblogs.asp.net/hernandl/archive/2004/10/28/appdomainshcopy.aspx
					setup.CachePath = currentSetup.CachePath;
				}
			}

			log.Info("Creating AppDomain '{0}'", setup.ApplicationName);

			Evidence evidence = AppDomain.CurrentDomain.Evidence;

			AppDomain appDomain = AppDomain.CreateDomain(setup.ApplicationName, evidence, setup);

			log.Debug("  BaseDir: " + appDomain.BaseDirectory);
			log.Debug("  RelativeSearchPath: " + appDomain.RelativeSearchPath);
			log.Debug("  ShadowCopyDirectories: " + appDomain.SetupInformation.ShadowCopyDirectories);
			log.Debug("  CachePath: " + appDomain.SetupInformation.CachePath);
			log.Debug("  PrivateBinPath: " + appDomain.SetupInformation.PrivateBinPath);

			RemoteLoader l = this.CreateRemoteLoader(appDomain);

			// register the loader
			registry.RegisterLoader(domainId, l);

			this.InitializeBatchRegistration(l, domainNode.Children["batchRegistration"]);

			log.Debug("Adding as child kernel");
			kernel.AddChildKernel(l.Kernel);

			log.Info("Domain '{0}' created successfully.", appDomain.FriendlyName);

			return appDomain;
		}

		private RemoteLoader CreateRemoteLoader(AppDomain appDomain)
		{
			string remoteLoaderAsmName = typeof(RemoteLoader).Assembly.FullName;
			string remoteLoaderClsName = typeof(RemoteLoader).FullName;

			try
			{
				appDomain.DoCallBack(
					delegate
						{
							Assembly.Load("Castle.Core");
							Assembly.Load("Castle.MicroKernel");
							Assembly.Load("Castle.Facilities.DynamicLoader");
						});
			}
			catch(Exception ex)
			{
				log.Fatal("Error while loading required assemblies", ex);
				throw new FacilityException("Failed to load RemoteLoader required assemblies", ex);
			}

			try
			{
				// creates the RemoteLoader
				log.Debug("Creating the RemoteLoader");
				return (RemoteLoader) appDomain.CreateInstanceAndUnwrap(remoteLoaderAsmName, remoteLoaderClsName);
			}
			catch(Exception ex)
			{
				log.Fatal("Error while creating the RemoteLoader", ex);
				throw new FacilityException("Failed to create the RemoteLoader", ex);
			}
		}

		/// <summary>
		/// Initializes the batch registration.
		/// </summary>
		/// <param name="loader">The loader.</param>
		/// <param name="batchRegistrationNode">The batch registration node.</param>
		protected virtual void InitializeBatchRegistration(RemoteLoader loader, IConfiguration batchRegistrationNode)
		{
			if (batchRegistrationNode == null)
				return;

			foreach(IConfiguration comp in batchRegistrationNode.Children)
			{
				switch(comp.Name)
				{
					case "components":
						InitializeBatchComponents(loader, comp);
						break;
					default:
						throw new FacilityException("Unrecognized configuration node: " + comp.Name);
				}
			}
		}

		/// <summary>
		/// Register each batch component.
		/// </summary>
		/// <param name="loader">The <see cref="RemoteLoader"/> instance in which to register</param>
		/// <param name="componentsNode">The component configuration node</param>
		/// <remarks>
		/// <example>
		/// An example of a valid configuration node:
		/// <code>
		///	  &lt;component id="componentid.*"&gt;
		///     &lt;providesService service="Company.Project.IService, Company.Project" /&gt;
		///   &lt;/component&gt;
		/// </code>
		/// </example>
		/// </remarks>
		private void InitializeBatchComponents(RemoteLoader loader, IConfiguration componentsNode)
		{
			if (componentsNode == null)
				return;

			string componentIdMask = componentsNode.Attributes["id"];
			List<Type> servicesProvided = new List<Type>();

			foreach(IConfiguration cond in componentsNode.Children)
			{
				switch(cond.Name)
				{
					case "providesService":
						servicesProvided.Add(Type.GetType(cond.Attributes["service"]));
						break;
					default:
						throw new FacilityException("Unrecognized configuration node: " + cond.Name);
				}
			}

			loader.RegisterByServiceProvided(componentIdMask, servicesProvided.ToArray());
		}

		#region Configuration utility methods

		private string GetChildNodeValue(IConfiguration cfg, string nodeName)
		{
			if (cfg == null)
				return null;
			IConfiguration node = cfg.Children[nodeName];
			if (node == null)
				return null;
			return node.Value;
		}

		/// <summary>
		/// Gets the config attribute.
		/// </summary>
		/// <param name="cfg">The CFG.</param>
		/// <param name="attribute">The attribute.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <param name="defaultValueArguments">The default value arguments.</param>
		/// <returns></returns>
		protected string GetConfigAttribute(IConfiguration cfg, string attribute, string defaultValue,
		                                    params object[] defaultValueArguments)
		{
			string value = cfg.Attributes[attribute];
			if (value == null || value.Length == 0)
				value = String.Format(defaultValue, defaultValueArguments);

			return value;
		}

		private bool IsEmpty(string value)
		{
			return value == null || value.Length == 0;
		}

		private bool IsTrue(string value)
		{
			return String.Compare(value, "true", true) == 0;
		}

		#endregion

		#region Filesystem directory handling

		/// <summary>
		/// Normalizes a directory path. It includes resolving parent (<c>..</c>) paths
		/// and the <c>~</c> prefix, which maps to the root of the current application.
		/// </summary>
		/// <param name="path">The directory path</param>
		/// <returns>The normalized directory path</returns>
		/// <seealso cref="GetCurrentAppRootDirectory"/>
		protected virtual string NormalizeDirectoryPath(string path)
		{
			if (path.StartsWith("~"))
				path = GetCurrentAppRootDirectory() + path.Substring(1);
			return new DirectoryInfo(path).FullName;
		}

		/// <summary>
		/// Gets the root directory of the current application.
		/// For web applications, it is obtained from <see cref="HttpServerUtility.MapPath"/>.
		/// For other applications, <see cref="AppDomain.BaseDirectory"/> is used.
		/// </summary>
		protected virtual string GetCurrentAppRootDirectory()
		{
			if (isWeb)
				return HttpContext.Current.Server.MapPath("~");

			return AppDomain.CurrentDomain.BaseDirectory;
		}

		#endregion

		/// <summary>
		/// <see cref="IDisposable"/> implementation. Releases all <see cref="RemoteLoader"/>s
		/// and <see cref="AppDomain"/>s.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
				registry.Dispose();
		}
	}
}