// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using NUnit.Framework;
	using Castle.Core.Interceptor;

	[TestFixture]
	public class ConstraintViolationInDebuggerTestCase : BasePEVerifyTestCase
	{
		// This test case yields a TypeLoadException in the debugger, but works perfectly without a debugger attached.
		// It also produces verifiable code.
		// In Visual Studio 2010 this test passes just fine with the debugger attached.
		[Test]
		public void TestCase()
		{
			generator.ProxyBuilder.CreateInterfaceProxyTypeWithTarget(
				typeof (IPresentationHost), Type.EmptyTypes, typeof (PresentationHost), ProxyGenerationOptions.Default);

			IServiceAgent agent =
				(IServiceAgent)
				generator.CreateInterfaceProxyWithTarget<IServiceAgent>(new ServiceAgent(), new StandardInterceptor());
			agent.GetProxy<string>();
		}

		public class PresentationHost : IPresentationHost
		{
			#region IPresentationHost Members

			public void Register<T>() where T : IPresentation
			{
			}

			public void Register<T>(Type service) where T : IPresentation
			{
			}

			#endregion
		}

		public class ServiceAgent : IServiceAgent
		{
			#region IServiceAgent Members

			public T GetProxy<T>() where T : class
			{
				return null;
			}

			public T GetProxy<T>(object callbackInstance) where T : class
			{
				return null;
			}

			#endregion
		}


		public interface IPresentationHost
		{
			void Register<T>() where T : IPresentation;
			void Register<T>(Type service) where T : IPresentation;
		}

		public interface IServiceAgent
		{
			T GetProxy<T>() where T : class;
			T GetProxy<T>(object callbackInstance) where T : class;
		}


		public interface IPresentation
		{
		}
	}
}