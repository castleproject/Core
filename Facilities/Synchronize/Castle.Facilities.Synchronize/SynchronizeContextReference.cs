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

	/// <summary>
	/// Identifies the type of synchornization context reference. 
	/// </summary>
	public enum SynchronizeContextReferenceType
	{
		/// <summary>
		/// Service interface reference.
		/// </summary>
		Interface,

		/// <summary>
		/// Component key reference.
		/// </summary>
		Key
	}

	/// <summary>
	/// Represents a reference to a SynchronizeContext component.
	/// </summary>
	[Serializable]
	public class SynchronizeContextReference
	{
		private Type serviceType;
		private String componentKey;
		private readonly SynchronizeContextReferenceType refType;

		/// <summary>
		/// Initializes a new instance of the <see cref="SynchronizeContextReference"/> class.
		/// </summary>
		/// <param name="componentKey">The component key.</param>
		public SynchronizeContextReference(String componentKey)
		{
			if (componentKey == null)
			{
				throw new ArgumentNullException("componentKey cannot be null");
			}

			refType = SynchronizeContextReferenceType.Key;
			this.componentKey = componentKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SynchronizeContextReference"/> class.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		public SynchronizeContextReference(Type serviceType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException("'serviceType' cannot be null");
			}

			refType = SynchronizeContextReferenceType.Interface;
			this.serviceType = serviceType;
		}

		/// <summary>
		/// Gets the type of the synchronization service.
		/// </summary>
		/// <value>The type of the synchronization service.</value>
		public Type ServiceType
		{
			get { return serviceType; }
		}

		/// <summary>
		/// Gets the synchronization context component key.
		/// </summary>
		/// <value>The synchronization component key.</value>
		public String ComponentKey
		{
			get { return componentKey; }
		}

		/// <summary>
		/// Gets the type of the reference.
		/// </summary>
		/// <value>The type of the reference.</value>
		public SynchronizeContextReferenceType ReferenceType
		{
			get { return refType; }
		}

		/// <summary>
		/// Determines if the other reference is equal.
		/// </summary>
		/// <param name="obj">The other reference.</param>
		/// <returns>true if equal, false otherwise.</returns>
		public override bool Equals(object obj)
		{
			if (this == obj) return true;

			var other = obj as SynchronizeContextReference;
			if (other == null) return false;

			return refType == other.refType &&
			       componentKey == other.componentKey &&
			       serviceType == other.ServiceType;
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <returns>The hash code.</returns>
		public override int GetHashCode()
		{
			return refType.GetHashCode() ^
			       (componentKey != null
			        	? componentKey.GetHashCode()
			        	: serviceType.GetHashCode());
		}

		/// <summary>
		/// Gets the string representation of the reference.
		/// </summary>
		/// <returns>The string representation of the reference.</returns>
		public override string ToString()
		{
			string str = "SynchronizationContextReference : ";

			if (refType == SynchronizeContextReferenceType.Key)
			{
				str += string.Format("key = {0}", ComponentKey);
			}
			else
			{
				str += string.Format("service = {0}", ServiceType.FullName);
			}

			return str;
		}
	}
}