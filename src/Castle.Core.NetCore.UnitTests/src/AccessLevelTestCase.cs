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
	using Castle.DynamicProxy.Tests.Classes;
	using Castle.DynamicProxy.Tests.Interceptors;

	using System.Collections.Generic;

	using Xunit;

	using System.Reflection;

	public class AccessLevelTestCase : BasePEVerifyTestCase
	{
		[Fact]
		public void ProtectedConstructor()
		{
			NonPublicConstructorClass proxy =
				generator.CreateClassProxy(
					typeof(NonPublicConstructorClass), new StandardInterceptor())
					as NonPublicConstructorClass;

			Assert.NotNull(proxy);

			proxy.DoSomething();
		}

		[Fact]
		public void ProtectedInternalConstructor()
		{
			ProtectedInternalConstructorClass proxy =
				generator.CreateClassProxy(
					typeof(ProtectedInternalConstructorClass), new StandardInterceptor())
					as ProtectedInternalConstructorClass;

			Assert.NotNull(proxy);

			proxy.DoSomething();
		}

		[Fact]
		public void ProtectedMethods()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			NonPublicMethodsClass proxy = (NonPublicMethodsClass)
				generator.CreateClassProxy(typeof(NonPublicMethodsClass), logger);

			proxy.DoSomething();
			Assert.Equal(2, logger.Invocations.Count);
			Assert.Equal("DoSomething", logger.Invocations[0]);
			Assert.Equal("DoOtherThing", logger.Invocations[1]);
		}

		[Fact]
		public void InternalConstructorIsNotReplicated()
		{
			object proxy = generator.CreateClassProxy(typeof(Dictionary<int, object>), new StandardInterceptor());
			Assert.Null(proxy.GetType().GetConstructor(new[] { typeof(IInterceptor[]), typeof(bool) }));
		}

		internal class InternalClass
		{
			internal InternalClass()
			{
			}
		}

#if !MONO && !SILVERLIGHT && !NETCORE
		[Fact]
		public void InternalConstructorIsReplicatedWhenInternalsVisibleTo()
		{
			object proxy = generator.CreateClassProxy(typeof(InternalClass), new StandardInterceptor());
			Assert.NotNull(proxy.GetType().GetConstructor(new[] { typeof(IInterceptor[]) }));
		}
#endif
	}
}