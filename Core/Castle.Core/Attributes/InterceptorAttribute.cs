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

	/// <summary>
	/// Used to declare that a component wants interceptors acting on it.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class InterceptorAttribute : Attribute
	{
		private readonly InterceptorReference interceptorRef;

		/// <summary>
		/// Constructs the InterceptorAttribute pointing to
		/// a key to a interceptor
		/// </summary>
		/// <param name="componentKey"></param>
		public InterceptorAttribute(String componentKey)
		{
			interceptorRef = new InterceptorReference(componentKey);
		}

		/// <summary>
		/// Constructs the InterceptorAttribute pointing to
		/// a service
		/// </summary>
		/// <param name="interceptorType"></param>
		public InterceptorAttribute(Type interceptorType)
		{
			interceptorRef = new InterceptorReference(interceptorType);
		}

		public InterceptorReference Interceptor
		{
			get { return interceptorRef; }
		}
	}
}