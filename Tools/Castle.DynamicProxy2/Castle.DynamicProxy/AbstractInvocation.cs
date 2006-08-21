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

namespace Castle.DynamicProxy
{
	using System;
	using System.Reflection;

	public abstract class AbstractInvocation : IInvocation //MarshalByRefObject, IInvocation
	{
		private readonly IInterceptor[] interceptors;
		private readonly Type targetType;
		private readonly MethodInfo targetMethod;
		private MethodInfo interfMethod;
		private object returnValue;
		private int execIndex = -1;
		private object[] arguments;

		protected AbstractInvocation(IInterceptor[] interceptors, Type targetType,
									 MethodInfo targetMethod, object[] arguments)
		{
			this.interceptors = interceptors;
			this.targetType = targetType;
			this.targetMethod = targetMethod;
			this.arguments = arguments;
		}

		protected AbstractInvocation(IInterceptor[] interceptors, Type targetType, 
		                             MethodInfo targetMethod, MethodInfo interfMethod, object[] arguments) : 
		                             	this(interceptors, targetType, targetMethod, arguments)
		{
			this.interfMethod = interfMethod;
		}

		public Type TargetType
		{
			get { return targetType; }
		}

		public MethodInfo Method
		{
			get { return interfMethod == null ? targetMethod : interfMethod; }
		}

		public MethodInfo MethodInvocationTarget
		{
			get { return targetMethod; }
		}

		public object ReturnValue
		{
			get { return returnValue; }
			set { returnValue = value; }
		}

		public object[] Arguments
		{
			get { return arguments; }
		}

		public void SetArgumentValue(int index, object value)
		{
			//TODO: Boundary checks
			arguments[index] = value;
		}

		public object GetArgumentValue(int index)
		{
			//TODO: Boundary checks
			return arguments[index];
		}

		public void Proceed()
		{
			execIndex++;

			if (execIndex == interceptors.Length)
			{
				InvokeMethodOnTarget();
			}
			else if (execIndex > interceptors.Length)
			{
				throw new ApplicationException("Blah");
			}
			else
			{
				interceptors[execIndex].Intercept(this);
			}
		}

		protected abstract void InvokeMethodOnTarget();
	}
}
