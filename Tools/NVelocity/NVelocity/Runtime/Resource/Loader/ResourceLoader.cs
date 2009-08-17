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

namespace NVelocity.Runtime.Resource.Loader
{
	using System;
	using System.IO;
	using Commons.Collections;

	/// <summary>
	/// This is abstract class the all text resource loaders should extend.
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: ResourceLoader.cs,v 1.3 2003/10/27 13:54:11 corts Exp $</version>
	public abstract class ResourceLoader
	{
		/// <summary>
		/// Does this loader want templates produced with it
		/// cached in the Runtime.
		/// </summary>
		protected internal bool isCachingOn = false;

		/// <summary>
		/// This property will be passed on to the templates
		/// that are created with this loader.
		/// </summary>
		protected internal long modificationCheckInterval = 2;

		/// <summary>
		/// Class name for this loader, for logging/debugging
		/// purposes.
		/// </summary>
		protected internal String className = null;

		protected internal IRuntimeServices runtimeServices = null;

		/// <summary>
		/// This initialization is used by all resource
		/// loaders and must be called to set up common
		/// properties shared by all resource loaders
		/// </summary>
		public void CommonInit(IRuntimeServices rs, ExtendedProperties configuration)
		{
			runtimeServices = rs;

			// these two properties are not required for all loaders.
			// For example, for ClassPathLoader, what would cache mean? 
			// so adding default values which I think are the safest

			// don't cache, and modCheckInterval irrelevant...

			isCachingOn = configuration.GetBoolean("cache", false);
			modificationCheckInterval = configuration.GetLong("modificationCheckInterval", 0);

			// this is a must!
			className = configuration.GetString("class");
		}

		/// <summary>
		/// Initialize the template loader with a
		/// a resources class.
		/// </summary>
		public abstract void Init(ExtendedProperties configuration);

		/// <summary>
		/// Get the InputStream that the Runtime will parse
		/// to create a template.
		/// </summary>
		public abstract Stream GetResourceStream(String source);

		/// <summary>
		/// Given a template, check to see if the source of InputStream
		/// has been modified.
		/// </summary>
		public abstract bool IsSourceModified(Resource resource);

		/// <summary>
		/// Get the last modified time of the InputStream source
		/// that was used to create the template. We need the template
		/// here because we have to extract the name of the template
		/// in order to locate the InputStream source.
		/// </summary>
		public abstract long GetLastModified(Resource resource);

		/// <summary>
		/// Return the class name of this resource Loader
		/// </summary>
		public String ClassName
		{
			get { return className; }
		}

		/// <summary>
		/// Set the caching state. If true, then this loader
		/// would like the Runtime to cache templates that
		/// have been created with InputStreams provided
		/// by this loader.
		/// </summary>
		public bool CachingOn
		{
			set { isCachingOn = value; }
			get { return isCachingOn; }
		}

		public long ModificationCheckInterval
		{
			get { return modificationCheckInterval; }
			set { modificationCheckInterval = value; }
		}
	}
}