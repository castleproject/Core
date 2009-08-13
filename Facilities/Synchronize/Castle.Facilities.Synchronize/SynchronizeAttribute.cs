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
	using System.Threading;

	/// <summary>
	/// Used to declare that a component wants synchronization.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=false)]
	public class SynchronizeAttribute : Attribute
	{
		private SynchronizeContextReference interceptorRef;

		/// <summary>
		/// Constructs an empty SynchronizeAttribute.
		/// </summary>
		public SynchronizeAttribute()
		{
		}

		/// <summary>
		/// Constructs the SynchronizeAttribute pointing to a key.
		/// </summary>
		/// <param name="componentKey">The component key.</param>
		public SynchronizeAttribute(String componentKey)
		{
			interceptorRef = new SynchronizeContextReference(componentKey);
		}

		/// <summary>
		/// Constructs the SynchronizeAttribute pointing to a service.
		/// </summary>
		/// <param name="interceptorType">The service type.</param>
		public SynchronizeAttribute(Type interceptorType)
		{
			interceptorRef = new SynchronizeContextReference(interceptorType);
		}

		/// <summary>
		/// Determines if ambient <see cref="SynchronizationContext"/> is used.
		/// </summary>
		public bool UseAmbientContext { get; set; }

		/// <summary>
		/// Gets the synchronization context reference.
		/// </summary>
		public SynchronizeContextReference SynchronizeContext
		{
			get { return interceptorRef; }
		}
	}
}