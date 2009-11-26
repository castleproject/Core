// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Interceptor;

	public abstract class CompositionInvocation : AbstractInvocation
	{
		protected object target;

		protected CompositionInvocation(
			object target,
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
			: base(proxy, interceptors, proxiedMethod, arguments)
		{
			this.target = target;
		}

		protected CompositionInvocation(
			object target, 
			object proxy,
			IInterceptor[] interceptors, 
			MethodInfo proxiedMethod, 
			object[] arguments, 
			IInterceptorSelector selector, 
			ref IInterceptor[] methodInterceptors)
			: base(proxy, GetTargetType(target), interceptors, proxiedMethod, arguments, selector, ref methodInterceptors)
		{
			this.target = target;
		}

		private static Type GetTargetType(object targetObject)
		{
			if (targetObject == null)
			{
				return null;
			}
			return targetObject.GetType();
		}

		protected void EnsureValidTarget()
		{
			string message;
			if (target == null)
			{
				message = "This is a DynamicProxy2 error: the interceptor attempted " +
				          "to 'Proceed' for method '" + Method.ToString() + "' which has no target." +
				          " When calling method without target there is no implementation to 'proceed' to " +
				          "and it is the responsibility of the interceptor to mimic the implementation (set return value, out arguments etc)";
				throw new NotImplementedException(message);
			}

			if (!ReferenceEquals(target, proxyObject))
			{
				return;
			}
			message = "This is a DynamicProxy2 error: target of invocation has been set to the proxy itself. " +
			          "This may result in recursively calling the method over and over again until stack overflow, which may destabilize your program." +
			          "This usually signifies a bug in the calling code. Make sure no interceptor sets proxy as its invocation target.";
			throw new InvalidOperationException(message);
		}

		protected void EnsureValidProxyTarget(object newTarget)
		{
			if (newTarget == null)
			{
				throw new ArgumentNullException("newTarget");
			}

			if (!ReferenceEquals(newTarget, proxyObject))
			{
				return;
			}
			var message = "This is a DynamicProxy2 error: target of proxy has been set to the proxy itself. " +
						  "This would result in recursively calling proxy methods over and over again until stack overflow, which may destabilize your program." +
						  "This usually signifies a bug in the calling code. Make sure no interceptor sets proxy as its own target.";
			throw new InvalidOperationException(message);
		}

		public override object InvocationTarget
		{
			get { return target; }
		}

		public override Type TargetType
		{
			get
			{
				return GetTargetType(target);
			}
		}

		public override MethodInfo MethodInvocationTarget
		{
			get { return InvocationHelper.GetMethodOnObject(target, Method); }
		}
	}
}