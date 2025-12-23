// Copyright 2004-2025 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class CaseSensitivityTestCase : BasePEVerifyTestCase
	{
		[TestCase(typeof(IDifferentlyCasedEvents))]
		[TestCase(typeof(IDifferentlyCasedMethods))]
		[TestCase(typeof(IDifferentlyCasedProperties))]
		public void Can_proxy_type_with_differently_cased_members(Type interfaceTypeToProxy)
		{
			_ = generator.CreateInterfaceProxyWithoutTarget(interfaceTypeToProxy);
		}

		[Test]
		public void Can_distinguish_differently_cased_events_during_interception()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IDifferentlyCasedEvents>(interceptor);

			proxy.Abc += delegate { };
			Assert.AreEqual("add_Abc", interceptor.Invocation.Method.Name);

			proxy.aBc += delegate { };
			Assert.AreEqual("add_aBc", interceptor.Invocation.Method.Name);

			proxy.abC += delegate { };
			Assert.AreEqual("add_abC", interceptor.Invocation.Method.Name);
		}

		[Test]
		public void Can_distinguish_differently_cased_methods_during_interception()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IDifferentlyCasedMethods>(interceptor);

			proxy.Abc();
			Assert.AreEqual("Abc", interceptor.Invocation.Method.Name);

			proxy.aBc();
			Assert.AreEqual("aBc", interceptor.Invocation.Method.Name);

			proxy.abC();
			Assert.AreEqual("abC", interceptor.Invocation.Method.Name);
		}

		[Test]
		public void Can_distinguish_differently_cased_properties_during_interception()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget<IDifferentlyCasedProperties>(interceptor);

			_ = proxy.Abc;
			Assert.AreEqual("get_Abc", interceptor.Invocation.Method.Name);

			_ = proxy.aBc;
			Assert.AreEqual("get_aBc", interceptor.Invocation.Method.Name);

			_ = proxy.abC;
			Assert.AreEqual("get_abC", interceptor.Invocation.Method.Name);
		}

		public interface IDifferentlyCasedEvents
		{
			event EventHandler Abc;
			event EventHandler aBc;
			event EventHandler abC;
		}

		public interface IDifferentlyCasedMethods
		{
			void Abc();
			void aBc();
			void abC();
		}

		public interface IDifferentlyCasedProperties
		{
			object Abc { get; }
			object aBc { get; }
			object abC { get; }
		}
	}
}
