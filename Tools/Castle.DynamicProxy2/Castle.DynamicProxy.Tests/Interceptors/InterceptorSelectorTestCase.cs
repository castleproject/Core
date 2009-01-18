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

namespace Castle.DynamicProxy.Tests.InterceptorSelector
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Core.Interceptor;
	using NUnit.Framework;

	[TestFixture]
	public class InterceptorSelectorTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void BasicCase()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.Selector = new AllInterceptorSelector();
			ISimpleInterface target =
				generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface), new SimpleClass(), options,
				                                         new NoopInterceptor()) as ISimpleInterface;
			Assert.IsNotNull(target);
			target.Do();
		}

		[Test]
		public void SelectorWorksForGenericMethods()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			CallCountingInterceptor countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			IGenericInterface target =
				generator.CreateInterfaceProxyWithTarget(typeof(IGenericInterface), new GenericClass(), options,
				                                         new AddTwoInterceptor(),
				                                         countingInterceptor) as IGenericInterface;
			Assert.IsNotNull(target);
			int result = target.GenericMethod<int>();
			Assert.AreEqual(1, countingInterceptor.Count);
			Assert.AreEqual(0, result);
			string result2 = target.GenericMethod<string>();
			Assert.AreEqual(2, countingInterceptor.Count);
			Assert.AreEqual(default(string), result2);
		}

		[Test]
		public void SelectorWorksForMethods()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			CallCountingInterceptor countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			ISimpleInterface target =
				generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface), new SimpleClass(), options,
				                                         new AddTwoInterceptor(), countingInterceptor) as ISimpleInterface;
			Assert.IsNotNull(target);
			int result = target.Do();
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, countingInterceptor.Count);
		}

		[Test]
		public void SelectorWorksForMixins()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SimpleClass());
			CallCountingInterceptor countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			ISimpleInterface target =
				generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterfaceWithProperty),
				                                         new SimpleClassWithProperty(), options,
				                                         new AddTwoInterceptor(),
				                                         countingInterceptor) as ISimpleInterface;
			Assert.IsNotNull(target);
			int result = target.Do();
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, countingInterceptor.Count);
		}

		[Test]
		public void SelectorWorksForMultipleGenericMethods()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			CallCountingInterceptor countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			IMultiGenericInterface target =
				generator.CreateInterfaceProxyWithTarget(typeof(IMultiGenericInterface), new MultiGenericClass(),
				                                         options,
				                                         new AddTwoInterceptor(),
				                                         countingInterceptor) as IMultiGenericInterface;
			Assert.IsNotNull(target);
			int result = target.Method<int, string>("ignored");
			Assert.AreEqual(1, countingInterceptor.Count);
			Assert.AreEqual(0, result);
			string result2 = target.Method<string, int>(0);
			Assert.AreEqual(2, countingInterceptor.Count);
			Assert.AreEqual(default(string), result2);
		}

		[Test]
		public void SelectorWorksForProperties()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			CallCountingInterceptor countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			ISimpleInterfaceWithProperty target =
				generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterfaceWithProperty),
				                                         new SimpleClassWithProperty(), options,
				                                         new AddTwoInterceptor(),
				                                         countingInterceptor) as ISimpleInterfaceWithProperty;
			Assert.IsNotNull(target);
			int result = target.Age;
			Assert.AreEqual(5, result);
			Assert.AreEqual(1, countingInterceptor.Count);
		}
	}

	public class MultiGenericClass : IMultiGenericInterface
	{
		#region IMultiGenericInterface Members

		public T1 Method<T1, T2>(T2 p)
		{
			return default(T1);
		}

		public T2 Method<T1, T2>(T1 p)
		{
			return default(T2);
		}

		#endregion
	}

	public interface IMultiGenericInterface
	{
		T1 Method<T1, T2>(T2 p);
		T2 Method<T1, T2>(T1 p);
	}

	public class GenericClass : IGenericInterface
	{
		#region IGenericInterface Members

		public T GenericMethod<T>()
		{
			return default(T);
		}

		#endregion
	}

	public interface IGenericInterface
	{
		T GenericMethod<T>();
	}

	public interface ISimpleInterfaceWithProperty
	{
		int Age { get; }
	}

	public class SimpleClassWithProperty : ISimpleInterfaceWithProperty
	{
		#region ISimpleInterfaceWithProperty Members

		public int Age
		{
			get { return 5; }
		}

		#endregion
	}

	[Serializable]
	internal class TypeInterceptorSelector<TInterceptor> : IInterceptorSelector where TInterceptor : IInterceptor
	{
		#region IInterceptorSelector Members

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			List<IInterceptor> interceptorsOfT = new List<IInterceptor>();
			foreach (IInterceptor interceptor in interceptors)
			{
				if (interceptor is TInterceptor)
				{
					interceptorsOfT.Add(interceptor);
				}
			}
			return interceptorsOfT.ToArray();
		}

		#endregion
	}

	[Serializable]
	public class CallCountingInterceptor : IInterceptor
	{
		private int _count;

		public int Count
		{
			get { return _count; }
		}

		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			_count++;
			invocation.Proceed();
		}

		#endregion
	}

	[Serializable]
	public class AddTwoInterceptor : IInterceptor
	{
		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			invocation.Proceed();
			int ret = (int)invocation.ReturnValue;
			ret += 2;
			invocation.ReturnValue = ret;
		}

		#endregion
	}

	[Serializable]
	internal class NoopInterceptor : IInterceptor
	{
		#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			invocation.Proceed();
		}

		#endregion
	}

	[Serializable]
	public class AllInterceptorSelector : IInterceptorSelector
	{
		#region IInterceptorSelector Members

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			return interceptors;
		}

		#endregion
	}

	[Serializable]
	public class SimpleClass : ISimpleInterface
	{
		#region ISimpleInterface Members

		public int Do()
		{
			return 3;
		}

		#endregion
	}

	public interface ISimpleInterface
	{
		int Do();
	}
}