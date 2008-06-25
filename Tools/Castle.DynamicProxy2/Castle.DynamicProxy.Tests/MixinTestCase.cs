// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Mixins;
	using NUnit.Framework;

	[TestFixture]
	public class MixinTestCase : BasePEVerifyTestCase
	{		
		private class AssertInvocationInterceptor: StandardInterceptor
		{
			public bool Invoked;
			public object proxy;
			public object mixin;

			protected override void PreProceed(IInvocation invocation)
			{
				Invoked = true;
				mixin = invocation.InvocationTarget;
				proxy = invocation.Proxy;
				base.PreProceed(invocation);
			}
		}

		public interface IMyInterface
		{
			String Name { get; set; }

			bool Started { get; set; }

			int Calc(int x, int y);

			int Calc(int x, int y, int z, Single k);
		}

		[Serializable]
		public class MyInterfaceImpl: IMyInterface
		{
			private String _name;
			private bool _started;

			public MyInterfaceImpl()
			{
			}

			public virtual String Name
			{
				get
				{
					return _name;
				}
				set
				{
					_name = value;
				}
			}

			public virtual bool Started
			{
				get
				{
					return _started;
				}
				set
				{
					_started = value;
				}
			}

			public virtual int Calc(int x, int y)
			{
				return x + y;
			}

			public virtual int Calc(int x, int y, int z, Single k)
			{
				return x + y + z + (int)k;
			}
		}

		[Test]
		public void SimpleMixin()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));

			Assert.IsFalse(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.IsNotNull(mixin);
			Assert.AreEqual(1, mixin.DoSomething());

			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin_instance, interceptor.mixin);
		}

		[Test]
		public void TwoMixins()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();

			SimpleMixin mixin1 = new SimpleMixin();
			OtherMixin mixin2 = new OtherMixin();

			proxyGenerationOptions.AddMixinInstance(mixin1);
			proxyGenerationOptions.AddMixinInstance(mixin2);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateClassProxy(
				typeof(SimpleClass), proxyGenerationOptions, interceptor);

			Assert.IsFalse(interceptor.Invoked);

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.IsNotNull(mixin);
			Assert.AreEqual(1, mixin.DoSomething());

			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin1, interceptor.mixin);

			IOtherMixin other = proxy as IOtherMixin;
			Assert.IsNotNull(other);
			Assert.AreEqual(3, other.Sum(1, 2));
			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin2, interceptor.mixin);
		}

		[Test]
		public void MixinForInterfaces()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();

			SimpleMixin mixin_instance = new SimpleMixin();
			proxyGenerationOptions.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			MyInterfaceImpl target = new MyInterfaceImpl();

			object proxy = generator.CreateInterfaceProxyWithTarget(
				typeof(IMyInterface), target, proxyGenerationOptions, interceptor);

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(IMyInterface).IsAssignableFrom(proxy.GetType()));

			Assert.IsFalse(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.IsNotNull(mixin);
			Assert.AreEqual(1, mixin.DoSomething());

			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin_instance, interceptor.mixin);
		}

		[Test]
		public void CanCreateSimpleMixinWithoutGettingExecutionEngineExceptionsOrBadImageExceptions()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();
			proxyGenerationOptions.AddMixinInstance(new SimpleMixin());
			object proxy = generator.CreateClassProxy(
				typeof(object), proxyGenerationOptions, new AssertInvocationInterceptor());

			Assert.IsTrue(proxy is ISimpleMixin);

			((ISimpleMixin)proxy).DoSomething();
		}

		[Test]
		public void MixinImplementingMoreThanOneInterface()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();

			ComplexMixin mixin_instance = new ComplexMixin();
			proxyGenerationOptions.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateClassProxy(
				typeof(SimpleClass), proxyGenerationOptions, interceptor);

			Assert.IsNotNull(proxy);
			Assert.IsTrue(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));

			Assert.IsFalse(interceptor.Invoked);

			IThird inter3 = proxy as IThird;
			Assert.IsNotNull(inter3);
			inter3.DoThird();

			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin_instance, interceptor.mixin);

			ISecond inter2 = proxy as ISecond;
			Assert.IsNotNull(inter2);
			inter2.DoSecond();

			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin_instance, interceptor.mixin);

			IFirst inter1 = proxy as IFirst;
			Assert.IsNotNull(inter1);
			inter1.DoFirst();

			Assert.IsTrue(interceptor.Invoked);
			Assert.AreSame(proxy, interceptor.proxy);
			Assert.AreSame(mixin_instance, interceptor.mixin);
		}
	}
}
