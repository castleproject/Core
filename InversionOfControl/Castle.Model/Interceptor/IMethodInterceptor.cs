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

	/// <summary>
	/// Interface that should be implemented 
	/// by any component that wishes to be referenced as interceptor.
	/// </summary>
	public interface IMethodInterceptor
	{
		/// <summary>
		/// Method invoked by the proxy in order to allow
		/// the interceptor to do its work before and after
		/// the actual invocation.
		/// </summary>
		/// <param name="invocation">The invocation holds the details of this interception</param>
		/// <param name="args">The original method arguments</param>
		/// <returns>The return value of this invocation</returns>
		object Intercept( IMethodInvocation invocation, params object[] args );
	}
}
