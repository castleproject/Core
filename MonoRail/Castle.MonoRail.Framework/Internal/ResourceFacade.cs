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

namespace Castle.MonoRail.Framework.Internal
{
	using System.Collections;
	using System.Globalization;
	using System.Resources;

	/// <summary>
	/// Simple facade that provides the IResource interface to a
	/// ResourceManager instance.
	/// </summary>
	public class ResourceFacade : IResource
	{
		private readonly ResourceManager resourceManager;
		private readonly CultureInfo cultureInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceFacade"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		/// <param name="cultureInfo">The culture info.</param>
		public ResourceFacade(ResourceManager resourceManager, CultureInfo cultureInfo)
		{
			this.resourceManager = resourceManager;
			this.cultureInfo = cultureInfo;
		}

		/// <summary>
		/// Returns the object linked to the specific key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object GetObject(string key)
		{
			return resourceManager.GetObject(key, cultureInfo);
		}

		/// <summary>
		/// Returns the object linked to the specific key as a string.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public string GetString(string key)
		{
			return resourceManager.GetString(key, cultureInfo);
		}

		/// <summary>
		/// Returns the object linked to the specific key.
		/// </summary>
		/// <value></value>
		public object this[string key]
		{
			get { return resourceManager.GetObject(key, cultureInfo); }
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return resourceManager
				.GetResourceSet(cultureInfo, true, true)
				.GetEnumerator();
		}
	}
}
