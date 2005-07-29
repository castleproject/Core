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
		public String ClassName
		{
			get { return className; }

		}

		public bool CachingOn
		{
			set { isCachingOn_Renamed_Field = value; }

		}

		public long ModificationCheckInterval
		{
			get { return modificationCheckInterval; }

			set { this.modificationCheckInterval = value; }

		}

		///
		/// <summary> Does this loader want templates produced with it
		/// cached in the Runtime.
		/// </summary>
		protected internal bool isCachingOn_Renamed_Field = false;

		/// <summary> This property will be passed on to the templates
		/// that are created with this loader.
		/// </summary>
		protected internal long modificationCheckInterval = 2;

		/// <summary> Class name for this loader, for logging/debuggin
		/// purposes.
		/// </summary>
		protected internal String className = null;

		protected internal RuntimeServices rsvc = null;

		/// <summary> This initialization is used by all resource
		/// loaders and must be called to set up common
		/// properties shared by all resource loaders
		/// </summary>
		public void commonInit(RuntimeServices rs, ExtendedProperties configuration)
		{
			this.rsvc = rs;

			/*
			*  these two properties are not required for all loaders.
			*  For example, for ClasspathLoader, what would cache mean? 
			*  so adding default values which I think are the safest
			*
			*  don't cache, and modCheckInterval irrelevant...
			*/

			isCachingOn_Renamed_Field = configuration.GetBoolean("cache", false);
			modificationCheckInterval = configuration.GetLong("modificationCheckInterval", 0);

			/*
			* this is a must!
			*/

			className = configuration.GetString("class");
		}

		///
		/// <summary> Initialize the template loader with a
		/// a resources class.
		/// </summary>
		public abstract void init(ExtendedProperties configuration);

		///
		/// <summary> Get the InputStream that the Runtime will parse
		/// to create a template.
		/// </summary>
		public abstract Stream getResourceStream(String source);

		/// <summary> Given a template, check to see if the source of InputStream
		/// has been modified.
		/// </summary>
		public abstract bool isSourceModified(Resource resource);

		/// <summary> Get the last modified time of the InputStream source
		/// that was used to create the template. We need the template
		/// here because we have to extract the name of the template
		/// in order to locate the InputStream source.
		/// </summary>
		public abstract long getLastModified(Resource resource);

		/// <summary> Return the class name of this resource Loader
		/// </summary>
		/// <summary> Set the caching state. If true, then this loader
		/// would like the Runtime to cache templates that
		/// have been created with InputStreams provided
		/// by this loader.
		/// </summary>
		/// <summary> The Runtime uses this to find out whether this
		/// template loader wants the Runtime to cache
		/// templates created with InputStreams provided
		/// by this loader.
		/// </summary>
		public bool isCachingOn()
		{
			return isCachingOn_Renamed_Field;
		}

		/// <summary> Set the interval at which the InputStream source
		/// should be checked for modifications.
		/// </summary>

		/// <summary> Get the interval at which the InputStream source
		/// should be checked for modifications.
		/// </summary>
	}
}