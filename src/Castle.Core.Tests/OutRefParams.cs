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

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;
	using NUnit.Framework;

	[TestFixture]
	public class OutRefParams : BasePEVerifyTestCase
	{
		[Test]
		public void CanCreateProxyOfInterfaceWithOutParameter()
		{
			KeepDataInterceptor interceptor = new KeepDataInterceptor();
			object proxy = generator.CreateInterfaceProxyWithoutTarget(typeof (IWithRefOut), interceptor);
			Assert.IsNotNull(proxy);
		}

		[Test]
		public void CanCallMethodWithOutParameter()
		{
			int i;
			WithCallbackInterceptor interceptor = new WithCallbackInterceptor(delegate { });
			IWithRefOut proxy = (IWithRefOut) generator.CreateInterfaceProxyWithoutTarget(typeof (IWithRefOut), interceptor);
			proxy.Do(out i);
		}

		[Test]
		public void CanAffectValueOfOutParameter()
		{
			int i;
			WithCallbackInterceptor interceptor =
				new WithCallbackInterceptor(delegate(IInvocation invocation) { invocation.Arguments[0] = 5; });
			IWithRefOut proxy = (IWithRefOut) generator.CreateInterfaceProxyWithoutTarget(typeof (IWithRefOut), interceptor);
			proxy.Do(out i);
			Assert.AreEqual(5, i);
		}

		[Test]
		public void CanCreateProxyWithRefParam()
		{
			int i = 3;
			WithCallbackInterceptor interceptor =
				new WithCallbackInterceptor(delegate(IInvocation invocation) { invocation.Arguments[0] = 5; });
			IWithRefOut proxy = (IWithRefOut) generator.CreateInterfaceProxyWithoutTarget(typeof (IWithRefOut), interceptor);
			proxy.Did(ref i);
			Assert.AreEqual(5, i);
		}


		[Test]
		public void CanCreateComplexOutRefProxyOnClass()
		{
			int i = 3;
			string s1 = "2";
			string s2;
			WithCallbackInterceptor interceptor = new WithCallbackInterceptor(delegate(IInvocation invocation)
			                                                              	{
			                                                              		invocation.Arguments[0] = 5;
			                                                              		invocation.Arguments[1] = "aaa";
			                                                              		invocation.Arguments[3] = "bbb";
			                                                              	});
			MyClass proxy = (MyClass) generator.CreateClassProxy(typeof (MyClass), interceptor);
			proxy.MyMethod(out i, ref s1, 1, out s2);
			Assert.AreEqual(5, i);
			Assert.AreEqual(s1, "aaa");
			Assert.AreEqual(s2, "bbb");
		}

		[Test, Explicit]
		public void CanCreateProxyWithStructRefParam()
		{
			MyStruct s = new MyStruct(10);
			MyClass proxy = (MyClass) generator.CreateClassProxy(typeof (MyClass), new StandardInterceptor());
			proxy.MyMethodWithStruct(ref s);
			Assert.AreEqual(20, s.Value);
		}

		public struct MyStruct
		{
			public int Value;

			public MyStruct(int value)
			{
				Value = value;
			}
		}


		public class MyClass
		{
			public virtual void MyMethod(out int i, ref string s, int i1, out string s2)
			{
				throw new NotImplementedException();
			}

			public virtual void MyMethodWithStruct(ref MyStruct s)
			{
				s.Value = 2*s.Value;
			}
		}
	}
}