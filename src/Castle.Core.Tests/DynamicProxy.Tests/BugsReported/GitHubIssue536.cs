// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.BugsReported
{
	using System;
	using System.Reflection;
	using System.Threading.Tasks;

	using NUnit.Framework;

	[TestFixture]
	public class GitHubIssue536 : BasePEVerifyTestCase
	{
		[Test]
		public void DynamicProxy_NonIntercepted_Property_Leaked()
		{
			var instance = new TestClassForCache();
			var toProxy = instance.GetType();

			var proxyGenerationOptions = new ProxyGenerationOptions(new TestCacheProxyGenerationHook());

			var generator = new ProxyGenerator();
			var proxy = generator.CreateClassProxyWithTarget(toProxy,
				instance,
				proxyGenerationOptions);

			var accessor = (ITestCacheInterface)proxy;
			accessor.InstanceProperty = 1;

			Assert.AreEqual(accessor.InstanceProperty, instance.InstanceProperty);
		}

		public class TestCacheProxyGenerationHook : AllMethodsHook
		{
			public override bool ShouldInterceptMethod(Type type, MethodInfo methodInfo)
			{
				return false;
			}
		}

		public interface ITestCacheInterface
		{
			int InstanceProperty { get; set; }
		}

		public class TestClassForCache : ITestCacheInterface
		{
			public virtual int InstanceProperty { get; set; }
		}
	}
}
