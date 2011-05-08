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
	///   Provides an extension point that allows proxies to choose specific interceptors on
	///   a per method basis.
	/// </summary>
	public interface IInterceptorSelector
	{
		/// <summary>
		///   Selects the interceptors that should intercept calls to the given <paramref name = "method" />.
		/// </summary>
		/// <param name = "type">The type declaring the method to intercept.</param>
		/// <param name = "method">The method that will be intercepted.</param>
		/// <param name = "interceptors">All interceptors registered with the proxy.</param>
		/// <returns>An array of interceptors to invoke upon calling the <paramref name = "method" />.</returns>
		/// <remarks>
		///   This method is called only once per proxy instance, upon the first call to the
		///   <paramref name = "method" />. Either an empty array or null are valid return values to indicate
		///   that no interceptor should intercept calls to the method. Although it is not advised, it is
		///   legal to return other <see cref = "IInterceptor" /> implementations than these provided in
		///   <paramref name = "interceptors" />.
		/// </remarks>
		IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors);
	}
}