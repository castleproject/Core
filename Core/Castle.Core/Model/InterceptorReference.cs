// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	[Serializable]
	public class InterceptorReference
	{
		private readonly InterceptorReferenceType refType;
		private Type serviceType;
		private String componentKey;

		public InterceptorReference( String componentKey )
		{
			if (componentKey == null)
			{
				throw new ArgumentNullException( "componentKey cannot be null" );
			}

			this.refType = InterceptorReferenceType.Key;
			this.componentKey = componentKey;
		}

		public InterceptorReference( Type serviceType )
		{
			if (serviceType == null)
			{
				throw new ArgumentNullException( "'serviceType' cannot be null" );
			}		

			this.refType = InterceptorReferenceType.Interface;
			this.serviceType = serviceType;
		}

		public Type ServiceType
		{
			get { return serviceType; }
		}

		public String ComponentKey
		{
			get { return componentKey; }
		}

		public InterceptorReferenceType ReferenceType
		{
			get { return refType; }
		}
	}
}
