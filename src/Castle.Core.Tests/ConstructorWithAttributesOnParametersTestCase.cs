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
	using System.Linq;
	using System.Reflection;
	using Castle.Core.Tests.Classes;

	using NUnit.Framework;



	[TestFixture]
	public class ConstructorWithAttributesOnParametersTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void ParametersAreCopiedToProxiedObjectConstructor()
		{
			var requiredObj = (ClassWithAttributesOnConstructorParameters)generator.CreateClassProxy(
				typeof(ClassWithAttributesOnConstructorParameters), new object[] { 10,"" });
			var constructor = requiredObj.GetType().GetConstructors().First();

			var requiredAttributes = constructor
				.GetParameters()
				.SelectMany(x => x.GetCustomAttributes(typeof(RequiredAttribute), true))
				.OfType<RequiredAttribute>()
				.ToArray();

			Assert.AreEqual(2, requiredAttributes.Length, "Two Required attributes should be found on constructor parameters");
			Assert.AreEqual(1, requiredAttributes[0].DefaultValue, "The attribute was not properly copied");
			Assert.AreEqual("test", requiredAttributes[1].DefaultValue, "The attribute was not properly copied");
		}


	}
}