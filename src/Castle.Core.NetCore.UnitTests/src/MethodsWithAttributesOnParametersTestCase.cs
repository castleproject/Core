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

	using Castle.Core.Tests.Classes;

	using Interceptors;

	using Xunit;

	public class MethodsWithAttributesOnParametersTestCase : BasePEVerifyTestCase
	{
		[Fact]
		//[ExpectedException(typeof (ArgumentException), ExpectedMessage = "No default value for argument")]
		public void ParametersAreCopiedToProxiedObject()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				var requiredObj = (ClassWithAttributesOnMethodParameters)
					generator.CreateClassProxy(
						typeof(ClassWithAttributesOnMethodParameters),
						new RequiredParamInterceptor());

				requiredObj.MethodOne(-1);
			});
		}

		[Fact]
		public void CanGetParameterAttributeFromProxiedObject()
		{
			var requiredObj = (ClassWithAttributesOnMethodParameters)
				generator.CreateClassProxy(
					typeof(ClassWithAttributesOnMethodParameters),
					new RequiredParamInterceptor());

			requiredObj.MethodTwo(null);
		}
	}
}