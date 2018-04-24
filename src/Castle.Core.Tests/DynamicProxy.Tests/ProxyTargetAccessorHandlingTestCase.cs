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

	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class ProxyTargetAccessorHandlingTestCase : BasePEVerifyTestCase
	{
		private ProxyGenerationOptions MixIn(object mixin)
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mixin);
			return options;
		}

		[Test]
		public void ClassProxy_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateClassProxy(typeof(object), new[] { typeof(IProxyTargetAccessor) }));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void ClassProxy_base()
		{
			var ex = Assert.Throws(typeof(ArgumentException), () =>
				generator.CreateClassProxy<ImplementsProxyTargetAccessor>());
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void ClassProxy_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateClassProxy(typeof(object), MixIn(new ImplementsProxyTargetAccessor())));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		//-------------

		[Test]
		public void InterfaceProxyWithoutTarget_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), new[] { typeof(IProxyTargetAccessor) }));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithoutTarget_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IOne), new[] { typeof(IProxyTargetAccessor) },
					MixIn(new ImplementsProxyTargetAccessor())));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithoutTarget_TargetInterface()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IProxyTargetAccessor)));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithoutTarget_TargetInterface_derived()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithoutTarget(typeof(IProxyTargetAccessorDerived)));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTarget_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IOne), new[] { typeof(IProxyTargetAccessor) }, new One()));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTarget_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IOne), new[] { typeof(IProxyTargetAccessor) }, new One(),
					MixIn(new ImplementsProxyTargetAccessor())));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTarget_Target()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IProxyTargetAccessor), new ImplementsProxyTargetAccessor()));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTarget_Target_derived()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTarget(typeof(IProxyTargetAccessorDerived), new ImplementsProxyTargetAccessorDerived()));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		//----------------------

		[Test]
		public void InterfaceProxyWithTargetInterface_AdditionalInterfaces()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne), new[] { typeof(IProxyTargetAccessor) }, new One()));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTargetInterface_Mixin()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IOne), new[] { typeof(IProxyTargetAccessor) }, new One(),
					MixIn(new ImplementsProxyTargetAccessor())));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTargetInterface_Target()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IProxyTargetAccessor), new ImplementsProxyTargetAccessor()));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}

		[Test]
		public void InterfaceProxyWithTargetInterface_Target_derived()
		{
			var ex = Assert.Throws(typeof(ProxyGenerationException), () =>
				generator.CreateInterfaceProxyWithTargetInterface(typeof(IProxyTargetAccessorDerived),
					new ImplementsProxyTargetAccessorDerived()));
			StringAssert.Contains("IProxyTargetAccessor", ex.Message);
		}
	}

	public class ImplementsProxyTargetAccessor : IProxyTargetAccessor
	{
		#region IProxyTargetAccessor Members

		public object DynProxyGetTarget()
		{
			throw new NotImplementedException();
		}

		public void DynProxySetTarget(object obj)
		{
			throw new NotImplementedException();
		}

		public IInterceptor[] GetInterceptors()
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public interface IProxyTargetAccessorDerived : IProxyTargetAccessor
	{
	}

	public class ImplementsProxyTargetAccessorDerived : IProxyTargetAccessorDerived
	{
		#region IProxyTargetAccessorDerived Members

		public object DynProxyGetTarget()
		{
			throw new NotImplementedException();
		}

		public void DynProxySetTarget(object obj)
		{
			throw new NotImplementedException();
		}

		public IInterceptor[] GetInterceptors()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}