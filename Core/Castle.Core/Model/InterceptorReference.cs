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

namespace Castle.Core
{
	using System;

	public enum InterceptorReferenceType
	{
		Interface,
		Key
	}

	/// <summary>
	/// Represents an reference to a Interceptor component.
	/// </summary>
#if !SILVERLIGHT
	[Serializable]
#endif
	public class InterceptorReference : IEquatable<InterceptorReference>
	{
		private readonly InterceptorReferenceType refType;
		private readonly Type serviceType;
		private readonly String componentKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorReference"/> class.
		/// </summary>
		/// <param name="componentKey">The component key.</param>
		public InterceptorReference(String componentKey)
		{
			if (componentKey == null)
			{
				throw new ArgumentNullException("componentKey cannot be null");
			}

			refType = InterceptorReferenceType.Key;
			this.componentKey = componentKey;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="InterceptorReference"/> class.
		/// </summary>
		/// <param name="serviceType">Type of the service.</param>
		public InterceptorReference(Type serviceType)
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException("'serviceType' cannot be null");
			}

			refType = InterceptorReferenceType.Interface;
			this.serviceType = serviceType;
		}

		/// <summary>
		/// Gets the type of the service.
		/// </summary>
		/// <value>The type of the service.</value>
		public Type ServiceType
		{
			get { return serviceType; }
		}

		/// <summary>
		/// Gets the interceptor component key.
		/// </summary>
		/// <value>The component key.</value>
		public String ComponentKey
		{
			get { return componentKey; }
		}

		/// <summary>
		/// Gets the type of the reference.
		/// </summary>
		/// <value>The type of the reference.</value>
		public InterceptorReferenceType ReferenceType
		{
			get { return refType; }
		}

		/// <summary>
		/// Gets an <see cref="InterceptorReference"/> for the component key.
		/// </summary>
		/// <param name="key">The component key.</param>
		/// <returns>The <see cref="InterceptorReference"/></returns>
		public static InterceptorReference ForKey(String key)
		{
			return new InterceptorReference(key);
		}

		/// <summary>
		/// Gets an <see cref="InterceptorReference"/> for the service.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <returns>The <see cref="InterceptorReference"/></returns>
		public static InterceptorReference ForType(Type service)
		{
			return new InterceptorReference(service);
		}

		/// <summary>
		/// Gets an <see cref="InterceptorReference"/> for the service.
		/// </summary>
		/// <typeparam name="T">The service type.</typeparam>
		/// <returns>The <see cref="InterceptorReference"/></returns>
		public static InterceptorReference ForType<T>()
		{
			return new InterceptorReference(typeof(T));
		}

		public bool Equals(InterceptorReference other)
		{
			if (other == null) return false;
			if (!Equals(refType, other.refType)) return false;
			if (!Equals(serviceType, other.serviceType)) return false;
			if (!Equals(componentKey, other.componentKey)) return false;
			return true;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) return true;
			return Equals(obj as InterceptorReference);
		}

		public override int GetHashCode()
		{
			int result = refType.GetHashCode();
			result = 29*result + (serviceType != null ? serviceType.GetHashCode() : 0);
			result = 29*result + (componentKey != null ? componentKey.GetHashCode() : 0);
			return result;
		}
	}
}