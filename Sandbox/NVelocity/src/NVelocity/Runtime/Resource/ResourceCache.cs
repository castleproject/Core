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
	using System.Collections;

	/// <summary> Interface that defines the shape of a pluggable resource cache
	/// for the included ResourceManager
	/// *
	/// </summary>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ResourceCache.cs,v 1.3 2003/10/27 13:54:11 corts Exp $
	///
	/// </version>
	public interface ResourceCache
	{
		/// <summary>  initializes the ResourceCache.  Will be
		/// called before any utilization
		/// *
		/// </summary>
		/// <param name="rs">RuntimeServices to use for logging, etc
		///
		/// </param>
		void initialize(RuntimeServices rs);

		/// <summary>  retrieves a Resource from the
		/// cache
		/// *
		/// </summary>
		/// <param name="resourceKey">key for Resource to be retrieved
		/// </param>
		/// <returns>Resource specified or null if not found
		///
		/// </returns>
		Resource get(Object resourceKey);

		/// <summary>  stores a Resource in the cache
		/// *
		/// </summary>
		/// <param name="resourceKey">key to associate with the Resource
		/// </param>
		/// <param name="resource">Resource to be stored
		/// </param>
		/// <returns>existing Resource stored under this key, or null if none
		///
		/// </returns>
		Resource put(Object resourceKey, Resource resource);

		/// <summary>  removes a Resource from the cache
		/// *
		/// </summary>
		/// <param name="resourceKey">resource to be removed
		/// </param>
		/// <param name="Resource">stored under key
		///
		/// </param>
		Resource remove(Object resourceKey);

		/// <summary>  returns an Iterator of Keys in the cache
		/// </summary>
		IEnumerator enumerateKeys();
	}
}