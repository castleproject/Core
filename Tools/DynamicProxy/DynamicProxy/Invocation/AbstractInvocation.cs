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
		private object proxy;
		private object target;
		private MethodInfo method;

		public AbstractInvocation( object proxy, object target, MethodInfo method )
		{
			this.proxy = proxy;
			this.target = target;
			this.method = method;
		}

		public object Proxy
		{
			get { return proxy; }
		}

		public object InvocationTarget
		{
			get { return target; }
			set { target = value; }
		}

		public MethodInfo Method
		{
			get { return method; }
		}

		public abstract object Proceed(params object[] args);
	}
}