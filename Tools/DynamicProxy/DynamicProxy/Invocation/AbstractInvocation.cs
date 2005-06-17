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

	public abstract class AbstractInvocation : IInvocation
	{
		protected ICallable _callable;
		private MethodInfo _method;
		private object _proxy;
		private object _target;
		protected object _changed_target;

		public AbstractInvocation( ICallable callable, object proxy, MethodInfo method, object newtarget )
		{
			_callable = callable;
			_proxy = proxy;

			_target = callable.Target;
			
			if (newtarget != null)
			{
				_target = newtarget;
			}

			_method = method;
		}

		public object Proxy
		{
			get { return _proxy; }
		}

		public object InvocationTarget
		{
			get { return _changed_target != null ? _changed_target : _target; }
			set { _changed_target = value; }
		}

		public MethodInfo Method
		{
			get { return _method; }
		}

		public MethodInfo MethodInvocationTarget
		{
			get { return Method; }
		}

		public virtual object Proceed(params object[] args)
		{
			// If the user changed the target, we use reflection
			// otherwise the delegate will be used.
			if (_changed_target == null)
			{
				return _callable.Call( args );
			}
			else
			{
				return Method.Invoke(_changed_target, args);
			}
		}
	}
}