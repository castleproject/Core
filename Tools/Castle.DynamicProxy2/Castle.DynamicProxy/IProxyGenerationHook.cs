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

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Used during the target type inspection process.
	/// Implementors have a chance to interfere in the
	/// proxy generation process
	/// </summary>
	public interface IProxyGenerationHook
	{
		/// <summary>
		/// Invoked by the generation process to know if
		/// the specified member should be proxied
		/// </summary>
		/// <param name="type"></param>
		/// <param name="memberInfo"></param>
		/// <returns></returns>
		bool ShouldInterceptMethod(Type type, MethodInfo memberInfo);

		/// <summary>
		/// Invoked by the generation process to notify that a
		/// member wasn't marked as virtual.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="memberInfo"></param>
		void NonVirtualMemberNotification(Type type, MemberInfo memberInfo);

		/// <summary>
		/// Invoked by the generation process to notify 
		/// that the whole process is completed.
		/// </summary>
		void MethodsInspected();
	}
}
