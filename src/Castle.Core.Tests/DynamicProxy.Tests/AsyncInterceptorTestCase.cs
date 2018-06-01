﻿// Copyright 2004-2016 Castle Project - http://www.castleproject.org/
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

namespace CastleTests.DynamicProxy.Tests
{
	using System;
	using System.Linq;
	using System.Reflection;
	using System.Threading.Tasks;

	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Tests;

	using CastleTests.DynamicProxy.Tests.Classes;
	using CastleTests.DynamicProxy.Tests.Interfaces;
	using CastleTests.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class AsyncInterceptorTestCase : BasePEVerifyTestCase
	{
	    [Test]
	    public async Task Should_Intercept_Asynchronous_Methods_With_An_Async_Operations_Prior_To_Calling_Proceed()
	    {
			// Arrange
		    IInterfaceWithAsynchronousMethod target = new ClassWithAsynchronousMethod();
			IInterceptor interceptor = new AsyncInterceptor();

		    IInterfaceWithAsynchronousMethod proxy =
			    generator.CreateInterfaceProxyWithTargetInterface(target, interceptor);

			// Act
		    await proxy.Method().ConfigureAwait(false);
	    }
	}
}
