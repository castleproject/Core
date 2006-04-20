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

namespace Castle.DynamicProxy.Invocation
{
	using System;
	using System.Reflection;

	[CLSCompliant(true)]
	public abstract class AbstractInvocation : MarshalByRefObject, IInvocation
	{
		protected ICallable callable;
		private MethodInfo method;
		private object proxy;
		private object target;
		protected object changed_target;

		public AbstractInvocation( ICallable callable, object proxy, MethodInfo method, object newtarget )
		{
			this.callable = callable;
			this.proxy = proxy;

			this.target = callable.Target;
			
			if (newtarget != null)
			{
				this.target = newtarget;
			}

			this.method = method;
		}

		public object Proxy
		{
			get { return proxy; }
		}

		public object InvocationTarget
		{
			get { return changed_target != null ? changed_target : target; }
			set { changed_target = value; }
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public MethodInfo MethodInvocationTarget
		{
			get { return Method; }
		}

		public virtual object Proceed(params object[] args)
		{
			// If the user changed the target, we use reflection
			// otherwise the delegate will be used.
			if (changed_target == null)
			{
				return callable.Call( args );
			}
			else
			{
				return Method.Invoke( changed_target, args );
			}
		}
	}
}
