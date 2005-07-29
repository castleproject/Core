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

namespace NVelocity.Runtime.Resource
{
	using System;
	using NVelocity.Runtime.Resource.Loader;

	/// <summary>
	/// This class represent a general text resource that
	/// may have been retrieved from any number of possible
	/// sources.
	/// </summary>
	/// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author><a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	/// <version> $Id: Resource.cs,v 1.5 2004/01/02 00:13:51 corts Exp $</version>
	public abstract class Resource
	{
		/// <summary>
		/// The number of milliseconds in a minute, used to calculate the
		/// check interval.
		/// </summary>
		protected internal const long MILLIS_PER_SECOND = 1000;

		/// <summary>
		/// Resource might require ancillary storage of some kind
		/// </summary>
		protected internal Object data = null;

		/// <summary>
		/// Character encoding of this resource
		/// </summary>
		protected internal String encoding = RuntimeConstants_Fields.ENCODING_DEFAULT;

		/// <summary>
		/// The file modification time (in milliseconds) for the cached template.
		/// </summary>
		protected internal long lastModified = 0;

		/// <summary>
		/// How often the file modification time is checked (in milliseconds).
		/// </summary>
		protected internal long modificationCheckInterval = 0;

		/// <summary>
		/// Name of the resource
		/// </summary>
		protected internal String name;

		/// <summary>
		/// The next time the file modification time will be checked (in milliseconds).
		/// </summary>
		protected internal long nextCheck = 0;

		/// <summary>
		/// The template loader that initially loaded the input
		/// stream for this template, and knows how to check the
		/// source of the input stream for modification.
		/// </summary>
		protected internal ResourceLoader resourceLoader;

		protected internal RuntimeServices rsvc = null;


		public bool IsSourceModified()
		{
			return resourceLoader.isSourceModified(this);
		}

		/// <summary> Perform any subsequent processing that might need
		/// to be done by a resource. In the case of a template
		/// the actual parsing of the input stream needs to be
		/// performed.
		/// </summary>
		/// <returns>Whether the resource could be processed successfully.
		/// For a {@link org.apache.velocity.Template} or {@link
		/// org.apache.velocity.runtime.resource.ContentResource}, this
		/// indicates whether the resource could be read.
		/// @exception ResourceNotFoundException Similar in semantics as
		/// returning <code>false</code>.
		/// </returns>
		public abstract bool Process();

		/// <summary> Set the modification check interval.
		/// </summary>
		/// <param name="interval">The interval (in seconds).
		///
		/// </param>
		/// <summary> Is it time to check to see if the resource
		/// source has been updated?
		/// </summary>
		public bool RequiresChecking()
		{
			/*
	    *  short circuit this if modificationCheckInterval == 0
	    *  as this means "don't check"
	    */

			if (modificationCheckInterval <= 0)
			{
				return false;
			}

			/*
	    *  see if we need to check now
	    */

			return ((DateTime.Now.Ticks - 621355968000000000)/10000 >= nextCheck);
		}

		/// <summary>
		/// 'Touch' this template and thereby resetting the nextCheck field.
		/// </summary>
		public void Touch()
		{
			nextCheck = (DateTime.Now.Ticks - 621355968000000000)/10000 + (MILLIS_PER_SECOND*modificationCheckInterval);
		}

		/// <summary>
		/// Set arbitrary data object that might be used
		/// by the resource.
		///
		/// Get arbitrary data object that might be used
		/// by the resource.
		/// </summary>
		public Object Data
		{
			get { return data; }
			set { this.data = value; }
		}

		/// <summary>
		/// set the encoding of this resource
		/// for example, "ISO-8859-1"
		/// 
		/// get the encoding of this resource
		/// for example, "ISO-8859-1"
		/// </summary>
		public String Encoding
		{
			get { return encoding; }
			set { this.encoding = value; }
		}

		/// <summary>
		/// Return the lastModifed time of this
		/// template.
		/// 
		/// Set the last modified time for this
		/// template.
		/// </summary>
		public long LastModified
		{
			get { return lastModified; }
			set { this.lastModified = value; }
		}

		public long ModificationCheckInterval
		{
			set { this.modificationCheckInterval = value; }
		}

		/// <summary>
		/// Set the name of this resource, for example test.vm.
		/// 
		/// Get the name of this template.
		/// </summary>
		public String Name
		{
			get { return name; }
			set { this.name = value; }
		}

		/// <summary>
		/// Return the template loader that pulled
		/// in the template stream
		/// 
		/// Set the template loader for this template. Set
		/// when the Runtime determines where this template
		/// came from the list of possible sources.
		/// </summary>
		public ResourceLoader ResourceLoader
		{
			get { return resourceLoader; }
			set { this.resourceLoader = value; }
		}

		public RuntimeServices RuntimeServices
		{
			set { rsvc = value; }
		}

	}
}