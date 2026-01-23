// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Internal;

	using NUnit.Framework;

	[TestFixture]
	public class ArgumentsUtilTestCase
	{
		public interface Methods
		{
			public void ByValue(int arg);
			public void In(in int arg);
			public void Out(out int arg);
			public void Ref(ref int arg);
			public void RefReadonly(ref readonly int arg);
		}

		[TestCase(nameof(Methods.ByValue), false)]
		[TestCase(nameof(Methods.In), true)]
		[TestCase(nameof(Methods.Out), true)]
		[TestCase(nameof(Methods.Ref), true)]
		[TestCase(nameof(Methods.RefReadonly), true)]
		public void IsByRef(string methodName, bool expected)
		{
			var parameter = GetParameterOf(methodName);
			Assert.AreEqual(expected, parameter.IsByRef);
		}

		[TestCase(nameof(Methods.In), true)]
		[TestCase(nameof(Methods.Out), false)]
		[TestCase(nameof(Methods.Ref), false)]
		[TestCase(nameof(Methods.RefReadonly), true)]
		public void IsReadOnly(string methodName, bool expected)
		{
			var parameter = GetParameterOf(methodName);
			Assume.That(parameter.ParameterType.IsByRef);
			Assert.AreEqual(expected, parameter.IsReadOnly);
		}

		private ParameterInfo GetParameterOf(string methodName)
		{
			return typeof(Methods).GetMethod(methodName)!.GetParameters().Single();
		}
	}
}
