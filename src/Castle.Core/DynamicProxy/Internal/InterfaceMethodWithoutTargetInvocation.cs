// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Reflection;

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	[EditorBrowsable(EditorBrowsableState.Never)]
	public sealed class InterfaceMethodWithoutTargetInvocation : AbstractInvocation
	{
		public InterfaceMethodWithoutTargetInvocation(object target, object proxy, IInterceptor[] interceptors, MethodInfo proxiedMethod, object[] arguments)
			: base(proxy, interceptors, proxiedMethod, arguments)
		{
			// This invocation type is suitable for interface method invocations that cannot proceed
			// to a target, i.e. where `InvokeMethodOnTarget` will always throw:

			Debug.Assert(target == null, $"{nameof(InterfaceMethodWithoutTargetInvocation)} does not support targets.");
			Debug.Assert(proxiedMethod.IsAbstract, $"{nameof(InterfaceMethodWithoutTargetInvocation)} does not support non-abstract methods.");

			// Why this restriction? Because it greatly benefits proxy type generation performance.
			//
			// For invocations that can proceed to a target, `InvokeMethodOnTarget`'s implementation
			// depends on the target method's signature. Because of this, DynamicProxy needs to
			// dynamically generate a separate invocation type per such method. Type generation is
			// always expensive... that is, slow.
			//
			// However, if it is known that `InvokeMethodOnTarget` won't forward, but throw,
			// no custom (dynamically generated) invocation type is needed at all, and we can use
			// this unspecific invocation type instead.
		}

		// The next three properties mimick the behavior seen with an interface proxy without target.
		// (This is why this type's name starts with `Interface`.) A similar type could be written
		// for class proxies without target, but the values returned here would be different.

		public override object InvocationTarget => null;

		public override MethodInfo MethodInvocationTarget => null;

		public override Type TargetType => null;

		protected override void InvokeMethodOnTarget() => ThrowOnNoTarget();
	}
}
