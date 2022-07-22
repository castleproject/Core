// Copyright 2004-2022 Castle Project - http://www.castleproject.org/
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
	public sealed class InheritanceInvocationWithoutTarget : InheritanceInvocation
	{
		public InheritanceInvocationWithoutTarget(Type targetType, object proxy, IInterceptor[] interceptors, MethodInfo proxiedMethod, object[] arguments)
			: base(targetType, proxy, interceptors, proxiedMethod, arguments)
		{
			Debug.Assert(proxiedMethod.IsAbstract, $"{nameof(InheritanceInvocationWithoutTarget)} does not support non-abstract methods.");
		}

		protected override void InvokeMethodOnTarget() => ThrowOnNoTarget();
	}
}
