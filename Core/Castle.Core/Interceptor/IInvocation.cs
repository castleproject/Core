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
	using System.Reflection;

	public interface IInvocation
	{
		// object Proxy { get; }

		// object InvocationTarget { get;set; }

		/// <summary>
		/// 
		/// </summary>
		Type TargetType { get; }

		/// <summary>
		/// 
		/// </summary>
		object[] Arguments { get; }

		/// <summary>
		/// 
		/// </summary>
		void SetArgumentValue(int index, object value);

		/// <summary>
		/// 
		/// </summary>
		object GetArgumentValue(int index);

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		void Proceed();

		/// <summary>
		/// 
		/// </summary>
		object ReturnValue { get; set; }

		/// <summary>
		/// 
		/// </summary>
		MethodInfo Method { get; }

		/// <summary>
		/// Returns the method on the target of invocation, 
		/// which can be for example the method defined on an
		/// interface, if dealing with an interface proxy
		/// </summary>
		MethodInfo MethodInvocationTarget { get; }
	}
}
