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

	using NUnit.Framework;

	[TestFixture]
	public class ClassProxyWithDefaultValuesTestCase : BasePEVerifyTestCase
	{
#if DOTNET45
		[Test]
		public void MethodParameterWithDefaultValue_DefaultValueIsSetOnProxiedMethodAsWell()
		{
			var proxiedType = generator.CreateClassProxy<ClassWithMethodWithParameterWithDefaultValue>().GetType();

			var parameter = proxiedType.GetMethod("Method").GetParameters().Single(paramInfo => paramInfo.Name == "value");

			Assert.True(parameter.HasDefaultValue);
			Assert.AreEqual(3, parameter.DefaultValue);
		}
#endif
	}
}