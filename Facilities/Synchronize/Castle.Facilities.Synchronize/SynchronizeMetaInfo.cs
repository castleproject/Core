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

namespace Castle.Facilities.Synchronize
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Maintains the synchronization meta-info for a component.
	/// </summary>
	internal class SynchronizeMetaInfo
	{
		private readonly SynchronizeAttribute defaultSyncAttrib;
		private readonly Dictionary<MethodInfo, SynchronizeAttribute> method2Att;

		/// <summary>
		/// Initializes a new instance of the <see cref="SynchronizeMetaInfo"/> class.
		/// </summary>
		/// <param name="defaultSyncAttrib">The default synchronization.</param>
		public SynchronizeMetaInfo(SynchronizeAttribute defaultSyncAttrib)
		{
			this.defaultSyncAttrib = defaultSyncAttrib;
			method2Att = new Dictionary<MethodInfo, SynchronizeAttribute>();
		}

		/// <summary>
		/// Adds the synchronizaed method to the store.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="syncAttrib">The method synchronization.</param>
		public void Add(MethodInfo method, SynchronizeAttribute syncAttrib)
		{
			if (syncAttrib.SynchronizeContext == null && defaultSyncAttrib == null)
			{
				throw new FacilityException(
					String.Format("Method {0} did not specify a synchronization context" +
					              " and no default was defined.", method));
			}

			method2Att.Add(method, syncAttrib);
		}

		/// <summary>
		/// Gets the list of synchronized methods.
		/// </summary>
		/// <value>The list of synchronized methods.</value>
		public MethodInfo[] Methods
		{
			get
			{
				MethodInfo[] methods = new MethodInfo[method2Att.Count];
				method2Att.Keys.CopyTo(methods, 0);
				return methods;
			}
		}

		/// <summary>
		/// Gets the reference to the synchronized context
		/// requested by the method.
		/// </summary>
		/// <param name="methodInfo">The method.</param>
		/// <returns>The synchroniztion context reference or null.</returns>
		public SynchronizeContextReference GetSynchronizedContextFor(MethodInfo methodInfo)
		{
			SynchronizeAttribute syncAttrib;

			if (method2Att.TryGetValue(methodInfo, out syncAttrib))
			{
				return syncAttrib.SynchronizeContext ?? defaultSyncAttrib.SynchronizeContext;
			}

			return null;
		}

		/// <summary>
		/// Gets the list of unique synchronization context references.
		/// </summary>
		/// <returns>The list of unique synchronization context references.</returns>
		public IList<SynchronizeContextReference> GetUniqueSynchContextReferences()
		{
			List<SynchronizeContextReference> references = new List<SynchronizeContextReference>();

			if (defaultSyncAttrib != null && defaultSyncAttrib.SynchronizeContext != null)
			{
				references.Add(defaultSyncAttrib.SynchronizeContext);
			}

			foreach(SynchronizeAttribute syncAttrib in method2Att.Values)
			{
				SynchronizeContextReference reference = syncAttrib.SynchronizeContext;

				if (reference != null && !references.Contains(reference))
				{
					references.Add(reference);
				}
			}

			return references.AsReadOnly();
		}
	}
}