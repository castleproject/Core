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
	using System.Reflection;

	/// <summary>
	/// Proceed with, manipulate or find more information about the call that 
	/// is being intercepted
	/// </summary>
	public interface IInvocation
	{
		/// <summary>
		/// Get the dynamic proxy that intercepted this call.
		/// </summary>
		object Proxy { get; }

		/// <summary>
		/// Get or set target that will be invoked when Process() is called.  		
		/// </summary>
		/// <remarks>
		/// Changing InvocationTarget only effects this call.  Any call made after
		/// this will invoke the original target of the proxy.
		/// </remarks>
		object InvocationTarget { get;set; }

		/// <summary>
		/// Get the method that is being invoked.
		/// </summary>
		MethodInfo Method { get; }

		/// <summary>
		/// Proceed with the call that was intercepted.
		/// </summary>
		/// <param name="args">The arguments that will be passed onto the method.</param>
		/// <returns>The argument returned from the method.</returns>
		object Proceed( params object[] args );

		/// <summary>
		/// Get the method on the target object that is being invoked.
		/// </summary>
		MethodInfo MethodInvocationTarget { get; }

	}

}
