// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests
{
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Interceptors;
	using NUnit.Framework;
	
	[TestFixture]
	public class OutRefParams
	{
		private ProxyGenerator generator;

		[SetUp]
		public void Init()
		{
			generator = new ProxyGenerator();
		}

		[Test]
		public void CanCreateProxyOfInterfaceWithOutParameter()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			object proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(WithOut),interceptor);
			Assert.IsNotNull(proxy);
		}

		[Test]
		public void CanCallMethodWithOutParameter()
		{
			int i;
			InvocatingInterceptor interceptor = new InvocatingInterceptor(delegate {});
			WithOut proxy = (WithOut)generator.CreateInterfaceProxyWithoutTarget(typeof(WithOut),interceptor);
			proxy.Do(out i);
		}

		[Test]
		public void CanAffectValueOfOutParameter()
		{
			int i;
			InvocatingInterceptor interceptor = new InvocatingInterceptor(delegate (IInvocation invocation)
			{
				invocation.Arguments[0] = 5;
			});
			WithOut proxy = (WithOut)generator.CreateInterfaceProxyWithoutTarget(typeof(WithOut),interceptor);
			proxy.Do(out i);
			Assert.AreEqual(5,i);
		}

		[Test]
		public void CanCreateProxyWithRefParam()
		{
			int i =3;
			InvocatingInterceptor interceptor = new InvocatingInterceptor(delegate (IInvocation invocation)
			{
				invocation.Arguments[0] = 5;
			});
			WithOut proxy = (WithOut)generator.CreateInterfaceProxyWithoutTarget(typeof(WithOut),interceptor);
			proxy.Did(ref i);
			Assert.AreEqual(5,i);
		}


		[Test]
		public void CanCreateComplexOutRefProxyOnClass()
		{
			int i =3;
			string s1 = "2";
			string s2;
			InvocatingInterceptor interceptor = new InvocatingInterceptor(delegate (IInvocation invocation)
			{
				invocation.Arguments[0] = 5;
				invocation.Arguments[1] = "aaa";
				invocation.Arguments[3] = "bbb";
			});
			MyClass proxy = (MyClass)generator.CreateClassProxy(typeof(MyClass),interceptor);
			proxy.MyMethod(out i, ref s1, 1, out s2);
			Assert.AreEqual(5,i);
			Assert.AreEqual(s1, "aaa" );
			Assert.AreEqual(s2, "bbb" );
		}

		public interface WithOut
		{
			void Do(out int i);
			void Did(ref int i);
		}


		 public class MyClass
        {
            public virtual void MyMethod(out int i, ref string s, int i1, out string s2)
            {
                throw new NotImplementedException(); 
            }
        }

		public class InvocatingInterceptor : IInterceptor
		{
			public delegate void Invoked(IInvocation invocation);

			private Invoked invoked;

			public InvocatingInterceptor(Invoked invoked)
			{
				this.invoked = invoked;
			}


			public void Intercept(IInvocation invocation)
			{
				invoked(invocation);
			}
		}
	}
}