// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Interceptor
{
	using System;

	/// <summary>
	/// Represent a step - and a concern - during the 
	/// execution of a method.
	/// </summary>
	/// <
	public interface IInterceptor
	{
		/// <summary>
		/// Implementors should performs its logic before and/or 
		/// after invoking the Next.Process
		/// </summary>
		/// <param name="instance">The actual component instance</param>
		/// <param name="method">Method being executed</param>
		/// <param name="arguments">Method arguments</param>
		/// <returns>Should return the method result</returns>
		object Process( object instance, System.Reflection.MethodInfo method, params object[] arguments );

		/// <summary>
		/// Holds the next interceptor in the chain.
		/// </summary>
		IInterceptor Next {get; set;}
	}
}
