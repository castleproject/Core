// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;

	[TestFixture]
	public class GenericInterfaceWithGenericMethod
	{
		ProxyGenerator proxyGenerator;
		ProxyGenerationOptions options;

#if FEATURE_XUNITNET
		public GenericInterfaceWithGenericMethod()
#else
		[SetUp]
		public void Setup()
#endif
		{
			proxyGenerator = new ProxyGenerator();
			options = new ProxyGenerationOptions();
		}

		[Test]
		public void FailingCastleProxyCase()
		{
			var type = typeof(IMinimumFailure<string>);
			var result = proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new Type[0], options);

			Assert.IsNotNull(result as IMinimumFailure<string>);
		}

		public interface IMinimumFailure<T>
		{
			void NormalMethod();
			IEnumerable<T> FailingMethod<T2>(T2 pred);
		}
	}
}