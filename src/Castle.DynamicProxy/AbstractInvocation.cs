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
	using System.Diagnostics;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Castle.Core.Interceptor;

#if SILVERLIGHT
	public abstract class AbstractInvocation : IInvocation
#else
	[Serializable]
	public abstract class AbstractInvocation : IInvocation, ISerializable
#endif
	{
		private readonly IInterceptor[] interceptors;
		private readonly object[] arguments;
		private object returnValue;
		private int execIndex = -1;
		private Type[] genericMethodArguments;
		private readonly MethodInfo proxiedMethod;
		protected readonly object proxyObject;

		protected AbstractInvocation(
			object proxy,
			IInterceptor[] interceptors,
			MethodInfo proxiedMethod,
			object[] arguments)
		{
			Debug.Assert(proxiedMethod != null);
			this.proxyObject = proxy;
			this.interceptors = interceptors;
			this.proxiedMethod = proxiedMethod;
			this.arguments = arguments;
		}

		protected AbstractInvocation(object proxy, Type targetType, IInterceptor[] interceptors, MethodInfo proxiedMethod, object[] arguments, IInterceptorSelector selector, ref IInterceptor[] methodInterceptors)
			: this(proxy, interceptors, proxiedMethod, arguments)
		{
			methodInterceptors = SelectMethodInterceptors(selector, methodInterceptors, targetType);
			this.interceptors = methodInterceptors;
		}

		private IInterceptor[] SelectMethodInterceptors(IInterceptorSelector selector, IInterceptor[] methodInterceptors, Type targetType)
		{
			if (methodInterceptors == null)
			{
				methodInterceptors = selector.SelectInterceptors(targetType, Method, interceptors) ??
				                     new IInterceptor[0];
			}
			return methodInterceptors;
		}

		public void SetGenericMethodArguments(Type[] arguments)
		{
			genericMethodArguments = arguments;
		}

		public abstract object InvocationTarget { get; }

		public abstract Type TargetType { get; }

		public abstract MethodInfo MethodInvocationTarget { get; }

		public Type[] GenericArguments
		{
			get { return genericMethodArguments; }
		}

		public object Proxy
		{
			get { return proxyObject; }
		}

		public MethodInfo Method
		{
			get
			{
				return proxiedMethod;
			}
		}

		public MethodInfo GetConcreteMethod()
		{
			return EnsureClosedMethod(Method);
		}

		public MethodInfo GetConcreteMethodInvocationTarget()
		{
			// it is ensured by the InvocationHelper that method will be closed
			var method = MethodInvocationTarget;
			Debug.Assert(method == null || method.IsGenericMethodDefinition == false,
			             "method == null || method.IsGenericMethodDefinition == false");
			return method;
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
			arguments[index] = value;
		}

		public object GetArgumentValue(int index)
		{
			return arguments[index];
		}

		public void Proceed()
		{
			if (interceptors == null)
				// not yet fully initialized? probably, an intercepted method is called while we are being deserialized
			{
				InvokeMethodOnTarget();
				return;
			}

			execIndex++;

			if (execIndex == interceptors.Length)
			{
				InvokeMethodOnTarget();
			}
			else if (execIndex > interceptors.Length)
			{
				string interceptorsCount;
				if (interceptors.Length > 1)
				{
					interceptorsCount = " each one of " + interceptors.Length + " interceptors";
				}
				else
				{
					interceptorsCount = " interceptor";
				}

				var message = "This is a DynamicProxy2 error: invocation.Proceed() has been called more times than expected." +
				              "This usually signifies a bug in the calling code. Make sure that" + interceptorsCount +
				              " selected for this method '" + Method + "'" +
				              "calls invocation.Proceed() at most once.";
				throw new InvalidOperationException(message);
			}
			else
			{
				interceptors[execIndex].Intercept(this);
			}
		}

#if !SILVERLIGHT
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.SetType(typeof (RemotableInvocation));
			info.AddValue("invocation", new RemotableInvocation(this));
		}
#endif

		protected abstract void InvokeMethodOnTarget();

		private MethodInfo EnsureClosedMethod(MethodInfo method)
		{
			if (method.ContainsGenericParameters)
			{
				Debug.Assert(genericMethodArguments != null);
				return method.GetGenericMethodDefinition().MakeGenericMethod(genericMethodArguments);
			}
			return method;
		}
	}
}
