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

namespace Castle.DynamicProxy.Internal
{
	using System;
	using System.Reflection;

	public abstract class InheritanceInvocation : AbstractInvocation
	{
		private readonly Type targetType;

		protected InheritanceInvocation(
			Type targetType,
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
			: base(proxy, interceptors, proxiedMethod, arguments)
		{
			this.targetType = targetType;
		}

		protected InheritanceInvocation(
			Type targetType,
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments,
			IInterceptorSelector selector,
			ref IInterceptor[] methodInterceptors)
			: base(proxy, targetType, interceptors, proxiedMethod, arguments, selector, ref methodInterceptors)
		{
			this.targetType = targetType;
		}

		public override object InvocationTarget
		{
			get { return Proxy; }
		}

		public override MethodInfo MethodInvocationTarget
		{
			get { return InvocationHelper.GetMethodOnType(targetType, Method); }
		}

		public override Type TargetType
		{
			get { return targetType; }
		}

		protected abstract override void InvokeMethodOnTarget();
	}
}