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

namespace AspectSharp.Core.Dispatcher
{
	using System;
	using System.Collections;
	using System.Reflection;
	using System.Runtime.Serialization;
	using AopAlliance.Intercept;
	using AspectSharp.Lang.AST;
	using AspectSharp.Core.Matchers;

	/// <summary>
	/// Summary description for DefaultInvocationDispatcher.
	/// </summary>
	[Serializable]
	public class DefaultInvocationDispatcher : IInvocationDispatcher, IDeserializationCallback
	{
		private AspectDefinition _aspect;
		private IJoinPointMatcher _matcher;

		[NonSerialized] private IDictionary _type2AdviceInstance = new Hashtable();
		[NonSerialized] private IDictionary _method2Advices = new Hashtable();

		public DefaultInvocationDispatcher(AspectDefinition aspect)
		{
			_aspect = aspect;
			_matcher = new DefaultJoinPointMatcher(aspect.PointCuts);
		}

		public void Init(AspectEngine engine)
		{
		}

		public void OnDeserialization(object sender)
		{
			_type2AdviceInstance = new Hashtable();
			_method2Advices = new Hashtable();
		}

		public object Intercept(Castle.DynamicProxy.IInvocation invocation, params object[] arguments)
		{
			IMethodInterceptor[] interceptors = ObtainAdvicesForMethod(invocation.Method,
			                                                           invocation.InvocationTarget, arguments);

			if (interceptors.Length == 0)
			{
				// Nothing to intercept. 
				// Get on with it!
				return invocation.Proceed(arguments);
			}

			InvocationComposite alliance_invocation = new InvocationComposite(
				interceptors, invocation, arguments);

			return alliance_invocation.Proceed();
		}

		private IMethodInterceptor[] ObtainAdvicesForMethod(MethodInfo method, object instance, object[] arguments)
		{
			IMethodInterceptor[] interceptors = ObtainFromCache(method);

			if (interceptors == null)
			{
				PointCutDefinition[] pointcuts = _matcher.Match(method);

				if (pointcuts.Length != 0)
				{
					interceptors = ObtainAdvices(pointcuts);
				}
				else
				{
					interceptors = new IMethodInterceptor[0];
					;
				}

				RegisterInCache(method, interceptors);
			}

			return interceptors;
		}

		protected IMethodInterceptor[] ObtainFromCache(MethodInfo method)
		{
			return (IMethodInterceptor[]) _method2Advices[method];
		}

		protected void RegisterInCache(MethodInfo method, IMethodInterceptor[] interceptors)
		{
			_method2Advices[method] = interceptors;
		}

		protected IMethodInterceptor[] ObtainAdvices(PointCutDefinition[] pointcuts)
		{
			ArrayList advices = new ArrayList();

			foreach (PointCutDefinition pointcut in pointcuts)
			{
				advices.AddRange(CreateInterceptor(pointcut.Advices));
			}

			return (IMethodInterceptor[]) advices.ToArray(typeof (IMethodInterceptor));
		}

		protected IMethodInterceptor[] CreateInterceptor(InterceptorDefinitionCollection advices)
		{
			IMethodInterceptor[] interceptors = new IMethodInterceptor[advices.Count];

			for (int i = 0; i < interceptors.Length; i++)
			{
				Type adviceType = advices[i].TypeReference.ResolvedType;
				interceptors[i] = ObtainInterceptorInstance(adviceType);
			}

			return interceptors;
		}

		protected virtual IMethodInterceptor ObtainInterceptorInstance(Type adviceType)
		{
			IMethodInterceptor interceptor = _type2AdviceInstance[adviceType] as IMethodInterceptor;

			if (interceptor == null)
			{
				interceptor = Activator.CreateInstance(adviceType) as IMethodInterceptor;
				InitializeInterceptor(interceptor);
				_type2AdviceInstance[adviceType] = interceptor;
			}

			return interceptor;
		}

		private void InitializeInterceptor(IMethodInterceptor interceptor)
		{
//			if (interceptor is IAspectEngineAware)
//			{
//				(interceptor as IAspectEngineAware).SetEngine(_engine);
//			}
		}
	}
}