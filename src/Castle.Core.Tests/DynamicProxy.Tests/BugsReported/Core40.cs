// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.BugsReported
{
	using System;

	using NUnit.Framework;

	public class Core40ClassToProxy
	{
		public Core40ClassToProxy(Object arg1, Object arg2)
		{
		}
	}

	[TestFixture]
	public class Core40 : BasePEVerifyTestCase
	{
		[Test]
		public void ShouldGenerateTypeWithIndexers()
		{
			Assert.Throws<InvalidProxyConstructorArgumentsException>(delegate {
				generator.CreateClassProxy(typeof(Core40ClassToProxy), new object[] { null, null, null });
			});
		}
	}
}