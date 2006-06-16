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

namespace Castle.Facilities.DynamicLoader
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Reflection;
	using System.Runtime.Remoting;
	using System.Web;

	using Castle.MicroKernel.Facilities;
	using Castle.Model.Configuration;

	public class DynamicLoaderFacility : AbstractFacility
	{
		private readonly DynamicLoaderRegistry registry = new DynamicLoaderRegistry();

		bool isWeb;
		List<AppDomain> domains = new List<AppDomain>();
		Dictionary<AppDomain, RemoteLoader> loaders = new Dictionary<AppDomain, RemoteLoader>();

		protected override void Init()
		{
			Kernel.AddComponentInstance("dynamicLoader.registry", registry);
			Kernel.ComponentModelBuilder.AddContributor(new DynamicLoaderInspector(registry));
			
			isWeb = IsTrue(FacilityConfig.Attributes["isWeb"]);

			foreach (IConfiguration cfg in FacilityConfig.Children)
			{
				switch (cfg.Name)
				{
					case "domain":
						domains.Add(CreateAppDomain(cfg));
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
			
			AppDomainSetup setup = new AppDomainSetup();
			
			setup.ApplicationName = domainNode.Attributes["applicationName"];
			setup.ApplicationBase = NormalizeDirectoryPath(GetChildNodeValue(cfg, "applicationBase"));
			setup.ConfigurationFile = GetChildNodeValue(cfg, "configurationFile");

			setup.PrivateBinPath = GetChildNodeValue(cfg, "privateBinPath");
			if (!IsEmpty(setup.PrivateBinPath))
			{
				setup.PrivateBinPathProbe = Boolean.TrueString;
			}
			else
			{
				setup.DisallowApplicationBaseProbing = false;
				setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
				if (isWeb)
					setup.PrivateBinPath = Path.Combine(setup.PrivateBinPath, "bin");
			}

			setup.ShadowCopyFiles = domainNode.Attributes["shadowCopyFiles"];

			if (IsTrue(setup.ShadowCopyFiles))
			{
				setup.CachePath = cfg.Attributes["cachePath"];
				if (IsEmpty(setup.CachePath))
					setup.CachePath = String.Format("{0}{1}shadow{1}", setup.ApplicationBase, Path.DirectorySeparatorChar);
				setup.ShadowCopyDirectories = setup.ApplicationBase;
			}

			AppDomain appDomain = AppDomain.CreateDomain(setup.ApplicationName, null, setup);
			//appDomain.AssemblyResolve += new HereLoader(typeof(DynamicLoaderFacility).Assembly).AssemblyResolve;

			ObjectHandle h = appDomain.CreateInstance(typeof(RemoteLoader).Assembly.FullName,
																								typeof(RemoteLoader).FullName);

			RemoteLoader l = (RemoteLoader) h.Unwrap();

			registry.RegisterLoader(domainId, l);

			InitializeBatchRegistration(l, domainNode.Children["batchRegistration"]);
			InitializeComponents(l, domainNode.Children["components"]);

			Kernel.AddChildKernel(l.Kernel);

			return appDomain;
		}

		private void InitializeComponents(RemoteLoader loader, IConfiguration configuration)
		{
		}

		protected virtual void InitializeBatchRegistration(RemoteLoader loader, IConfiguration batchRegistrationNode)
		{
			foreach (IConfiguration comp in batchRegistrationNode.Children)
			{
				switch (comp.Name)
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
			string componentIdMask = componentsNode.Attributes["id"];
			List<Type> servicesProvided = new List<Type>();
			
			foreach (IConfiguration cond in componentsNode.Children)
			{
				switch (cond.Name)
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

		protected string GetConfigAttribute(IConfiguration cfg, string attribute, string defaultValue, params object[] defaultValueArguments)
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

		private void Unload()
		{
			foreach (RemoteLoader l in loaders.Values)
			{
				Kernel.RemoveChildKernel(l.Kernel);
				l.Dispose();
			}
			foreach (AppDomain d in domains)
				AppDomain.Unload(d);
		}

		public override void Dispose()
		{
			Unload();

			base.Dispose();
		}
		
		public class HereLoader : MarshalByRefObject
		{
			Assembly asm;

			public HereLoader(Assembly asm)
			{
				this.asm = asm;
			}

			public Assembly AssemblyResolve(object sender, ResolveEventArgs args)
			{
				return asm;
			}
		}
	}
}