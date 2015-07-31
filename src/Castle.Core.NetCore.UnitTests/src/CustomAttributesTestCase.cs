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
	using System.Reflection;

	using Xunit;

	public class CustomAttributesTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void Should_Proxy_type_having_complicated_arguments()
		{
			// http://support.castleproject.org/projects/DYNPROXY/issues/view/DYNPROXY-ISSUE-108
			var proxy = generator.CreateClassProxy(typeof(Classes.ClassWith_Smart_Attribute));
			var properties = proxy.GetType().GetProperties();
			Assert.NotEmpty(properties);
			properties[0].GetCustomAttributes(false);
		}
	}
}