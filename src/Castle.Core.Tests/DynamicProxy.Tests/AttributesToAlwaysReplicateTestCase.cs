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
	using System.Linq;
	using System.Reflection;

	using NUnit.Framework;

	[TestFixture]
	public class AttributesToAlwaysReplicateTestCase : BasePEVerifyTestCase
	{
		// The following test specifies what the C# compiler is expected to do
		// (namely to always replicate the attribute in derived classes, even when
		// it is omitted).
		// If ever this test starts failing, then DynamicProxy's behavior
		// regarding ParamArrayAttribute might need to be reconsidered.
		[Test]
		public void Csharp_compiler_always_replicates_ParamArrayAttribute()
		{
			var manuallyDerivedType = typeof(ManuallyDerived);
			var method = manuallyDerivedType.GetMethod("Method");
			var param = method.GetParameters().Single(p => p.Name == "args");
			var paramHasOwnAttr = param.IsDefined(typeof(ParamArrayAttribute), false);
			Assert.True(paramHasOwnAttr);
		}

		[Test]
		public void DynamicProxy_always_replicates_ParamArrayAttribute()
		{
			var proxyType = generator.CreateClassProxy<Base>().GetType();
			var method = proxyType.GetMethod("Method");
			var param = method.GetParameters().Single(p => p.Name == "args");
			var paramHasOwnAttr = param.IsDefined(typeof(ParamArrayAttribute), false);
			Assert.True(paramHasOwnAttr);
		}

		[Test]
		public void Replicated_ParamArrayAttribute_is_still_only_seen_a_single_time()
		{
			var proxyType = generator.CreateClassProxy<Base>().GetType();
			var method = proxyType.GetMethod("Method");
			var param = method.GetParameters().Single(p => p.Name == "args");
			var paramAttrs = param.GetCustomAttributes(typeof(ParamArrayAttribute), true);
			var paramAttrCount = paramAttrs.Count();
			Assert.AreEqual(1, paramAttrCount);
		}

		public class Base
		{
			public virtual void Method(params object[] args)
			{
			}
		}

		public class ManuallyDerived : Base
		{
			public override void Method(object[] args) // note the omitted `params`
			{
			}
		}
	}
}
