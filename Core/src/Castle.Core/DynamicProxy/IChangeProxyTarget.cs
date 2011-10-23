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

	/// <summary>
	///   Exposes means to change target objects of proxies and invocations
	/// </summary>
	public interface IChangeProxyTarget
	{
		/// <summary>
		///   Changes the target object (<see cref = "IInvocation.InvocationTarget" />) of current <see cref = "IInvocation" />.
		/// </summary>
		/// <param name = "target">The new value of target of invocation.</param>
		/// <remarks>
		///   Although the method takes <see cref = "object" /> the actual instance must be of type assignable to <see
		///    cref = "IInvocation.TargetType" />, otherwise an <see cref = "InvalidCastException" /> will be thrown.
		///   Also while it's technically legal to pass null reference (Nothing in Visual Basic) as <paramref name = "target" />, for obvious reasons Dynamic Proxy will not be able to call the intercepted method on such target.
		///   In this case last interceptor in the pipeline mustn't call <see cref = "IInvocation.Proceed" /> or a <see
		///    cref = "NotImplementedException" /> will be throws.
		///   Also while it's technically legal to pass proxy itself as <paramref name = "target" />, this would create stack overflow.
		///   In this case last interceptor in the pipeline mustn't call <see cref = "IInvocation.Proceed" /> or a <see
		///    cref = "InvalidOperationException" /> will be throws.
		/// </remarks>
		/// <exception cref = "InvalidCastException">Thrown when <paramref name = "target" /> is not assignable to the proxied type.</exception>
		void ChangeInvocationTarget(object target);

		/// <summary>
		///   Permanently changes the target object of the proxy. This does not affect target of the current invocation.
		/// </summary>
		/// <param name = "target">The new value of target of the proxy.</param>
		/// <remarks>
		///   Although the method takes <see cref = "object" /> the actual instance must be of type assignable to proxy's target type, otherwise an <see
		///    cref = "InvalidCastException" /> will be thrown.
		///   Also while it's technically legal to pass null reference (Nothing in Visual Basic) as <paramref name = "target" />, for obvious reasons Dynamic Proxy will not be able to call the intercepted method on such target.
		///   In this case last interceptor in the pipeline mustn't call <see cref = "IInvocation.Proceed" /> or a <see
		///    cref = "NotImplementedException" /> will be throws.
		///   Also while it's technically legal to pass proxy itself as <paramref name = "target" />, this would create stack overflow.
		///   In this case last interceptor in the pipeline mustn't call <see cref = "IInvocation.Proceed" /> or a <see
		///    cref = "InvalidOperationException" /> will be throws.
		/// </remarks>
		/// <exception cref = "InvalidCastException">Thrown when <paramref name = "target" /> is not assignable to the proxied type.</exception>
		void ChangeProxyTarget(object target);
	}
}