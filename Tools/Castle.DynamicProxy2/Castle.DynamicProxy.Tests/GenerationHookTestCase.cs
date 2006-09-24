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
	using System.Collections;
	using System.Reflection;
	
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;
	
	using NUnit.Framework;

	[TestFixture]
	public class GenerationHookTestCase
	{
		private ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

		[Test]
		public void HookIsUsedForConcreteClassProxy()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			LogHook hook = new LogHook(typeof(ServiceClass));
			
			ProxyGenerationOptions options = new ProxyGenerationOptions(hook);

			ServiceClass proxy = (ServiceClass) generator.CreateClassProxy(
				typeof(ServiceClass), options, logger);

			Assert.IsTrue(hook.Completed);
			Assert.AreEqual(10, hook.AskedMembers.Count);
			Assert.AreEqual(2, hook.NonVirtualMembers.Count);
			
			proxy.Sum(1, 2);
			Assert.IsFalse(proxy.Valid);

			Assert.AreEqual("get_Valid ", logger.LogContents);
		}

		[Test, Ignore("Not implemented yet")]
		public void HookIsUsedForInterfaceProxy()
		{
			
		}

		class LogHook : IProxyGenerationHook
		{
			private readonly Type targetTypeToAssert;
			private IList nonVirtualMembers = new ArrayList();
			private IList askedMembers = new ArrayList();
			private bool completed;

			public LogHook(Type targetTypeToAssert)
			{
				this.targetTypeToAssert = targetTypeToAssert;
			}

			public IList NonVirtualMembers
			{
				get { return nonVirtualMembers; }
			}

			public IList AskedMembers
			{
				get { return askedMembers; }
			}

			public bool Completed
			{
				get { return completed; }
			}

			public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
			{
				Assert.AreEqual(targetTypeToAssert, type);

				askedMembers.Add(memberInfo);

				if (memberInfo.Name.StartsWith("Sum"))
				{
					return false;
				}

				return true;
			}

			public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
			{
				Assert.AreEqual(targetTypeToAssert, type);

				nonVirtualMembers.Add(memberInfo);
			}

			public void MethodsInspected()
			{
				completed = true;
			}
		}
	}
}
