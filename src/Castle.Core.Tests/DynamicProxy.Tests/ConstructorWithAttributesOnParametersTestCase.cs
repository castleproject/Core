// Copyright 2004-2018 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Classes;

	using NUnit.Framework;

	[TestFixture]
	public class ConstructorWithAttributesOnParametersTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void CustomAttributesOnParametersAreCopiedToProxiedObjectConstructor()
		{
			var requiredObj = generator.CreateClassProxy(typeof(ClassWithAttributesOnConstructorParameters), new object[] { 10,"" });
			var constructor = requiredObj.GetType().GetConstructors().First();

			// There should be exactly two parameters with a `RequiredAttribute`:
			var paramWithRequiredAttributeCount =
				constructor.GetParameters().Where(p => p.IsDefined(typeof(RequiredAttribute), true)).Count();
			Assert.AreEqual(2, paramWithRequiredAttributeCount);

			// There should be exactly one parameter named `intParam`:
			var intParam = constructor.GetParameters().SingleOrDefault(p => p.Name == "intParam");
			Assert.NotNull(intParam);

			// That parameter should have exactly one `RequiredAttribute` carrying the value 1.
			var intParamRequiredAttrs = intParam.GetCustomAttributes<RequiredAttribute>(inherit: true);
			Assert.AreEqual(1, intParamRequiredAttrs.Count());
			Assert.AreEqual(1, intParamRequiredAttrs.Single().DefaultValue);

			// There should be exactly one parameter named `stringParam`:
			var stringParam = constructor.GetParameters().SingleOrDefault(p => p.Name == "stringParam");
			Assert.NotNull(stringParam);

			// That parameter should have exactly one `RequiredAttribute` carrying the value "test".
			var stringParamRequiredAttrs = stringParam.GetCustomAttributes<RequiredAttribute>(inherit: true);
			Assert.AreEqual(1, stringParamRequiredAttrs.Count());
			Assert.AreEqual("test", stringParamRequiredAttrs.Single().DefaultValue);
		}
	}
}
