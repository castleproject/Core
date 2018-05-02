// Copyright 2004-2014 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
#if FEATURE_SERIALIZATION
	using System.Xml.Serialization;
#endif

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class InterceptorSelectorTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void BasicCase()
		{
			var options = new ProxyGenerationOptions();
			options.Selector = new AllInterceptorSelector();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface), new SimpleClass(), options, new StandardInterceptor()) as ISimpleInterface;
			Assert.IsNotNull(target);
			target.Do();
		}

		[Test]
		public void SelectorWorksForGenericMethods()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(IGenericInterface), new GenericClass(), options,
			                                                      new AddTwoInterceptor(),
			                                                      countingInterceptor) as IGenericInterface;
			Assert.IsNotNull(target);
			var result = target.GenericMethod<int>();
			Assert.AreEqual(1, countingInterceptor.Count);
			Assert.AreEqual(0, result);
			var result2 = target.GenericMethod<string>();
			Assert.AreEqual(2, countingInterceptor.Count);
			Assert.AreEqual(default(string), result2);
		}

		[Test]
		public void SelectorWorksForMethods()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterface), new SimpleClass(), options,
			                                                      new AddTwoInterceptor(), countingInterceptor) as ISimpleInterface;
			Assert.IsNotNull(target);
			var result = target.Do();
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, countingInterceptor.Count);
		}

		[Test]
		public void SelectorWorksForMixins()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new SimpleClass());
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterfaceWithProperty),
			                                                      new SimpleClassWithProperty(), options,
			                                                      new AddTwoInterceptor(),
			                                                      countingInterceptor) as ISimpleInterface;
			Assert.IsNotNull(target);
			var result = target.Do();
			Assert.AreEqual(3, result);
			Assert.AreEqual(1, countingInterceptor.Count);
		}

		[Test]
		public void SelectorWorksForMultipleGenericMethods()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(IMultiGenericInterface), new MultiGenericClass(),
			                                                      options,
			                                                      new AddTwoInterceptor(),
			                                                      countingInterceptor) as IMultiGenericInterface;
			Assert.IsNotNull(target);
			var result = target.Method<int, string>("ignored");
			Assert.AreEqual(1, countingInterceptor.Count);
			Assert.AreEqual(0, result);
			var result2 = target.Method<string, int>(0);
			Assert.AreEqual(2, countingInterceptor.Count);
			Assert.AreEqual(default(string), result2);
		}

		[Test]
		public void SelectorWorksForProperties()
		{
			var options = new ProxyGenerationOptions();
			var countingInterceptor = new CallCountingInterceptor();
			options.Selector = new TypeInterceptorSelector<CallCountingInterceptor>();
			var target = generator.CreateInterfaceProxyWithTarget(typeof(ISimpleInterfaceWithProperty),
			                                                      new SimpleClassWithProperty(), options,
			                                                      new AddTwoInterceptor(),
			                                                      countingInterceptor) as ISimpleInterfaceWithProperty;
			Assert.IsNotNull(target);
			var result = target.Age;
			Assert.AreEqual(5, result);
			Assert.AreEqual(1, countingInterceptor.Count);
		}

		[Test]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_InterfaceProxyWithTargetInterface()
		{
			var options1 = new ProxyGenerationOptions { Selector = new AllInterceptorSelector() };
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<CallCountingInterceptor>()
			};

			var proxy1 = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(), options1);
			var proxy2 = generator.CreateInterfaceProxyWithTargetInterface<IOne>(new One(), options2);
			proxy1.OneMethod();
			proxy2.OneMethod();

			Assert.AreSame(proxy1.GetType(), proxy2.GetType());
		}

		[Test]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_InterfaceProxyWithTarget()
		{
			var options1 = new ProxyGenerationOptions { Selector = new AllInterceptorSelector() };
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<CallCountingInterceptor>()
			};

			var proxy1 = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), options1);
			var proxy2 = generator.CreateInterfaceProxyWithTarget<IOne>(new One(), options2);
			proxy1.OneMethod();
			proxy2.OneMethod();

			Assert.AreSame(proxy1.GetType(), proxy2.GetType());
		}

		[Test]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_InterfaceProxyWithoutTarget()
		{
			var options1 = new ProxyGenerationOptions { Selector = new AllInterceptorSelector() };
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<SetReturnValueInterceptor>()
			};

			var proxy1 = generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), Type.EmptyTypes, options1, new SetReturnValueInterceptor(2));
			var proxy2 = generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), Type.EmptyTypes, options2, new SetReturnValueInterceptor(2));
			(proxy1 as IOne).OneMethod();
			(proxy2 as IOne).OneMethod();

			Assert.AreSame(proxy1.GetType(), proxy2.GetType());
		}

		[Test]
		public void When_two_selectors_present_and_not_equal_should_cache_type_anyway_ClassProxy()
		{
			var options1 = new ProxyGenerationOptions { Selector = new AllInterceptorSelector() };
			var options2 = new ProxyGenerationOptions
			{
				Selector = new TypeInterceptorSelector<CallCountingInterceptor>()
			};

			var proxy1 = generator.CreateClassProxy(typeof(ServiceClass), Type.EmptyTypes, options1);
			var proxy2 = generator.CreateClassProxy(typeof(ServiceClass), Type.EmptyTypes, options2);
			(proxy1 as ServiceClass).Sum(2, 2);
			(proxy2 as ServiceClass).Sum(2, 2);

			Assert.AreSame(proxy1.GetType(), proxy2.GetType());
		}

		[Test]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithoutTarget()
		{
			var someInstanceOfProxyWithoutSelector = (IService2)generator.CreateInterfaceProxyWithoutTarget(typeof(IService2), new DoNothingInterceptor());
			var someInstanceOfProxyWithSelector = (IService2)generator.CreateInterfaceProxyWithoutTarget(typeof(IService2), new ProxyGenerationOptions
			{
				Selector = new AllInterceptorSelector()
			}, new DoNothingInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Test]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithTarget()
		{
			var someInstanceOfProxyWithoutSelector = (IService2)generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (IService2)generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(),
			                                                                                          new ProxyGenerationOptions { Selector = new AllInterceptorSelector() },
			                                                                                          new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Test]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithTargetInterface()
		{
			var someInstanceOfProxyWithoutSelector = (IService2)generator.CreateInterfaceProxyWithTargetInterface(typeof(IService2), new Service2(), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (IService2)generator.CreateInterfaceProxyWithTargetInterface(typeof(IService2), new Service2(),
			                                                                                                   new ProxyGenerationOptions { Selector = new AllInterceptorSelector() },
			                                                                                                   new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Test]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_ClassProxy()
		{
			var someInstanceOfProxyWithoutSelector = (Component2)generator.CreateClassProxy(typeof(Component2), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (Component2)generator.CreateClassProxy(typeof(Component2),
			                                                                             new ProxyGenerationOptions { Selector = new AllInterceptorSelector() },
			                                                                             new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Test]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_ClassProxyWithTarget()
		{
			var someInstanceOfProxyWithoutSelector = (Component2)generator.CreateClassProxyWithTarget(typeof(Component2), new Component2(), new StandardInterceptor());
			var someInstanceOfProxyWithSelector = (Component2)generator.CreateClassProxyWithTarget(typeof(Component2), new Component2(),
			                                                                                       new ProxyGenerationOptions { Selector = new AllInterceptorSelector() },
			                                                                                       new StandardInterceptor());

			// This runs fine
			someInstanceOfProxyWithoutSelector.DoOperation2();
			// This will throw System.InvalidProgramException
			someInstanceOfProxyWithSelector.DoOperation2();
		}

		[Test]
		[Bug("DYNPROXY-175")]
		public void Can_proxy_same_type_with_and_without_selector_InterfaceProxyWithTarget2()
		{
			var someInstanceOfProxyWithSelector1 = (IService2)generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(),
			                                                                                           new ProxyGenerationOptions { Selector = new SelectorWithState(1) },
			                                                                                           new StandardInterceptor());
			var someInstanceOfProxyWithSelector2 = (IService2)generator.CreateInterfaceProxyWithTarget(typeof(IService2), new Service2(),
			                                                                                           new ProxyGenerationOptions { Selector = new SelectorWithState(2) },
			                                                                                           new StandardInterceptor());

			Assert.AreSame(someInstanceOfProxyWithSelector1.GetType(), someInstanceOfProxyWithSelector2.GetType());
		}

		[Test]
		public void Cannot_proxy_inaccessible_interface()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateInterfaceProxyWithTarget<PrivateInterface>(new PrivateClass(), new IInterceptor[0]));
			StringAssert.StartsWith(
				"Can not create proxy for type Castle.DynamicProxy.Tests.InterceptorSelectorTestCase+PrivateInterface because it is not accessible. Make it public, or internal",
				ex.Message);
		}

		[Test]
		public void Cannot_proxy_generic_interface_with_inaccessible_type_argument()
		{
			var ex = Assert.Throws<GeneratorException>(() =>
				generator.CreateInterfaceProxyWithTarget<IList<PrivateInterface>>(new List<PrivateInterface>(), new IInterceptor[0]));
			StringAssert.StartsWith(
				"Can not create proxy for type System.Collections.Generic.IList`1[[Castle.DynamicProxy.Tests.InterceptorSelectorTestCase+PrivateInterface, Castle.Core.Tests, Version=0.0.0.0, Culture=neutral, PublicKeyToken=407dd0808d44fbdc]] because type Castle.DynamicProxy.Tests.InterceptorSelectorTestCase+PrivateInterface is not accessible. Make it public, or internal",
				ex.Message);
		}

		[Test]
		public void Cannot_proxy_generic_interface_with_type_argument_that_has_inaccessible_type_argument()
		{
			Assert.Throws<GeneratorException>(() => generator.CreateInterfaceProxyWithTarget<IList<IList<PrivateInterface>>>(new List<IList<PrivateInterface>>(), new IInterceptor[0]));
		}

		[Test]
		public void Can_proxy_generic_interface()
		{
			generator.CreateInterfaceProxyWithTarget<IList<object>>(new List<object>(), new IInterceptor[0]);
		}

		private interface PrivateInterface { }

		private class PrivateClass : PrivateInterface { }
	}

	public class MultiGenericClass : IMultiGenericInterface
	{
		public T1 Method<T1, T2>(T2 p)
		{
			return default(T1);
		}

		public T2 Method<T1, T2>(T1 p)
		{
			return default(T2);
		}
	}

	public interface IMultiGenericInterface
	{
		T1 Method<T1, T2>(T2 p);

		T2 Method<T1, T2>(T1 p);
	}

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
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
		public int Age
		{
			get { return 5; }
		}
	}

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	internal class TypeInterceptorSelector<TInterceptor> : IInterceptorSelector where TInterceptor : IInterceptor
	{
		#region IInterceptorSelector Members

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			var interceptorsOfT = new List<IInterceptor>();
			foreach (var interceptor in interceptors)
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

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class AllInterceptorSelector : IInterceptorSelector
	{
		#region IInterceptorSelector Members

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			return interceptors;
		}

		#endregion
	}

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
	public class SelectorWithState : IInterceptorSelector
	{
		private readonly int state;

		public SelectorWithState(int state)
		{
			this.state = state;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != GetType())
			{
				return false;
			}
			return Equals((SelectorWithState)obj);
		}

		public override int GetHashCode()
		{
			return state;
		}

		public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
		{
			return interceptors;
		}

		protected bool Equals(SelectorWithState other)
		{
			return state == other.state;
		}
	}

#if FEATURE_SERIALIZATION
	[Serializable]
#endif
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

	public class FakeProxy
	{
		// Fields
		public static ProxyGenerationOptions proxyGenerationOptions;
		public static MethodInfo token_Do;

#if FEATURE_SERIALIZATION
		[XmlIgnore]
#endif
		public IInterceptor[] __interceptors;

		public IInterceptorSelector __selector;

#if FEATURE_SERIALIZATION
		[XmlIgnore]
#endif
		public SimpleClass __target;

#if FEATURE_SERIALIZATION
		[NonSerialized]
		[XmlIgnore]
#endif
		public IInterceptor[] interceptors_Do;

		public virtual int Do()
		{
			// This item is obfuscated and can not be translated.
			if (interceptors_Do == null)
			{
				interceptors_Do = __selector.SelectInterceptors(TypeUtil.GetTypeOrNull(__target), token_Do, __interceptors) ?? new IInterceptor[0];
			}
			var objArray = new object[0];
			var @do = new ISimpleInterface_Do(__target, this, interceptors_Do, token_Do, objArray);
			@do.Proceed();
			return (int)@do.ReturnValue;
		}
	}

	public class ISimpleInterface_Do
	{
		public ISimpleInterface_Do(SimpleClass simpleClass, FakeProxy fakeProxy, IInterceptor[] interceptorsDo, MethodInfo tokenDo, object[] objArray)
		{
			throw new NotImplementedException();
		}

		public object ReturnValue
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public void Proceed()
		{
			throw new NotImplementedException();
		}
	}
}