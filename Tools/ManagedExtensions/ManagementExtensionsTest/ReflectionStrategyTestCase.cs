// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Test
{
	using System;

	using NUnit.Framework;

	using Castle.ManagementExtensions;
	using Castle.ManagementExtensions.Default.Strategy;
	using Castle.ManagementExtensions.Test.Components;

	/// <summary>
	/// Summary description for ReflectionStrategyTestCase.
	/// </summary>
	[TestFixture]
	public class ReflectionStrategyTestCase : Assertion
	{
		[Test]
		public void TestInfo()
		{
			Object instance = new DummyHttpServer();

			ReflectionInvokerStrategy strategy = new ReflectionInvokerStrategy();
			MDynamicSupport dynamic = strategy.Create(instance);
			AssertNotNull(dynamic);
			AssertNotNull(dynamic.Info);
			AssertEquals( 3, dynamic.Info.Operations.Count );
			AssertEquals( 1, dynamic.Info.Attributes.Count );
		}

		[Test]
		public void TestOperation()
		{
			DummyHttpServer instance = new DummyHttpServer();

			ReflectionInvokerStrategy strategy = new ReflectionInvokerStrategy();
			MDynamicSupport dynamic = strategy.Create(instance);
			
			Assert( !instance.Started );

			dynamic.Invoke("Start", null, null);

			Assert( instance.Started );
		}
	}
}
