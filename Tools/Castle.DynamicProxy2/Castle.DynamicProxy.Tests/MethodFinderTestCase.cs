namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections;
	using System.Reflection;
	using Castle.DynamicProxy.Generators;
	using NUnit.Framework;

	[TestFixture]
	public class MethodFinderTestCase
	{
		private static void AssertArraysAreEqualUnsorted(object[] expected, object[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length);
			ArrayList actualAsList = new ArrayList(actual);
			foreach(object expectedElement in expected)
			{
				Assert.Contains(expectedElement, actualAsList);
				actualAsList.Remove(expectedElement);
					// need to remove the element after it has been found to guarantee that duplicate elements are handled correctly
			}
		}

		[Test]
		public void AssertArrayAreEqualUnsorted()
		{
			AssertArraysAreEqualUnsorted(new object[0], new object[0]);
			AssertArraysAreEqualUnsorted(new object[] {null}, new object[] {null});
			AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {null, null, "one"});
			AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {"one", null, null});

			try
			{
				AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {"one", "one", null});
				Assert.Fail();
			}
			catch(AssertionException)
			{
				// ok
			}
			try
			{
				AssertArraysAreEqualUnsorted(new object[] {null, "one"}, new object[] {"one", null, null});
				Assert.Fail();
			}
			catch(AssertionException)
			{
				// ok
			}
			try
			{
				AssertArraysAreEqualUnsorted(new object[] {null, "one", null}, new object[] {"one", null});
				Assert.Fail();
			}
			catch(AssertionException)
			{
				// ok
			}
		}

		[Test]
		public void GetMethodsForPublic()
		{
			MethodInfo[] methods =
				MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Instance | BindingFlags.Public);
			MethodInfo[] realMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public);
			AssertArraysAreEqualUnsorted(realMethods, methods);
		}

		[Test]
		public void GetMethodsForNonPublic()
		{
			MethodInfo[] methods =
				MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo[] realMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			AssertArraysAreEqualUnsorted(realMethods, methods);
		}

		[Test]
		public void GetMethodsForPublicAndNonPublic()
		{
			MethodInfo[] methods =
				MethodFinder.GetAllInstanceMethods(typeof(object),
				                                   BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			MethodInfo[] realMethods =
				typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			AssertArraysAreEqualUnsorted(realMethods, methods);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetMethodsThrowsOnStatic()
		{
			MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void GetMethodsThrowsOnOtherFlags()
		{
			MethodFinder.GetAllInstanceMethods(typeof(object),
			                                   BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public |
			                                   BindingFlags.DeclaredOnly);
		}
	}
}