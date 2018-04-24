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
	using System.Reflection;
	using NUnit.Framework;

	public class GenericTestUtility
	{
		public static void CheckMethodInfoIsClosed(MethodInfo method, Type returnType, params Type[] parameterTypes)
		{
			Assert.IsFalse(method.ContainsGenericParameters);
			Assert.AreEqual(returnType, method.ReturnType);

			ParameterInfo[] parameters = method.GetParameters();
			Assert.AreEqual(parameterTypes.Length, parameters.Length);
			for (int i = 0; i < parameterTypes.Length; ++i)
			{
				Assert.AreEqual(parameterTypes[i], parameters[i].ParameterType);
			}
		}
	}
}