// Copyright 2004-2013 Castle Project - http://www.castleproject.org/
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
	using System.Linq;

	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;

	using CastleTests.DynamicProxy.Tests.Classes;
	using CastleTests.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class ClassProxyWithDefaultValuesTestCase : BasePEVerifyTestCase
	{
#if DOTNET45
		[Test]
#if __MonoCS__
		// Seems like mono is too strict, and doesn't handle a nullable default parameter in ParameterBuilder
		// https://github.com/mono/mono/blob/master/mcs/class/corlib/System.Reflection.Emit/ParameterBuilder.cs#L101
		[Ignore("System.ArgumentException : Constant does not match the defined type.")]
#endif
		public void MethodParameterWithDefaultValue_DefaultValueIsSetOnProxiedMethodAsWell()
		{
			var proxiedType = generator.CreateClassProxy<ClassWithMethodWithParameterWithDefaultValue>().GetType();

			var parameter = proxiedType.GetMethod("Method").GetParameters().Single(paramInfo => paramInfo.Name == "value");

			Assert.True(parameter.HasDefaultValue);
			Assert.AreEqual(3, parameter.DefaultValue);
		}

		[Test]
#if __MonoCS__
		[Ignore("Expected: False  But was: True")]
#endif
		public void MethodParameterWithDefaultValue_DefaultValueNullIsSetOnProxiedMethodAsWell()
		{
			var proxiedType = generator.CreateClassProxy<ClassWithMethodWithParameterWithNullDefaultValue>().GetType();

			var parameter = proxiedType.GetMethod("Method").GetParameters().Single(paramInfo => paramInfo.Name == "value");

			Assert.False(parameter.HasDefaultValue);
		}

		[Test]
		public void MethodParameterWithDefaultValue_UseNullDefaultValue_class_proxy()
		{
			var proxy = generator.CreateClassProxy<ClassWithMethodWithParameterWithNullDefaultValue>();
			var result = proxy.Method();

			Assert.IsTrue(result);
		}

		[Test]
		public void MethodParameterWithDefaultValue_UseNullDefaultValue_interface_proxy()
		{
			var proxy = generator.CreateInterfaceProxyWithoutTarget<InterfaceWithMethodWithParameterWithNullDefaultValue>(
					new SetReturnValueInterceptor(true));
			var result = proxy.Method();

			Assert.IsTrue(result);
		}
#endif
	}
}