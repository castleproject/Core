// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;

	[TestFixture]
	public class ClassProxyWithTargetTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Forwards_to_protected_method()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassWithProtectedMethod());
			Assert.AreEqual(42, proxy.InvokeFortyTwo());
		}

		public class ClassWithProtectedMethod
		{
			public int InvokeFortyTwo()
			{
				return FortyTwo();
			}

			protected virtual int FortyTwo()
			{
				return 42;
			}
		}
	}
}
