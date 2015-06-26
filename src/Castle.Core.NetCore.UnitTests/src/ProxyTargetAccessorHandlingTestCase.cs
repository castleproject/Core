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

	using Castle.DynamicProxy.Tests.Interfaces;
	using Castle.InterClasses;

	using Xunit;

	public class ProxyTargetAccessorHandlingTestCase : BasePEVerifyTestCase
	{
		private ProxyGenerationOptions MixIn(object mixin)
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(mixin);
			return options;
		}

		[Fact]
		public void ClassProxy_AdditionalInterfaces()
		{
			Action action = () =>
								  generator.CreateClassProxy(typeof(object),
															 new[]
																 {
																	 typeof (IProxyTargetAccessor)
																 });
			Exception exception = Assert.Throws<ProxyGenerationException>(action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
			//Assert.Contains("IProxyTargetAccessor",exception.Message);
		}

		[Fact]
		public void ClassProxy_base()
		{
			Action action = () =>
								  generator.CreateClassProxy
									  <ImplementsProxyTargetAccessor>();
			Exception exception = Assert.Throws(typeof(ArgumentException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void ClassProxy_Mixin()
		{
			Action action = () =>
								  generator.CreateClassProxy(typeof(object),
															 MixIn(
																 new ImplementsProxyTargetAccessor()));
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		//-------------
		[Fact]
		public void InterfaceProxyWithoutTarget_AdditionalInterfaces()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithoutTarget(
									  typeof(IOne),
									  new[] { typeof(IProxyTargetAccessor) });
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_Mixin()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithoutTarget(
									  typeof(IOne),
									  new[] { typeof(IProxyTargetAccessor) },
									  MixIn(new ImplementsProxyTargetAccessor()));
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_TargetInterface()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithoutTarget(
									  typeof(IProxyTargetAccessor));
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithoutTarget_TargetInterface_derived()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithoutTarget(
									  typeof(IProxyTargetAccessorDerived));
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_AdditionalInterfaces()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithTarget(
									  typeof(IOne),
									  new[] { typeof(IProxyTargetAccessor) },
									  new One());
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_Mixin()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithTarget(
									  typeof(IOne),
									  new[] { typeof(IProxyTargetAccessor) },
									  new One(),
									  MixIn(new ImplementsProxyTargetAccessor()));
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_Target()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithTarget(
									  typeof(IProxyTargetAccessor),
									  new ImplementsProxyTargetAccessor());
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTarget_Target_derived()
		{
			Action action = () =>
								  generator.CreateInterfaceProxyWithTarget(
									  typeof(IProxyTargetAccessorDerived),
									  new ImplementsProxyTargetAccessorDerived());
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		//----------------------


		[Fact]
		public void InterfaceProxyWithTargetInterface_AdditionalInterfaces()
		{
			Action action = () =>
								  generator.
									  CreateInterfaceProxyWithTargetInterface(
									  typeof(IOne),
									  new[] { typeof(IProxyTargetAccessor) },
									  new One());
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_Mixin()
		{
			Action action = () =>
								  generator.
									  CreateInterfaceProxyWithTargetInterface(
									  typeof(IOne),
									  new[] { typeof(IProxyTargetAccessor) },
									  new One(),
									  MixIn(new ImplementsProxyTargetAccessor()));
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_Target()
		{
			Action action = () =>
								  generator.
									  CreateInterfaceProxyWithTargetInterface(
									  typeof(IProxyTargetAccessor),
									  new ImplementsProxyTargetAccessor());
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}

		[Fact]
		public void InterfaceProxyWithTargetInterface_Target_derived()
		{
			Action action = () =>
								  generator.
									  CreateInterfaceProxyWithTargetInterface(
									  typeof(IProxyTargetAccessorDerived),
									  new ImplementsProxyTargetAccessorDerived());
			Exception exception = Assert.Throws(typeof(ProxyGenerationException), action);
			Assert.Contains("IProxyTargetAccessor", exception.Message);
		}
	}


	public class ImplementsProxyTargetAccessor : IProxyTargetAccessor
	{
		#region IProxyTargetAccessor Members

		public object DynProxyGetTarget()
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

		public IInterceptor[] GetInterceptors()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}