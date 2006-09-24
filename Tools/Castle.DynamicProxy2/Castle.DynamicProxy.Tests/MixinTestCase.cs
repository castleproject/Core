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

namespace Castle.DynamicProxy.Tests
{
	using NUnit.Framework;

	[TestFixture]
	public class MixinTestCase : BasePEVerifyTestCase
	{
		private ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

//		[Test]
//		public void SimpleMixin()
//		{
//			GeneratorContext context = new GeneratorContext();
//			SimpleMixin mixin_instance = new SimpleMixin();
//			context.AddMixinInstance(mixin_instance);
//
//			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();
//
//			object proxy = generator.CreateCustomClassProxy(
//				typeof(SimpleClass), interceptor, context);
//
//			Assert.IsNotNull(proxy);
//			Assert.IsTrue(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));
//
//			Assert.IsFalse(interceptor.Invoked);
//
//			ISimpleMixin mixin = proxy as ISimpleMixin;
//			Assert.IsNotNull(mixin);
//			Assert.AreEqual(1, mixin.DoSomething());
//
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin_instance, interceptor.mixin);
//		}
//
//		[Test]
//		public void TwoMixins()
//		{
//			GeneratorContext context = new GeneratorContext();
//			SimpleMixin mixin1 = new SimpleMixin();
//			OtherMixin mixin2 = new OtherMixin();
//
//			context.AddMixinInstance(mixin1);
//			context.AddMixinInstance(mixin2);
//
//			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();
//
//			object proxy = generator.CreateCustomClassProxy(
//				typeof(SimpleClass), interceptor, context);
//
//			Assert.IsFalse(interceptor.Invoked);
//
//			Assert.IsNotNull(proxy);
//			Assert.IsTrue(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));
//
//			ISimpleMixin mixin = proxy as ISimpleMixin;
//			Assert.IsNotNull(mixin);
//			Assert.AreEqual(1, mixin.DoSomething());
//
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin1, interceptor.mixin);
//
//			IOtherMixin other = proxy as IOtherMixin;
//			Assert.IsNotNull(other);
//			Assert.AreEqual(3, other.Sum(1, 2));
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin2, interceptor.mixin);
//
//		}
//
//		[Test]
//		public void MixinImplementingMoreThanOneInterface()
//		{
//			GeneratorContext context = new GeneratorContext();
//			ComplexMixin mixin_instance = new ComplexMixin();
//			context.AddMixinInstance(mixin_instance);
//
//			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();
//
//			object proxy = generator.CreateCustomClassProxy(
//				typeof(SimpleClass), interceptor, context);
//
//			Assert.IsNotNull(proxy);
//			Assert.IsTrue(typeof(SimpleClass).IsAssignableFrom(proxy.GetType()));
//
//			Assert.IsFalse(interceptor.Invoked);
//
//			IThird inter3 = proxy as IThird;
//			Assert.IsNotNull(inter3);
//			inter3.DoThird();
//
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin_instance, interceptor.mixin);
//
//			ISecond inter2 = proxy as ISecond;
//			Assert.IsNotNull(inter2);
//			inter2.DoSecond();
//
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin_instance, interceptor.mixin);
//
//			IFirst inter1 = proxy as IFirst;
//			Assert.IsNotNull(inter1);
//			inter1.DoFirst();
//
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin_instance, interceptor.mixin);
//		}
//
//		[Test]
//		public void MixinForInterfaces()
//		{
//			GeneratorContext context = new GeneratorContext();
//			SimpleMixin mixin_instance = new SimpleMixin();
//			context.AddMixinInstance(mixin_instance);
//
//			AssertInvocationInterceptor interceptor = new AssertInvocationInterceptor();
//
//			MyInterfaceImpl target = new MyInterfaceImpl();
//
//			object proxy = generator.CreateCustomProxy(
//				typeof(IMyInterface), interceptor, target, context);
//
//			Assert.IsNotNull(proxy);
//			Assert.IsTrue(typeof(IMyInterface).IsAssignableFrom(proxy.GetType()));
//
//			Assert.IsFalse(interceptor.Invoked);
//
//			ISimpleMixin mixin = proxy as ISimpleMixin;
//			Assert.IsNotNull(mixin);
//			Assert.AreEqual(1, mixin.DoSomething());
//
//			Assert.IsTrue(interceptor.Invoked);
//			Assert.AreSame(proxy, interceptor.proxy);
//			Assert.AreSame(mixin_instance, interceptor.mixin);
//		}
	}

//	public class AssertInvocationInterceptor : StandardInterceptor
//	{
//		public bool Invoked;
//		public object proxy;
//		public object mixin;
//
//		protected override void PreProceed(IInvocation invocation, params object[] args)
//		{
//			Invoked = true;
//			mixin = invocation.InvocationTarget;
//			proxy = invocation.Proxy;
//
//			base.PreProceed(invocation, args);
//		}
//	}
}
