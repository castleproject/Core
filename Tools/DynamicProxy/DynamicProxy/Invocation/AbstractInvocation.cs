// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
		protected object _original_target;
		private MethodInfo _method;
		private object _proxy;
		private object _target;

		public AbstractInvocation( ICallable callable, object proxy, MethodInfo method )
		{
			_callable = callable;
			_proxy = proxy;
			_target = _original_target = callable.Target;
			_method = method;
		}

		public object Proxy
		{
			get { return _proxy; }
		}

		public object InvocationTarget
		{
			get { return _target; }
			set { _target = value; }
		}

		public MethodInfo Method
		{
			get { return _method; }
		}

		public abstract object Proceed(params object[] args);
	}
}