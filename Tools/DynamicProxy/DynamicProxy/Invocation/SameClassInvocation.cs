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

namespace Castle.DynamicProxy.Invocation
{
	using System.Reflection;

	/// <summary>
	/// 
	/// </summary>
	public class SameClassInvocation : AbstractInvocation
	{
		public SameClassInvocation(ICallable callable, object proxy, MethodInfo method) : 
			base(callable, proxy, method)
		{
		}

		public override object Proceed(params object[] args)
		{
			// If the user changed the target, we use reflection
			// otherwise the delegate will be used.
			if (InvocationTarget == _original_target)
			{
				return _callable.Call( args );
			}
			else
			{
				return Method.Invoke(InvocationTarget, args);
			}
		}
	}
}