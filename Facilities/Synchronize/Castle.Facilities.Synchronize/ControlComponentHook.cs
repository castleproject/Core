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
	using System.Reflection;
	using Castle.MicroKernel.Proxy;

	/// <summary>
	/// Proxy generation hook to filter all System methods when
	/// proxying a Windows Forms Control.
	/// </summary>
	public class ControlComponentHook : IProxyHook
	{
		/// <summary>
		/// Filters System methods.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="methodInfo">The method info.</param>
		/// <returns>true if not a System namespace, false otherwise.</returns>
		public bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
		{
			return !methodInfo.DeclaringType.Namespace.StartsWith("System.");
		}

		/// <summary>
		/// Not used.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="memberInfo"></param>
		public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
		{
		}

		/// <summary>
		/// Not used.
		/// </summary>
		public void MethodsInspected()
		{
		}
	}
}