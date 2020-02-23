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

namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;

	using Castle.DynamicProxy;

	using NUnit.Framework;

	[TestFixture]
	public class CustomAttributeInfoTestCase
	{
		class MyAttribute1 : Attribute
		{
			public MyAttribute1()
			{
			}

			public MyAttribute1(int intArgument, string stringArgument, int[] arrayArgument)
			{
			}

			public int IntProperty { get; set; }
			public string StringProperty { get; set; }
			public int[] ArrayProperty { get; set; }

#pragma warning disable 649
			public int intField;
			public string stringField;
			public int[] arrayField;
#pragma warning restore 649
		}

		class MyAttribute2 : Attribute
		{
			public MyAttribute2(int intArgument, string stringArgument, int[] arrayArgument)
			{
			}
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Same_Constructor_Arguments_Are_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] { 1, 2, 3 }));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] { 1, 2, 3 }));

			Assert.AreEqual(x, y);
			Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
		}

		[Test]
		public void Attributes_Of_Different_Type_With_Same_Constructor_Arguments_Are_Not_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] { 1, 2, 3 }));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute2(42, "foo", new[] { 1, 2, 3 }));

			Assert.AreNotEqual(x, y);
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Different_Constructor_Arguments_Are_Not_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] { 1, 2, 3 }));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute1(99, "foo", new[] { 1, 2, 3 }));

			Assert.AreNotEqual(x, y);
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Different_Constructor_Array_Arguments_Are_Not_Equal()
		{
			var x = CustomAttributeInfo.FromExpression(() => new MyAttribute1(42, "foo", new[] { 1, 2, 3 }));
			var y = CustomAttributeInfo.FromExpression(() => new MyAttribute1(99, "foo", new[] { 1, 2, 4 }));

			Assert.AreNotEqual(x, y);
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Same_Properties_Are_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 3 } });
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 3 } });

			Assert.AreEqual(x, y);
			Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Different_Properties_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 3 } });
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { IntProperty = 99, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 3 } });

			Assert.AreNotEqual(x, y);
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Different_Array_Properties_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 3 } });
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { IntProperty = 99, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 4 } });

			Assert.AreNotEqual(x, y);
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Same_Fields_Are_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { intField = 42, stringField = "foo", arrayField = new[] { 1, 2, 3 } });
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { intField = 42, stringField = "foo", arrayField = new[] { 1, 2, 3 } });

			Assert.AreEqual(x, y);
			Assert.AreEqual(x.GetHashCode(), y.GetHashCode());
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Different_Fields_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { intField = 42, stringField = "foo", arrayField = new[] { 1, 2, 3 } });
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { intField = 99, stringField = "foo", arrayField = new[] { 1, 2, 3 } });

			Assert.AreNotEqual(x, y);
		}

		[Test]
		public void Attributes_Of_Same_Type_With_Different_Array_Fields_Are_Not_Equal()
		{
			var x =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { intField = 42, stringField = "foo", arrayField = new[] { 1, 2, 3 } });
			var y =
				CustomAttributeInfo.FromExpression(
					() => new MyAttribute1 { intField = 99, stringField = "foo", arrayField = new[] { 1, 2, 4 } });

			Assert.AreNotEqual(x, y);
		}

		[Test]
		[TestCaseSource("FromExpressionTestCases")]
		public void FromExpression_Creates_Same_CustomAttributeInfo_As_Calling_The_Constructor(
			Opaque<Expression<Func<Attribute>>> expr, CustomAttributeInfo expected)
		{
			var actual = CustomAttributeInfo.FromExpression(expr.Value);
			Assert.AreEqual(expected, actual);
		}

		public static IEnumerable<object[]> FromExpressionTestCases()
		{
			var defaultCtor = typeof(MyAttribute1).GetConstructor(Type.EmptyTypes);
			var ctorWithArgs = typeof(MyAttribute1).GetConstructor(new[] { typeof(int), typeof(string), typeof(int[]) });
			var intProperty = typeof(MyAttribute1).GetProperty("IntProperty");
			var stringProperty = typeof(MyAttribute1).GetProperty("StringProperty");
			var arrayProperty = typeof(MyAttribute1).GetProperty("ArrayProperty");
			var intField = typeof(MyAttribute1).GetField("intField");
			var stringField = typeof(MyAttribute1).GetField("stringField");
			var arrayField = typeof(MyAttribute1).GetField("arrayField");

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1(),
				new CustomAttributeInfo(defaultCtor, new object[0]));

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1(42, "foo", new[] { 1, 2, 3 }),
				new CustomAttributeInfo(ctorWithArgs, new object[] { 42, "foo", new[] { 1, 2, 3 } }));

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1 { IntProperty = 42, StringProperty = "foo", ArrayProperty = new[] { 1, 2, 3 } },
				new CustomAttributeInfo(
					defaultCtor,
					new object[0],
					new [] {intProperty, stringProperty, arrayProperty},
					new object[] { 42, "foo", new[] { 1, 2, 3 } }));

			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1 { intField = 42, stringField = "foo", arrayField = new[] { 1, 2, 3 } },
				new CustomAttributeInfo(
					defaultCtor,
					new object[0],
					new[] { intField, stringField, arrayField },
					new object[] { 42, "foo", new[] { 1, 2, 3 } }));

			// Use local variables instead of constants in the expression
			int arg1 = 42;
			string arg2 = "foo";
			int[] arg3 = { 1, 2, 3 };
			yield return CreateFromExpressionTestCase(
				() => new MyAttribute1(arg1, arg2, arg3),
				new CustomAttributeInfo(ctorWithArgs, new object[] { arg1, arg2, arg3 }));

		}

		private static object[] CreateFromExpressionTestCase(Expression<Func<Attribute>> expr, CustomAttributeInfo expected)
		{
			return new object[] { new Opaque<Expression<Func<Attribute>>>(expr), expected };
		}

		// NOTE: The following type is needed as a workaround for an issue with either NUnit 3's test adapter,
		// or Visual Studio's test executor: One of them appears to be unable to parse formatted LINQ expression
		// trees. Therefore, we need to wrap those in a dummy type to prevent formatting. This will change how
		// the individual test cases are displayed/identified in Test Explorer.
		//
		// See https://developercommunity.visualstudio.com/content/problem/663145/test-explorer-in-vs-1620-can-no-longer-run-certain.html.

		public struct Opaque<T>
		{
			public readonly T Value;

			public Opaque(T value)
			{
				Value = value;
			}

			public override string ToString()
			{
				return Value.GetType().ToString();
			}
		}
	}
}
