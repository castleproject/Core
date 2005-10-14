// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Test
{
	using System;
	using System.Collections.Specialized;
	using NUnit.Framework;

	[TestFixture]
	public class ProxyingAbstractClass
	{
		[Test]
		public void ProxyAnAbstractClass()
		{
			ProxyGenerator generator = new ProxyGenerator();
			LoginInterceptor interceptor = new LoginInterceptor();
			AbsCls ac = (AbsCls)generator.CreateClassProxy(typeof(AbsCls),interceptor, false);
			ac.Method();

			Assert.AreEqual("Method", interceptor.Methods[0]);
		}

		private class LoginInterceptor : IInterceptor
		{
			public StringCollection Methods = new StringCollection();

			public object Intercept(IInvocation invocation, params object[] args)
			{
				Methods.Add(invocation.Method.Name);
				return null;
			}
		}

		public abstract class AbsCls
		{
			public abstract void Method();
		}
	}
}
