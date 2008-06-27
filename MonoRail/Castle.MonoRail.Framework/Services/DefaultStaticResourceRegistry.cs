// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
		private static readonly Dictionary<ResourceKey, ResourceHolder> keyToResource =
			new Dictionary<ResourceKey, ResourceHolder>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultStaticResourceRegistry"/> class.
		/// </summary>
		public DefaultStaticResourceRegistry()
		{
			RegisterAssemblyResource("BehaviourScripts", null, null, "Castle.MonoRail.Framework",
			                         "Castle.MonoRail.Framework.JSResources.Behaviour", "jsfunctions", "text/javascript", null);

			RegisterAssemblyResource("AjaxScripts", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.Ajax", "jsfunctions", "text/javascript", null);

			RegisterAssemblyResource("FormHelperScript", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.FormHelper", "jsfunctions", "text/javascript", null);

			RegisterAssemblyResource("ZebdaScripts", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.ZebdaValidation", "jsfunctions", "text/javascript", null);

			RegisterAssemblyResource("ValidateCore", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.Validation", "fValidateCore", "text/javascript", null);

			RegisterAssemblyResource("ValidateLang", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.ValidationLang", "fValidateLang", "text/javascript", null);

			RegisterAssemblyResource("ValidateValidators", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.Validation", "fValidateValidators", "text/javascript", null);

			RegisterAssemblyResource("ValidateConfig", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.Validation", "fValidateConfig", "text/javascript", null);

			RegisterAssemblyResource("Effects2", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.Effects2", "functions", "text/javascript", null);

			RegisterAssemblyResource("EffectsFatScripts", null, null, "Castle.MonoRail.Framework",
									 "Castle.MonoRail.Framework.JSResources.EffectsFat", "fatfunctions", "text/javascript", null);
		}

		/// <summary>
		/// Registers an assembly resource.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="resourceEntry">The resource entry name/key.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="lastModified">The last modified.</param>
		public void RegisterAssemblyResource(string name, string location, string version,
		                                     string assemblyName, string resourceName, string resourceEntry,
		                                     string mimeType, DateTime? lastModified)
		{
			AssertParams(name, assemblyName, resourceName, resourceEntry, mimeType);

			CultureInfo ci = CultureInfo.InvariantCulture;

			if (location != null && location != "neutral")
			{
				ci = CultureInfo.CreateSpecificCulture(location);
			}

			IResource resource = new AssemblyBundleResource(
				new CustomUri("assembly://" + assemblyName + "/" + resourceName + "/" + resourceEntry), ci);

			keyToResource[new ResourceKey(name, location, version)] = new ResourceHolder(resource, mimeType, lastModified);
		}

		/// <summary>
		/// Registers a custom resource.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="resource">The resource.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="lastModified">The last modified.</param>
		public void RegisterCustomResource(string name, string location, string version,
		                                   IResource resource, string mimeType,
		                                   DateTime? lastModified)
		{
			AssertParams(name, resource, mimeType);

			keyToResource[new ResourceKey(name, location, version)] = new ResourceHolder(resource, mimeType, lastModified);
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
		/// Gets the a resource last modified.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		public DateTime? GetLastModified(string name, string location, string version)
		{
			ResourceHolder resource;

			if (!keyToResource.TryGetValue(new ResourceKey(name, location, version), out resource))
			{
				return null;
			}

			return resource.LastModified;
		}

		/// <summary>
		/// Gets the resource content identified by the name, location and version.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="lastModified">The last modified.</param>
		/// <returns></returns>
		public string GetResource(string name, string location, string version, out string mimeType,
		                          out DateTime? lastModified)
		{
			ResourceHolder resource;

			if (!keyToResource.TryGetValue(new ResourceKey(name, location, version), out resource))
			{
				throw new ResourceException("Could not load resource: " + name + " location: " + location + " version: " + version);
			}

			mimeType = resource.MimeType;
			lastModified = resource.LastModified;

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
			private readonly IResource resource;
			private readonly DateTime? lastModified;
			private readonly string mimeType;

			public ResourceHolder(IResource resource, string mimeType, DateTime? lastModified)
			{
				this.resource = resource;
				this.lastModified = lastModified;
				this.mimeType = mimeType;
			}

			public DateTime? LastModified
			{
				get { return lastModified; }
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
				if (resourceKey == null)
				{
					return false;
				}
				if (String.Compare(name, resourceKey.name, StringComparison.OrdinalIgnoreCase) != 0)
				{
					return false;
				}
				if (String.Compare(location, resourceKey.location, StringComparison.OrdinalIgnoreCase) != 0)
				{
					return false;
				}
				if (String.Compare(version, resourceKey.version, StringComparison.OrdinalIgnoreCase) != 0)
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