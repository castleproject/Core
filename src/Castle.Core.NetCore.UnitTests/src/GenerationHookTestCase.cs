// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace CastleTests
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.InterClasses;
	using Castle.DynamicProxy.Tests.Interceptors;

	using CastleTests.DynamicProxy.Tests.Classes;

	using Xunit;

	public class GenerationHookTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void HookIsUsedForConcreteClassProxy()
		{
			var logger = new LogInvocationInterceptor();
			var hook = new LogHook(typeof(ServiceClass), true);

			var options = new ProxyGenerationOptions(hook);

			var proxy = (ServiceClass)generator.CreateClassProxy(typeof(ServiceClass), options, logger);

			Assert.True(hook.Completed);
			Assert.Equal(13, hook.AskedMembers.Count);//, "Asked members");
			Assert.Equal(2, hook.NonVirtualMembers.Count);//, "Non-virtual members");

			proxy.Sum(1, 2);
			Assert.False(proxy.Valid);

			Assert.Equal("get_Valid ", logger.LogContents);
		}

		[Fact]
		public void HookIsUsedForInterfaceProxy()
		{
			var logger = new LogInvocationInterceptor();
			var hook = new LogHook(typeof(IService), false);

			var options = new ProxyGenerationOptions(hook);

			var proxy = (IService)
						generator.CreateInterfaceProxyWithTarget(
							typeof(IService), new ServiceImpl(), options, logger);

			Assert.True(hook.Completed);
			Assert.Equal(10, hook.AskedMembers.Count);
			Assert.Equal(0, hook.NonVirtualMembers.Count);

			Assert.Equal(3, proxy.Sum(1, 2));
			Assert.False(proxy.Valid);

			Assert.Equal("Sum get_Valid ", logger.LogContents);
		}

		[Fact]
		public void Hook_can_NOT_see_GetType_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var getType = typeof(EmptyClass).GetMethod("GetType");
			Assert.False(hook.AskedMembers.Contains(getType));
			Assert.False(hook.NonVirtualMembers.Contains(getType));
		}

		[Fact]
		public void Hook_can_NOT_see_MemberwiseClone_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var memberwiseClone = typeof(EmptyClass).GetMethod("MemberwiseClone", BindingFlags.NonPublic | BindingFlags.Instance);
			Assert.False(hook.AskedMembers.Contains(memberwiseClone));
			Assert.False(hook.NonVirtualMembers.Contains(memberwiseClone));
		}

		[Fact]
		public void Hook_can_see_Equals_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var equals = typeof(EmptyClass).GetMethod("Equals");
			Assert.True(hook.AskedMembers.Contains(equals));
		}

		[Fact]
		public void Hook_can_see_GetHashCode_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var getHashCode = typeof(EmptyClass).GetMethod("GetHashCode");
			Assert.True(hook.AskedMembers.Contains(getHashCode));
		}

		[Fact]
		public void Hook_can_see_ToString_method()
		{
			var hook = new LogHook(typeof(EmptyClass));

			generator.CreateClassProxy(typeof(EmptyClass), new ProxyGenerationOptions(hook));

			var equals = typeof(EmptyClass).GetMethod("ToString");
			Assert.True(hook.AskedMembers.Contains(equals));
		}
	}

#if !SILVERLIGHT && !NETCORE
	[Serializable]
#endif
	public class LogHook : IProxyGenerationHook
	{
		private readonly Type targetTypeToAssert;
		private readonly bool screeningEnabled;
		private readonly IList<MemberInfo> nonVirtualMembers = new List<MemberInfo>();
		private readonly IList<MemberInfo> askedMembers = new List<MemberInfo>();
		private bool completed;

		public LogHook(Type targetTypeToAssert, bool screeningEnabled = false)
		{
			this.targetTypeToAssert = targetTypeToAssert;
			this.screeningEnabled = screeningEnabled;
		}

		public IList<MemberInfo> NonVirtualMembers
		{
			get { return nonVirtualMembers; }
		}

		public IList<MemberInfo> AskedMembers
		{
			get { return askedMembers; }
		}

		public bool Completed
		{
			get { return completed; }
		}

		public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
		{
			Assert.Equal(targetTypeToAssert, type);

			askedMembers.Add(memberInfo);

			if (screeningEnabled && memberInfo.Name.StartsWith("Sum"))
			{
				return false;
			}

			return true;
		}

		public void NonProxyableMemberNotification(Type type, MemberInfo memberInfo)
		{
			Assert.Equal(targetTypeToAssert, type);

			nonVirtualMembers.Add(memberInfo);
		}

		public void MethodsInspected()
		{
			completed = true;
		}
	}
}
