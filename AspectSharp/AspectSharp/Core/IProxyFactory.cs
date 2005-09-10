 // Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace AspectSharp.Core
{
	using System;
	using AspectSharp.Lang.AST;

	/// <summary>
	/// Defines the contract used by AspectEngine to obtain proxies.
	/// </summary>
	/// <remarks>
	/// The generated type must obey the requirements of Aspect#, in other
	/// words, the definitions in the AspectDefinition must be proper handled.
	/// </remarks>
	public interface IProxyFactory
	{
		/// <summary>
		/// Implementors must return a proxy (concrete class) that implements 
		/// the specified interface and dispatch the call to the specified target.
		/// </summary>
		/// <param name="inter">The interface to be implemented</param>
		/// <param name="target">The invocation default target</param>
		/// <param name="aspect">Definitions</param>
		/// <returns>The proxy instance</returns>
		object CreateInterfaceProxy(Type inter, object target, AspectDefinition aspect);

		/// <summary>
		/// Implementors must return a proxy (concrete class) that extends
		/// the specified classType.
		/// </summary>
		/// <param name="classType">The proxy super class</param>
		/// <param name="aspect">Definitions</param>
		/// <returns>The proxy instance</returns>
		object CreateClassProxy(Type classType, AspectDefinition aspect, params object[] constructorArgs);
	}
}