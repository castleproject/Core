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
	using Interceptors;
	using NUnit.Framework;

	[TestFixture]
	public class MethodsWithAttributesOnParametersTestCase : BasePEVerifyTestCase
	{
		[Test]
		[ExpectedException(typeof (ArgumentException), ExpectedMessage = "No default value for argument")]
		public void ParametersAreCopiedToProxiedObject()
		{
			ClassWithAttributesOnMethodParameters requiredObj = (ClassWithAttributesOnMethodParameters)
			                                                    generator.CreateClassProxy(
			                                                    	typeof (ClassWithAttributesOnMethodParameters),
			                                                    	new RequiredParamInterceptor());

			requiredObj.MethodOne(-1);
		}

		[Test]
		public void CanGetParameterAttributeFromProxiedObject()
		{
			ClassWithAttributesOnMethodParameters requiredObj = (ClassWithAttributesOnMethodParameters)
			                                                    generator.CreateClassProxy(
			                                                    	typeof (ClassWithAttributesOnMethodParameters),
			                                                    	new RequiredParamInterceptor());

			requiredObj.MethodTwo(null);
		}
	}

    public class ClassWithAttributesOnMethodParameters
	{
		public virtual void MethodOne([Required(BadValue = -1)] int val)
		{
			Assert.IsFalse(val == -1);
		}

		public virtual void MethodTwo([Required("")] string name)
		{
			Assert.IsNotNull(name);
		}
	}

	public class RequiredAttribute : Attribute
	{
		private object defaultValue;
		private bool hasDefault = false;
		public object BadValue;

		public RequiredAttribute()
		{
		}

		public RequiredAttribute(object defaultValue)
		{
			hasDefault = true;
			this.defaultValue = defaultValue;
		}

		public object DefaultValue
		{
			get
			{
				if (!hasDefault)
				{
					throw new ArgumentException("No default value for argument");
				}
				return defaultValue;
			}
		}
	}
}