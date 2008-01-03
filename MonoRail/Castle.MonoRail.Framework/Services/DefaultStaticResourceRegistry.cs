// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using Castle.Core.Resource;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultStaticResourceRegistry : IStaticResourceRegistry
	{
		private static readonly Dictionary<ResourceKey, ResourceHolder> keyToResource = new Dictionary<ResourceKey, ResourceHolder>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultStaticResourceRegistry"/> class.
		/// </summary>
		public DefaultStaticResourceRegistry()
		{
			RegisterAssemblyResource("BehaviourScripts", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Behaviour", "jsfunctions", "text/javascript");

			RegisterAssemblyResource("AjaxScripts", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Ajax", "jsfunctions", "text/javascript");
			
			RegisterAssemblyResource("FormHelperScript", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.FormHelper", "jsfunctions", "text/javascript");
			
			RegisterAssemblyResource("ZebdaScripts", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.ZebdaValidation", "jsfunctions", "text/javascript");

			RegisterAssemblyResource("ValidateCore", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Validation", "fValidateCore", "text/javascript");
			
			RegisterAssemblyResource("ValidateLang", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.ValidationLang", "fValidateLang", "text/javascript");

			RegisterAssemblyResource("ValidateValidators", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Validation", "fValidateValidators", "text/javascript");

			RegisterAssemblyResource("ValidateConfig", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Validation", "fValidateConfig", "text/javascript");
			
			RegisterAssemblyResource("Effects2", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Effects2", "functions", "text/javascript");
			
			RegisterAssemblyResource("EffectsFatScripts", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.EffectsFat", "fatfunctions", "text/javascript");
		}

		/// <summary>
		/// Registers an assembly resource.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="assemblyName"></param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="resourceEntry">The resource entry name/key.</param>
		public void RegisterAssemblyResource(string name, string location, string version,
		                                     string assemblyName, string resourceName, string resourceEntry, string mimeType)
		{
			AssertParams(name, assemblyName, resourceName, resourceEntry, mimeType);

			CultureInfo ci = CultureInfo.InvariantCulture;

			if (location != null && location != "neutral")
			{
				ci = CultureInfo.CreateSpecificCulture(location);
			}

			IResource resource = new AssemblyBundleResource(
				new CustomUri("assembly://" + assemblyName + "/" + resourceName + "/" + resourceEntry), ci);

			keyToResource[new ResourceKey(name, location, version)] = new ResourceHolder(resource, mimeType);
		}

		/// <summary>
		/// Registers a custom resource.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="resource">The resource.</param>
		/// <param name="mimeType">Mime-type.</param>
		public void RegisterCustomResource(string name, string location, string version, IResource resource, string mimeType)
		{
			AssertParams(name, resource, mimeType);

			keyToResource[new ResourceKey(name, location, version)] = new ResourceHolder(resource, mimeType);
		}

		/// <summary>
		/// Checks whether the resource exists for name, location and version
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		public bool Exists(string name, string location, string version)
		{
			return keyToResource.ContainsKey(new ResourceKey(name, location, version));
		}

		/// <summary>
		/// Gets the resource content identified by the name, location and version.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <returns></returns>
		public string GetResource(string name, string location, string version, out string mimeType)
		{
			ResourceHolder resource;

			if (!keyToResource.TryGetValue(new ResourceKey(name, location, version), out resource))
			{
				throw new ResourceException("Could not load resource: " + name + " location: " + location + " version: " + version);
			}

			mimeType = resource.MimeType;
			return resource.Content;
		}

		#region Asserts

		private void AssertParams(string name, IResource resource, string mimeType)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(mimeType))
			{
				throw new ArgumentNullException("mimeType");
			}
			if (resource == null)
			{
				throw new ArgumentNullException("resource");
			}
		}

		private void AssertParams(string name, string assemblyName, string resourceName, string resourceEntry, string mimeType)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (string.IsNullOrEmpty(mimeType))
			{
				throw new ArgumentNullException("mimeType");
			}
			if (string.IsNullOrEmpty(assemblyName))
			{
				throw new ArgumentNullException("assemblyName");
			}
			if (string.IsNullOrEmpty(resourceName))
			{
				throw new ArgumentNullException("resourceName");
			}
			if (string.IsNullOrEmpty(resourceEntry))
			{
				throw new ArgumentNullException("resourceEntry");
			}
		}

		#endregion

		private class ResourceHolder
		{
			private IResource resource;
			private string mimeType;

			public ResourceHolder(IResource resource, string mimeType)
			{
				this.resource = resource;
				this.mimeType = mimeType;
			}

			public string MimeType
			{
				get { return mimeType; }
			}

			public string Content
			{
				get { return resource.GetStreamReader().ReadToEnd(); }
			}
		}

		private class ResourceKey : IEquatable<ResourceKey>
		{
			private string name, location, version;

			public ResourceKey(string name, string location, string version)
			{
				this.name = name;
				this.location = location;
				this.version = version;
			}

			public bool Equals(ResourceKey resourceKey)
			{
				if (resourceKey == null) return false;
				if (String.Compare(name, resourceKey.name, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					return false;
				}
				if (String.Compare(location, resourceKey.location, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					return false;
				}
				if (String.Compare(version, resourceKey.version, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					return false;
				}
				return true;
			}

			public override int GetHashCode()
			{
				int result = name.ToLowerInvariant().GetHashCode();
				result = 29 * result + (location != null ? location.ToLowerInvariant().GetHashCode() : 0);
				result = 29 * result + (version != null ? version.ToLowerInvariant().GetHashCode() : 0);
				return result;
			}
		}
	}
}