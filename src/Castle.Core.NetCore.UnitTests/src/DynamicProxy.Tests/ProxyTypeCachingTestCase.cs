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
	using System.Collections.Generic;

	using Castle.DynamicProxy.Tests.Interfaces;

	using Xunit;

	public class ProxyTypeCachingTestCase : BasePEVerifyTestCase
	{
		private const string AllKinds = "All";

		private object Proxy(ProxyKind kind, params Type[] additionalInterfacesToProxy)
		{
			switch (kind)
			{
				case ProxyKind.Class:
					return generator.CreateClassProxy(typeof(SimpleClass), additionalInterfacesToProxy);
				case ProxyKind.WithoutTarget:
					return generator.CreateInterfaceProxyWithoutTarget(typeof(IEmpty), additionalInterfacesToProxy);
				case ProxyKind.WithTarget:
					return generator.CreateInterfaceProxyWithTarget(typeof(IEmpty), additionalInterfacesToProxy, new Empty());
				case ProxyKind.WithTargetInterface:
					return generator.CreateInterfaceProxyWithTarget(typeof(IEmpty), additionalInterfacesToProxy, new Empty());
				default:
					Assert.True(false, string.Format("Invalid proxy kind: {0}", kind));
					return null; //to satisfy the compiler
			}
		}

		public static IEnumerable<ProxyKind> All
		{
			get
			{
				return new[]
				{
					ProxyKind.Class,
					ProxyKind.WithoutTarget,
					ProxyKind.WithTarget,
					ProxyKind.WithTargetInterface
				};
			}
		}

#if SILVERLIGHT
		[Test]
		public void Duplicated_interfaces_not_significant_SILVERLIGHT()
		{
			foreach (var kind in All)
			{
				Duplicated_interfaces_not_significant(kind);
			}
		}
#else
		[Theory]
		[InlineData(ProxyKind.Class)]
		[InlineData(ProxyKind.WithoutTarget)]
		[InlineData(ProxyKind.WithTarget)]
		[InlineData(ProxyKind.WithTargetInterface)]
#endif
		public void Duplicated_interfaces_not_significant(ProxyKind kind)
		{
			var first = Proxy(kind, typeof(IOne), typeof(IOne));
			var second = Proxy(kind, typeof(IOne));
			Assert.Same(first.GetType(), second.GetType());
		}

#if SILVERLIGHT
		[Test]
		public void Explicit_inclusion_of_base_interfaces_not_significant_SILVERLIGHT()
		{
			foreach (var kind in All)
			{
				Explicit_inclusion_of_base_interfaces_not_significant(kind);
			}
		}
#else
		[Theory]
		[InlineData(ProxyKind.Class)]
		[InlineData(ProxyKind.WithoutTarget)]
		[InlineData(ProxyKind.WithTarget)]
		[InlineData(ProxyKind.WithTargetInterface)]
#endif
		public void Explicit_inclusion_of_base_interfaces_not_significant(ProxyKind kind)
		{
			var first = Proxy(kind, typeof(IBase), typeof(ISub1));
			var second = Proxy(kind, typeof(ISub1));
			Assert.Same(first.GetType(), second.GetType());
		}

#if SILVERLIGHT
		[Test]
		public void Order_of_additional_interfaces_not_significant_SILVERLIGHT()
		{
			foreach (var kind in All)
			{
				Order_of_additional_interfaces_not_significant(kind);
			}
		}
#else
		[Theory]
		[InlineData(ProxyKind.Class)]
		[InlineData(ProxyKind.WithoutTarget)]
		[InlineData(ProxyKind.WithTarget)]
		[InlineData(ProxyKind.WithTargetInterface)]
#endif
		public void Order_of_additional_interfaces_not_significant(ProxyKind kind)
		{
			var first = Proxy(kind, typeof(IOne), typeof(ITwo));
			var second = Proxy(kind, typeof(ITwo), typeof(IOne));
			Assert.Same(first.GetType(), second.GetType());
		}
	}
}