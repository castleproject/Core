// Copyright 2004 The Apache Software Foundation
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

namespace Apache.Avalon.DynamicProxy
{
	using System;
	using System.Reflection;

	/// <summary>
	/// Defines the handler that will receive all methods 
	/// invoked on the proxy object.
	/// </summary>
	public interface IInvocationHandler
	{
		/// <summary>
		/// Implementation should invoke the method on the real object.
		/// </summary>
		/// <param name="proxy">proxy instance</param>
		/// <param name="method"><see cref="System.Reflection.MethodInfo"/> being invoked.</param>
		/// <param name="arguments">Arguments of method - if any</param>
		/// <returns>Should return the result of method invocation</returns>
		object Invoke(object proxy, MethodInfo method, params object[] arguments);
	}
}
