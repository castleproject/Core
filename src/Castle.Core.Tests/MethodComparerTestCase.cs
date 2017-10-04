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
	using System.Linq;
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

		public static T GenericMethod4<T>(T t)
		{
			return t;
		}

		public static IEnumerable<T> GenericMethod5<T>(T t)
		{
			return Enumerable.Empty<T>();
		}

		public static IEnumerable<IEnumerable<T>> GenericMethod6<T>(T t)
		{
			return Enumerable.Empty<IEnumerable<T>>();
		}

		public static IEnumerable<IEnumerable<int>> GenericMethod7()
		{
			return Enumerable.Empty<IEnumerable<int>>();
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

		private class Base
		{
			public virtual void GenericMethod()
			{
			}

			public virtual void GenericMethod2<T>()
			{
			}

			public virtual void GenericMethod3<T, H>(T t, H h)
			{
			}

			public virtual T GenericMethod4<T>(T t)
			{
				return t;
			}

			public virtual IEnumerable<T> GenericMethod5<T>(T t)
			{
				return Enumerable.Empty<T>();
			}

			public virtual IEnumerable<IEnumerable<T>> GenericMethod6<T>(T t)
			{
				return Enumerable.Empty<IEnumerable<T>>();
			}

			public virtual IEnumerable<IEnumerable<int>> GenericMethod7()
			{
				return Enumerable.Empty<IEnumerable<int>>();
			}
		}

		private class Inherited : Base
		{
			public override void GenericMethod()
			{
				base.GenericMethod();
			}

			public override void GenericMethod2<T>()
			{
				base.GenericMethod2<T>();
			}

			public override void GenericMethod3<T, H>(T t, H h)
			{
				base.GenericMethod3(t, h);
			}

			public override T GenericMethod4<T>(T t)
			{
				return base.GenericMethod4(t);
			}

			public override IEnumerable<T> GenericMethod5<T>(T t)
			{
				return base.GenericMethod5(t);
			}

			public override IEnumerable<IEnumerable<T>> GenericMethod6<T>(T t)
			{
				return base.GenericMethod6(t);
			}

			public override IEnumerable<IEnumerable<int>> GenericMethod7()
			{
				return base.GenericMethod7();
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

		[Test]
		public void Compare_generic_parameter_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(MethodComparerTestCase).GetMethod("GenericMethod4"),
				typeof(MethodComparerTestCase).GetMethod("GenericMethod4")));
		}

		[Test]
		public void Compare_generic_parameter_nested_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(MethodComparerTestCase).GetMethod("GenericMethod5"),
				typeof(MethodComparerTestCase).GetMethod("GenericMethod5")));
		}

		[Test]
		public void Compare_generic_parameter_double_nested_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(MethodComparerTestCase).GetMethod("GenericMethod6"),
				typeof(MethodComparerTestCase).GetMethod("GenericMethod6")));
		}

		[Test]
		public void Compare_double_nested_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(MethodComparerTestCase).GetMethod("GenericMethod7"),
				typeof(MethodComparerTestCase).GetMethod("GenericMethod7")));
		}

		[Test]
		public void Compare_virtual_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod"),
				typeof(Base).GetMethod("GenericMethod")));
		}

		[Test]
		public void Compare_virtual_generic_parameter_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod2"),
				typeof(Base).GetMethod("GenericMethod2")));
		}

		[Test]
		public void Compare_virtual_mutiple_generic_parameter_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod3"),
				typeof(Base).GetMethod("GenericMethod3")));
		}

		[Test]
		public void Compare_virtual_generic_parameter_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod4"),
				typeof(Base).GetMethod("GenericMethod4")));
		}

		[Test]
		public void Compare_virtual_generic_parameter_nested_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod5"),
				typeof(Base).GetMethod("GenericMethod5")));
		}

		[Test]
		public void Compare_virtual_generic_parameter_double_nested_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod6"),
				typeof(Base).GetMethod("GenericMethod6")));
		}

		[Test]
		public void Compare_virtual_double_nested_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod7"),
				typeof(Base).GetMethod("GenericMethod7")));
		}

		[Test]
		public void Compare_inherited_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod"),
				typeof(Inherited).GetMethod("GenericMethod")));
		}

		[Test]
		public void Compare_inherited_generic_parameter_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod2"),
				typeof(Inherited).GetMethod("GenericMethod2")));
		}

		[Test]
		public void Compare_inherited_mutiple_generic_parameter_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod3"),
				typeof(Inherited).GetMethod("GenericMethod3")));
		}

		[Test]
		public void Compare_inherited_generic_parameter_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod4"),
				typeof(Inherited).GetMethod("GenericMethod4")));
		}

		[Test]
		public void Compare_inherited_generic_parameter_nested_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod5"),
				typeof(Inherited).GetMethod("GenericMethod5")));
		}

		[Test]
		public void Compare_inherited_generic_parameter_double_nested_generic_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod6"),
				typeof(Inherited).GetMethod("GenericMethod6")));
		}

		[Test]
		public void Compare_inherited_double_nested_return_method()
		{
			MethodSignatureComparer mc = MethodSignatureComparer.Instance;
			Assert.IsTrue(mc.Equals(typeof(Base).GetMethod("GenericMethod7"),
				typeof(Inherited).GetMethod("GenericMethod7")));
		}

		[Test]
		public void Compare_two_method_overloads_with_generic_arg_types()
		{
			var mc = MethodSignatureComparer.Instance;
			var methods = typeof(IHaveOverloadedGenericMethod).GetMethods();
			var firstOverload = methods[0];
			var secondOverload = methods[1];

			// The overloads must obviously have different signatures, or we could never even have
			// compiled this test code successfully. The arguments must have a different type:
			Assert.IsFalse(mc.Equals(firstOverload, secondOverload));
		}

		private interface IHaveOverloadedGenericMethod
		{
			void GenericMethod<T>(GenericClass1<T> arg);
			void GenericMethod<T>(GenericClass2<T> arg);
		}

		private class GenericClass1<T> { }

		private class GenericClass2<T> { }
	}
}