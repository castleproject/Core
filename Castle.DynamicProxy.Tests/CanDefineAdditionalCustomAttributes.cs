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

using System;
using NUnit.Framework;

namespace Castle.DynamicProxy.Tests
{
	[TestFixture]
	public class CanDefineAdditionalCustomAttributes
	{
		[Test]
		public void On_class()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateBuilder<__Protect>());

			object proxy = new ProxyGenerator().CreateClassProxy(typeof(CanDefineAdditionalCustomAttributes), options);

			Assert.IsTrue(proxy.GetType().IsDefined(typeof(__Protect), false));
		}

		[Test]
		public void On_interfaces()
		{
			ProxyGenerationOptions options = new ProxyGenerationOptions();
			options.AdditionalAttributes.Add(AttributeUtil.CreateBuilder<__Protect>());
		
			object proxy = new ProxyGenerator().CreateInterfaceProxyWithoutTarget(typeof(IDisposable), new Type[0], options);

			Assert.IsTrue(proxy.GetType().IsDefined(typeof(__Protect), false));
		}
	}

	public class __Protect : Attribute
	{
	}
}