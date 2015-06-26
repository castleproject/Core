// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Mixins;
	using Castle.DynamicProxy.Tests.InterClasses;

	using Interceptors;

	using Xunit;

	using System.Reflection;

	public class MixinTestCase : BasePEVerifyTestCase
	{
		private class AssertInvocationInterceptor : StandardInterceptor
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

#if !SILVERLIGHT && !NETCORE
		[Serializable]
#endif
		public class MyInterfaceImpl : IMyInterface
		{
			private String _name;
			private bool _started;

			public MyInterfaceImpl()
			{
			}

			public virtual String Name
			{
				get { return _name; }
				set { _name = value; }
			}

			public virtual bool Started
			{
				get { return _started; }
				set { _started = value; }
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

		[Fact]
		public void SimpleMixin_ClassProxy()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void SimpleMixin_InterfaceProxy_WithTarget()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new ServiceImpl(),
				options, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IService).IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void SimpleMixin_InterfaceProxy_WithoutTarget()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IService), new Type[0],
				options, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IService).IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void SimpleMixin_InterfaceProxy_WithtTargetInterface()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateInterfaceProxyWithTargetInterface(typeof(IService), new ServiceImpl(), options,
				interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IService).IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
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

			Assert.False(interceptor.Invoked);

			Assert.NotNull(proxy);
			Assert.True(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin1, interceptor.mixin);

			IOtherMixin other = proxy as IOtherMixin;
			Assert.NotNull(other);
			Assert.Equal(3, other.Sum(1, 2));
			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin2, interceptor.mixin);
		}

		[Fact]
		public void MixinForInterfaces()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();

			SimpleMixin mixin_instance = new SimpleMixin();
			proxyGenerationOptions.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			MyInterfaceImpl target = new MyInterfaceImpl();

			object proxy = generator.CreateInterfaceProxyWithTarget(
				typeof(IMyInterface), target, proxyGenerationOptions, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(IMyInterface).IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			ISimpleMixin mixin = proxy as ISimpleMixin;
			Assert.NotNull(mixin);
			Assert.Equal(1, mixin.DoSomething());

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void CanCreateSimpleMixinWithoutGettingExecutionEngineExceptionsOrBadImageExceptions()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();
			proxyGenerationOptions.AddMixinInstance(new SimpleMixin());
			object proxy = generator.CreateClassProxy(
				typeof(object), proxyGenerationOptions, new AssertInvocationInterceptor());

			Assert.True(proxy is ISimpleMixin);

			((ISimpleMixin)proxy).DoSomething();
		}

		[Fact]
		public void MixinImplementingMoreThanOneInterface()
		{
			ProxyGenerationOptions proxyGenerationOptions = new ProxyGenerationOptions();

			ComplexMixin mixin_instance = new ComplexMixin();
			proxyGenerationOptions.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy = generator.CreateClassProxy(
				typeof(SimpleClass), proxyGenerationOptions, interceptor);

			Assert.NotNull(proxy);
			Assert.True(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));

			Assert.False(interceptor.Invoked);

			IThird inter3 = proxy as IThird;
			Assert.NotNull(inter3);
			inter3.DoThird();

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);

			ISecond inter2 = proxy as ISecond;
			Assert.NotNull(inter2);
			inter2.DoSecond();

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);

			IFirst inter1 = proxy as IFirst;
			Assert.NotNull(inter1);
			inter1.DoFirst();

			Assert.True(interceptor.Invoked);
			Assert.Same(proxy, interceptor.proxy);
			Assert.Same(mixin_instance, interceptor.mixin);
		}

		[Fact]
		public void TestTypeCachingWithMixins()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy1 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			options = new ProxyGenerationOptions();
			mixin_instance = new SimpleMixin();
			options.AddMixinInstance(mixin_instance);

			interceptor = new AssertInvocationInterceptor();

			object proxy2 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.True(proxy1.GetType().Equals(proxy2.GetType()));
		}

		[Fact]
		public void TestTypeCachingWithMultipleMixins()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin_instance1 = new SimpleMixin();
			ComplexMixin mixin_instance2 = new ComplexMixin();
			options.AddMixinInstance(mixin_instance1);
			options.AddMixinInstance(mixin_instance2);

			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();

			object proxy1 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			options = new ProxyGenerationOptions();
			mixin_instance1 = new SimpleMixin();
			mixin_instance2 = new ComplexMixin();
			options.AddMixinInstance(mixin_instance2);
			options.AddMixinInstance(mixin_instance1);

			interceptor = new AssertInvocationInterceptor();

			object proxy2 = generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);

			Assert.True(proxy1.GetType().Equals(proxy2.GetType()));
		}

		[Fact]
		public void TwoMixinsWithSameInterface()
		{
			// TODO: find where the exception should be thrown
			Assert.Throws<InvalidMixinConfigurationException>(() =>
			{
				ProxyGenerationOptions options = new ProxyGenerationOptions();
				SimpleMixin mixin1 = new SimpleMixin();
				OtherMixinImplementingISimpleMixin mixin2 = new OtherMixinImplementingISimpleMixin();
				options.AddMixinInstance(mixin1);
				options.AddMixinInstance(mixin2);

				StandardInterceptor interceptor = new StandardInterceptor();
				generator.CreateClassProxy(typeof(SimpleClass), options, interceptor);
			});
		}

		[Fact]
		public void MixinWithSameInterface_InterfaceWithTarget_AdditionalInterfaces()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin1 = new SimpleMixin();
			options.AddMixinInstance(mixin1);

			StandardInterceptor interceptor = new StandardInterceptor();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new Type[] { typeof(ISimpleMixin) },
				new ServiceImpl(), options, interceptor);
			Assert.Equal(1, (proxy as ISimpleMixin).DoSomething());
		}

		[Fact]
		public void MixinWithSameInterface_InterfaceWithTarget_AdditionalInterfaces_Derived()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			SimpleMixin mixin1 = new SimpleMixin();
			options.AddMixinInstance(mixin1);

			StandardInterceptor interceptor = new StandardInterceptor();
			var proxy = generator.CreateInterfaceProxyWithTarget(typeof(IService), new Type[] { typeof(IDerivedSimpleMixin) },
				new ServiceImpl(), options, interceptor);
			Assert.Equal(1, (proxy as ISimpleMixin).DoSomething());
		}
	}
}