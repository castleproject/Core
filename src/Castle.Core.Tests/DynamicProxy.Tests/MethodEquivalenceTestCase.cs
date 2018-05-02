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

	using NUnit.Framework;

	[TestFixture]
	public class MethodEquivalenceTestCase
	{
		[Test]
		public void CanProxyTypesWithMethodsOnlyDifferentByGenericArguments()
		{
			ProxyGenerator generator = new ProxyGenerator();

			IMyService target1 = (IMyService)generator.CreateInterfaceProxyWithTarget(
				typeof(IMyService), new MyServiceImpl(), new StandardInterceptor());
			Assert.IsNotNull(target1.CreateSomething<int>("aa"));

			IMyService target2 = (IMyService)generator.CreateClassProxy(
                typeof(MyServiceImpl), new StandardInterceptor());
			Assert.IsNotNull(target2.CreateSomething<int>("aa"));
		}
	}

	public interface IMyService
	{
		ISomething CreateSomething<T>(string somethingSpec);
		ISomething CreateSomething(string somethingKey);
	}

	public interface ISomething
	{
		void Do(Type type, string parameter);
	}

	public class NoOpSomething : ISomething
	{
		public void Do(Type type, string parameter) {}
	}

	public class MyServiceImpl : IMyService
	{
		public ISomething CreateSomething<T>(string somethingSpec)
		{
			return new NoOpSomething();
		}

		public ISomething CreateSomething(string somethingKey)
		{
			return new NoOpSomething();
		}
	}
}