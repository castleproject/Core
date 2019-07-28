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
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;

	using NUnit.Framework;

	[TestFixture]
	public class MethodFinderTestCase
	{
		[Test]
		public void GetMethodsForPublic()
		{
			MethodInfo[] methods =
				MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Instance | BindingFlags.Public);
			MethodInfo[] realMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.Public);
			CollectionAssert.AreEquivalent(realMethods, methods);
		}

		[Test]
		public void GetMethodsForNonPublic()
		{
			MethodInfo[] methods =
				MethodFinder.GetAllInstanceMethods(typeof(object), BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo[] realMethods = typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic);
			CollectionAssert.AreEquivalent(realMethods, methods);
		}

		[Test]
		public void GetMethodsForPublicAndNonPublic()
		{
			MethodInfo[] methods =
				MethodFinder.GetAllInstanceMethods(typeof(object),
												   BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			MethodInfo[] realMethods =
				typeof(object).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			CollectionAssert.AreEquivalent(realMethods, methods);
		}

		[Test]
		public void GetMethodsThrowsOnStatic()
		{
			Assert.Throws<ArgumentException>(() =>
				MethodFinder.GetAllInstanceMethods(typeof(object),
					BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
			);
		}

		[Test]
		public void GetMethodsThrowsOnOtherFlags()
		{
			Assert.Throws<ArgumentException>(() =>
				MethodFinder.GetAllInstanceMethods(typeof(object),
					BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)
			);
		}
	}
}
