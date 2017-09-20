// Copyright 2004-2017 Castle Project - http://www.castleproject.org/
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

	using NUnit.Framework;

	public class LazyInterceptorV1<T> : IInterceptor
		where T : class
	{
		public LazyInterceptorV1(Lazy<T> lazyTarget)
		{
			LazyTarget = lazyTarget;
		}

		private Lazy<T> LazyTarget { get; }

		public void Intercept(IInvocation invocation)
		{
			var target = invocation.InvocationTarget as T;
			if (target == null)
			{
				((IChangeProxyTarget)invocation).ChangeInvocationTarget(LazyTarget.Value);
				((IProxyTargetAccessor)invocation.Proxy).DynProxySetTarget(LazyTarget.Value);
			}

			invocation.Proceed();
		}
	}

	public class LazyInterceptorV2<T> : IInterceptor
		where T : class
	{
		public LazyInterceptorV2(Lazy<T> lazyTarget)
		{
			LazyTarget = lazyTarget;
		}

		private Lazy<T> LazyTarget { get; }

		public void Intercept(IInvocation invocation)
		{
			var target = invocation.InvocationTarget as T;
			if (target == null)
			{
				(invocation as IChangeProxyTarget).ChangeInvocationTarget(LazyTarget.Value);
#pragma warning disable CS0618 // obsolete: use DynProxySetTarget instead
				(invocation as IChangeProxyTarget).ChangeProxyTarget(LazyTarget.Value);
#pragma warning restore CS0618 // obsolete
			}

			invocation.Proceed();
		}
	}

	public interface IEventHandler
	{
		string Handle(EventArgs ea);
	}

	public interface IEventHandler<T> : IEventHandler
		where T : EventArgs
	{
	}

	public class EventArgs1 : EventArgs { }

	public class Handler1 : IEventHandler<EventArgs1>
	{
		public string Handle(EventArgs ea)
		{
			return "Handler1";
		}
	}

	public class EventArgs2 : EventArgs { }

	public class Handler2 : IEventHandler<EventArgs2>
	{
		public string Handle(EventArgs ea)
		{
			return "Handler2";
		}
	}

	public interface IEventHandler3 : IEventHandler { }

	public class Handler3 : IEventHandler3
	{
		public string Handle(EventArgs ea)
		{
			return "Handler3";
		}
	}

	[TestFixture]
	public class ChangeProxyTargetTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Should_Change_Target_Proxy_For_Generic_Interfaces_Recommended_Approach()
		{
			var lazyTarget1 = new Lazy<IEventHandler<EventArgs1>>(() => new Handler1());
			var lazyInterceptor1 = new LazyInterceptorV1<IEventHandler<EventArgs1>>(lazyTarget1);
			var proxy1 = generator.CreateInterfaceProxyWithTargetInterface<IEventHandler<EventArgs1>>(null, new[] { lazyInterceptor1 });

			var result1 = proxy1.Handle(EventArgs.Empty);
			Assert.AreEqual("Handler1", result1);

			var lazyTarget2 = new Lazy<IEventHandler<EventArgs2>>(() => new Handler2());
			var lazyInterceptor2 = new LazyInterceptorV1<IEventHandler<EventArgs2>>(lazyTarget2);
			var proxy2 = generator.CreateInterfaceProxyWithTargetInterface<IEventHandler<EventArgs2>>(null, new[] { lazyInterceptor2 });

			var result2 = proxy2.Handle(EventArgs.Empty);
			Assert.AreEqual("Handler2", result2);

			var lazyTarget3 = new Lazy<IEventHandler3>(() => new Handler3());
			var lazyInterceptor3 = new LazyInterceptorV1<IEventHandler3>(lazyTarget3);
			var proxy3 = generator.CreateInterfaceProxyWithTargetInterface<IEventHandler3>(null, new[] { lazyInterceptor3 });

			var result3 = proxy3.Handle(EventArgs.Empty);
			Assert.AreEqual("Handler3", result3);
		}

		[Test]
		public void Should_Change_Target_Proxy_For_Generic_Interfaces_Obsolete_Approach()
		{
			// issue #293
			var lazyTarget1 = new Lazy<IEventHandler<EventArgs1>>(() => new Handler1());
			var lazyInterceptor1 = new LazyInterceptorV2<IEventHandler<EventArgs1>>(lazyTarget1);
			var proxy1 = generator.CreateInterfaceProxyWithTargetInterface<IEventHandler<EventArgs1>>(null, new[] { lazyInterceptor1 });

			var result1 = proxy1.Handle(EventArgs.Empty);
			Assert.AreEqual("Handler1", result1);

			var lazyTarget2 = new Lazy<IEventHandler<EventArgs2>>(() => new Handler2());
			var lazyInterceptor2 = new LazyInterceptorV2<IEventHandler<EventArgs2>>(lazyTarget2);
			var proxy2 = generator.CreateInterfaceProxyWithTargetInterface<IEventHandler<EventArgs2>>(null, new[] { lazyInterceptor2 });

			var result2 = proxy2.Handle(EventArgs.Empty);
			Assert.AreEqual("Handler2", result2);

			var lazyTarget3 = new Lazy<IEventHandler3>(() => new Handler3());
			var lazyInterceptor3 = new LazyInterceptorV2<IEventHandler3>(lazyTarget3);
			var proxy3 = generator.CreateInterfaceProxyWithTargetInterface<IEventHandler3>(null, new[] { lazyInterceptor3 });

			var result3 = proxy3.Handle(EventArgs.Empty);
			Assert.AreEqual("Handler3", result3);
		}
	}
}