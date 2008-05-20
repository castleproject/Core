// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.JSGeneration
{
	using System;
	using Castle.MonoRail.Framework.JSGeneration;
	using Castle.MonoRail.Framework.JSGeneration.DynamicDispatching;
	using NUnit.Framework;

	[TestFixture]
	public class DynamicDispatcherTestCase
	{
		[Test]
		public void CollectsOperationsThatHaveAttribute()
		{
			DynamicDispatcher dispatcher = new DynamicDispatcher(new MainTarget());

			Assert.IsFalse(dispatcher.HasMethod("HiddenOp"));
			Assert.IsTrue(dispatcher.HasMethod("PublicOp"));
		}

		[Test]
		public void CanInvokeDynamicOperations()
		{
			DynamicDispatcher dispatcher = new DynamicDispatcher(new MainTarget());

			Assert.AreEqual("good code!", dispatcher.Dispatch("PublicOp", null));
		}

		[Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Method HiddenOp not found for dynamic dispatching")]
		public void CannotInvokeMethodsNotMarkedAsOperation()
		{
			DynamicDispatcher dispatcher = new DynamicDispatcher(new MainTarget());

			dispatcher.Dispatch("HiddenOp", null);
		}

		[Test]
		public void CanResolveMethodArguments()
		{
			DynamicDispatcher dispatcher = new DynamicDispatcher(new MainTarget());

			Assert.AreEqual(4, dispatcher.Dispatch("Sum", new object[] { 1, 3 }));
		}

		[Test]
		public void CanResolveMethodArgumentsWithParamArray()
		{
			DynamicDispatcher dispatcher = new DynamicDispatcher(new MainTarget());

			Assert.AreEqual(20, dispatcher.Dispatch("SumMany", new object[] { 1, 3, 7, 9 }));
		}

		[Test]
		public void ExposesExtensionMethods()
		{
			DynamicDispatcher dispatcher = new DynamicDispatcher(new MainTarget(), new MyExtension());

			Assert.IsTrue(dispatcher.HasMethod("PublicOp"));
			Assert.IsTrue(dispatcher.HasMethod("ExtMethod"));
			Assert.AreEqual("Hello", dispatcher.Dispatch("ExtMethod", null));
		}

		class MainTarget
		{
			public string HiddenOp()
			{
				return "bad code!";
			}

			[DynamicOperation]
			public string PublicOp()
			{
				return "good code!";
			}

			[DynamicOperation]
			public int Sum(int x, int y)
			{
				return x + y;
			}

			[DynamicOperation]
			public int SumMany(params int[] args)
			{
				int sum = 0;

				foreach(int i in args)
				{
					sum += i;
				}

				return sum;
			}
		}

		class MyExtension
		{
			[DynamicOperation]
			public string ExtMethod()
			{
				return "Hello";
			}

		}
	}
}
