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
	using System.Collections.Generic;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using NUnit.Framework;

	[TestFixture]
	public class MethodComparerTestCase
	{
		public static void GenericMethod<T>()
		{
		}

		public static void GenericMethod2<T>()
		{
		}

		public static void GenericMethod3<T, H>(T t, H h)
		{
		}

		private class FakeScope
		{
			public static void GenericMethod()
			{
			}

			public static void GenericMethod3<T>(T t)
			{
			}
		}

		private class NewScope
		{
			public static void GenericMethod3<T, H>(H t, T h)
			{
			}
		}

		[Test]
		public void CompareMethods()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(null, null));
			Assert.IsFalse(mc.Equals(null, typeof (object).GetMethod("ToString")));

			Assert.IsTrue(mc.Equals(typeof (object).GetMethod("ToString"), typeof (object).GetMethod("ToString")));
			Assert.IsTrue(mc.Equals(typeof (List<>).GetMethod("get_Count"), typeof (List<>).GetMethod("get_Count")));
			Assert.IsTrue(mc.Equals(typeof (List<int>).GetMethod("get_Count"), typeof (List<int>).GetMethod("get_Count")));
			Assert.IsTrue(mc.Equals(typeof (List<>).GetMethod("get_Count"), typeof (List<int>).GetMethod("get_Count")));
			Assert.IsTrue(mc.Equals(typeof (List<string>).GetMethod("get_Count"), typeof (List<>).GetMethod("get_Count")));
			Assert.IsTrue(mc.Equals(typeof (List<>).GetMethod("get_Item"), typeof (List<>).GetMethod("get_Item")));

			Assert.IsTrue(mc.Equals(typeof (List<string>).GetMethod("Add"), typeof (List<string>).GetMethod("Add")));
			Assert.IsFalse(mc.Equals(typeof (List<string>).GetMethod("Add"), typeof (List<>).GetMethod("Add")));
			Assert.IsFalse(mc.Equals(typeof (List<>).GetMethod("Add"), typeof (List<string>).GetMethod("Add")));

			Assert.IsTrue(mc.Equals(typeof (List<string>).GetMethod("get_Item"), typeof (List<string>).GetMethod("get_Item")));
			Assert.IsFalse(mc.Equals(typeof (List<string>).GetMethod("get_Item"), typeof (List<>).GetMethod("get_Item")));
			Assert.IsFalse(mc.Equals(typeof (List<>).GetMethod("get_Item"), typeof (List<string>).GetMethod("get_Item")));

			Assert.IsTrue(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod"),
			                        typeof (MethodComparerTestCase).GetMethod("GenericMethod")));
			Assert.IsTrue(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod").MakeGenericMethod(typeof (int)),
			                        typeof (MethodComparerTestCase).GetMethod("GenericMethod").MakeGenericMethod(typeof (int))));
			Assert.IsFalse(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod").MakeGenericMethod(typeof (int)),
			                         typeof (MethodComparerTestCase).GetMethod("GenericMethod").MakeGenericMethod(typeof (string))));
			Assert.IsFalse(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod").MakeGenericMethod(typeof (int)),
			                         typeof (MethodComparerTestCase).GetMethod("GenericMethod")));
			Assert.IsFalse(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod"),
			                         typeof (MethodComparerTestCase).GetMethod("GenericMethod2")));

			Assert.IsTrue(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod3"),
			                        typeof (MethodComparerTestCase).GetMethod("GenericMethod3")));
			Assert.IsFalse(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod3"),
			                         typeof (NewScope).GetMethod("GenericMethod3")));
			Assert.IsTrue(
				mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod3").MakeGenericMethod(typeof (int), typeof (int)),
				          typeof (NewScope).GetMethod("GenericMethod3").MakeGenericMethod(typeof (int), typeof (int))));

			Assert.IsFalse(
				mc.Equals(
					typeof (MethodComparerTestCase).GetMethod("GenericMethod3").MakeGenericMethod(typeof (int), typeof (string)),
					typeof (NewScope).GetMethod("GenericMethod3").MakeGenericMethod(typeof (int), typeof (string))));

			Assert.IsFalse(
				mc.Equals(
					typeof (MethodComparerTestCase).GetMethod("GenericMethod3").MakeGenericMethod(typeof (int), typeof (string)),
					typeof (NewScope).GetMethod("GenericMethod3").MakeGenericMethod(typeof (string), typeof (int))));

			Assert.IsFalse(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod3"),
			                         typeof (FakeScope).GetMethod("GenericMethod3")));

			Assert.IsFalse(mc.Equals(typeof (MethodComparerTestCase).GetMethod("GenericMethod"),
			                         typeof (FakeScope).GetMethod("GenericMethod")));

			Assert.IsFalse(mc.Equals(typeof (Console).GetMethod("WriteLine", new Type[] {typeof (object)}),
			                         typeof (Console).GetMethod("WriteLine", new Type[] {typeof (string), typeof (object[])})));
			Assert.IsTrue(mc.Equals(typeof (Console).GetMethod("WriteLine", new Type[] {typeof (string), typeof (object[])}),
			                        typeof (Console).GetMethod("WriteLine", new Type[] {typeof (string), typeof (object[])})));
		}
	}
}