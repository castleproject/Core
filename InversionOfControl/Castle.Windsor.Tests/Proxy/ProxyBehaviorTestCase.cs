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

#if !SILVERLIGHT // we do not support xml config on SL

namespace Castle.Windsor.Tests.Proxy
{
	using System;
	using Castle.Core.Interceptor;
	using Castle.Windsor.Tests.Components;
	using Castle.MicroKernel.ComponentActivator;
	using NUnit.Framework;

	[TestFixture]
	public class ProxyBehaviorTestCase
	{
		[Test]
		public void DefaultProxyBehaviorFromConfiguration()
		{
			IWindsorContainer container;

			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/proxyBehavior.xml"));

			ICalcService calcService = (ICalcService) container["default"];
			Assert.IsNotNull(calcService);
			Assert.IsTrue(calcService is IDisposable, "Service proxy should expose the IDisposable interface");
		}

		[Test]
		public void NoSingleInterfaceProxyBehaviorFromConfiguration()
		{
			IWindsorContainer container;

			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/proxyBehavior.xml"));

			ICalcService calcService = (ICalcService) container["noSingle"];
			Assert.IsNotNull(calcService);
			Assert.IsTrue(calcService is IDisposable, "Service proxy should expose the IDisposable interface");
		}

		[Test]
		public void UseSingleInterfaceProxyBehaviorFromConfiguration()
		{
			IWindsorContainer container;

			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/proxyBehavior.xml"));

			ICalcService calcService = (ICalcService) container["useSingle"];
			Assert.IsNotNull(calcService);
			Assert.IsFalse(calcService is IDisposable, "Service proxy should not expose the IDisposable interface");
		}

		[Test]
		public void UseSingleInterfaceProxyBehaviorFromAttribute()
		{
			IWindsorContainer container;

			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/proxyBehavior.xml"));

			ICalcService calcService = (ICalcService) container["useSingleAttribute"];
			Assert.IsFalse(calcService is IDisposable, "Service proxy should not expose the IDisposable interface");
		}

		[Test]
		public void RequestSingleInterfaceProxyWithAttribute()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("standard.interceptor", typeof(StandardInterceptor));
			container.AddComponent("useSingle", typeof(ICalcService), typeof(CalculatorServiceWithSingleProxyBehavior));

			ICalcService calcService = (ICalcService) container["useSingle"];
			Assert.IsNotNull(calcService);
			Assert.IsFalse(calcService is IDisposable, "Service proxy should not expose the IDisposable interface");
		}

		[Test]
		public void NoSingleInterfaceProxyWithAttribute()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("standard.interceptor", typeof(StandardInterceptor));
			container.AddComponent("noSingle", typeof(ICalcService), typeof(CalculatorServiceWithoutSingleProxyBehavior));

			ICalcService calcService = (ICalcService) container["noSingle"];
			Assert.IsNotNull(calcService);
			Assert.IsTrue(calcService is IDisposable, "Service proxy should expose the IDisposable interface");
		}

		[Test]
		public void RequestMarshalByRefProxyWithAttribute()
		{
			IWindsorContainer container = new WindsorContainer();

			container.AddComponent("standard.interceptor", typeof(StandardInterceptor));
			container.AddComponent("useMarshal", typeof(ICalcService), typeof(CalculatorServiceWithMarshalByRefProxyBehavior));

			ICalcService calcService = (ICalcService)container["useMarshal"];
			Assert.IsNotNull(calcService);
			Assert.IsFalse(calcService is CalculatorServiceWithMarshalByRefProxyBehavior, "Service proxy should not expose CalculatorServiceWithMarshalByRefProxyBehavior");
			Assert.IsTrue(calcService is MarshalByRefObject, "Service proxy should expose MarshalByRefObject");
			Assert.IsTrue(calcService is IDisposable, "Service proxy should expose the IDisposable interface");
		}

		[Test]
		public void InternalInterfaceIgnoredByProxy()
		{
			IWindsorContainer container;

			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("Proxy/proxyBehavior.xml"));

			try
			{
				ICalcService calcService = (ICalcService)container["hasInternalInterface"];
			}
			catch (ComponentActivatorException)
			{
				Assert.Fail("Internal interface should be ignored by proxy factory.");
			}
		}
	}
}

#endif