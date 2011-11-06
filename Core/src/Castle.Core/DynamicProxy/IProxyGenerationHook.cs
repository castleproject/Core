// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

	/// <summary>
	///   Used during the target type inspection process. Implementors have a chance to customize the
	///   proxy generation process.
	/// </summary>
	public interface IProxyGenerationHook
	{
		/// <summary>
		///   Invoked by the generation process to notify that the whole process has completed.
		/// </summary>
		void MethodsInspected();

		/// <summary>
		///   Invoked by the generation process to notify that a member was not marked as virtual.
		/// </summary>
		/// <param name = "type">The type which declares the non-virtual member.</param>
		/// <param name = "memberInfo">The non-virtual member.</param>
		/// <remarks>
		///   This method gives an opportunity to inspect any non-proxyable member of a type that has 
		///   been requested to be proxied, and if appropriate - throw an exception to notify the caller.
		/// </remarks>
		void NonProxyableMemberNotification(Type type, MemberInfo memberInfo);

		/// <summary>
		///   Invoked by the generation process to determine if the specified method should be proxied.
		/// </summary>
		/// <param name = "type">The type which declares the given method.</param>
		/// <param name = "methodInfo">The method to inspect.</param>
		/// <returns>True if the given method should be proxied; false otherwise.</returns>
		bool ShouldInterceptMethod(Type type, MethodInfo methodInfo);
	}
}