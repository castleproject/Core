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
	using System.Reflection;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.GenInterfaces;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interceptors;
	using GenClasses;
	using NUnit.Framework;

	[TestFixture]
	public class GenericMethodsProxyTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ProxyAdditionalInterfaceWithGenericMethods()
		{
			IService proxy = (IService)generator.CreateInterfaceProxyWithoutTarget(
				typeof(IService), new Type[] { typeof(OnlyGenMethodsInterface) }, 
				new StandardInterceptor());

			Assert.IsNotNull(proxy);
		}

		[Test]
		public void ProxyInterfaceWithGenericMethodWithTwoGenericParametersWhereOneIsBaseToAnother()
		{
			generator.CreateInterfaceProxyWithoutTarget<GenericMethodWhereOneGenParamInheritsTheOther>();
		}

		[Test]
		public void GenericMethod_WithConstraintOnOtherParameter()
		{
			Type type = typeof(IInterfaceWithGenericMethodWithDependentConstraint);

			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			IInterfaceWithGenericMethodWithDependentConstraint proxy = (IInterfaceWithGenericMethodWithDependentConstraint) 
					generator.CreateInterfaceProxyWithoutTarget(type, new Type[] {}, interceptor);

			proxy.RegisterType<object, string>();

			MethodInfo expectedMethod = 
					typeof(IInterfaceWithGenericMethodWithDependentConstraint).GetMethod("RegisterType").MakeGenericMethod(typeof(object), typeof(string));
			
			Assert.AreEqual (expectedMethod, interceptor.Invocation.Method);
		}

		[Test]
		public void GenericMethod_WithConstraintOnSurroundingTypeParameter ()
		{
			Type type = typeof (IGenericInterfaceWithGenericMethodWithDependentConstraint<object>);

			KeepDataInterceptor interceptor = new KeepDataInterceptor ();
			IGenericInterfaceWithGenericMethodWithDependentConstraint<object> proxy = (IGenericInterfaceWithGenericMethodWithDependentConstraint<object>)
					generator.CreateInterfaceProxyWithoutTarget(type, new Type[] { }, interceptor);

			proxy.RegisterType<string> ();

			MethodInfo expectedMethod =
					typeof (IGenericInterfaceWithGenericMethodWithDependentConstraint<object>).GetMethod ("RegisterType").MakeGenericMethod (typeof (string));

			Assert.AreEqual (expectedMethod, interceptor.Invocation.Method);
		}

		[Test]
		public void GenericMethod_WithGenericOfGenericArgument()
		{
			var proxy = generator.CreateClassProxy<ClassWithMethodWithGenericOfGenericOfT>();
			proxy.GenericMethodWithGenericOfGenericArgument<string>(null);
		}

		[Test]
		public void GenericMethod_WithArrayOfGenericOfGenericArgument()
		{
			var proxy = generator.CreateClassProxy<ClassWithMethodWithArrayOfListOfT>();
			proxy.GenericMethodWithListArrayArgument<string>(null);
		}
	}
}
