// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using Castle.DynamicProxy.Generators;
	using NUnit.Framework;

	[TestFixture]
	public class TypeEquivalentTestCase
	{
		[Test]
		public void SimpleCases()
		{
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(string), typeof(string)));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(int), typeof(int)));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(long), typeof(long)));
			
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(string), typeof(int)));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(int), typeof(string)));
		}

		[Test]
		public void GenericTypeParameter()
		{
			Type[] genericArgs = typeof(Nested<,>).GetGenericArguments();

			Type T = genericArgs[0];
			Type Z = genericArgs[1];

			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(T, T));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(T, Z));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(Z, T));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(typeof(int), genericArgs[0]));
		}

		[Test]
		public void GenericTypesWithGenericParameter()
		{
			Type[] genericArgs = typeof(Nested<,>).GetGenericArguments();
			
			Type T = genericArgs[0];
			Type Z = genericArgs[1];
			
			Type listOfT = typeof(List<>).MakeGenericType(T);
			Type listOfZ = typeof(List<>).MakeGenericType(Z);

			Type listOfString = typeof(List<>).MakeGenericType(typeof(String));
			Type listOfInt = typeof(List<>).MakeGenericType(typeof(int));

			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfString, listOfString));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfInt, listOfInt));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfT, listOfT));
			Assert.IsTrue(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfZ, listOfZ));

			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfString, listOfInt));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfT, listOfZ));
			Assert.IsFalse(InterfaceProxyWithTargetGenerator.IsTypeEquivalent(listOfZ, listOfT));
		}
		
		public class Nested<T, Z>
		{
			
		}
	}
}
