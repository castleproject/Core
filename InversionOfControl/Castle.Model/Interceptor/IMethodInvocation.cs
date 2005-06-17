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

namespace Castle.Model.Interceptor
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Supply information about the current method invocation.
	/// </summary>
	public interface IMethodInvocation
	{
		/// <summary>
		/// The proxy instance.
		/// </summary>
		object Proxy { get; }

		/// <summary>
		/// The target of this invocation, which usually is the
		/// instance of the class being proxy. 
		/// You might change the target, but be aware of the 
		/// implications.
		/// </summary>
		object InvocationTarget { get;set; }

		/// <summary>
		/// The Method being invoked.
		/// </summary>
		MethodInfo Method { get; }

		/// <summary>
		/// The Method being invoked on the target.
		/// </summary>
		MethodInfo MethodInvocationTarget { get; }

		/// <summary>
		/// The Proceed go on with the actual invocation.
		/// </summary>
		/// <param name="args">The arguments of the method</param>
		/// <returns></returns>
		object Proceed( params object[] args );
	}
}
