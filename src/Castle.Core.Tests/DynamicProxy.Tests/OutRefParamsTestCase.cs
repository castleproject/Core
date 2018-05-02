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

namespace Castle.DynamicProxy.Tests
{
	using System;

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
	public class OutRefParamsTestCase : BasePEVerifyTestCase
	{
		public interface IClassHasMethodThrowException
		{
			int MethodWithRefParam(ref int refParam, out string outParam);
		}

#if FEATURE_SERIALIZATION
		[Serializable]
#endif
		public class ClassHasMethodThrowException : IClassHasMethodThrowException
		{
			public virtual int MethodWithRefParam(ref int refParam, out string outParam)
			{
				refParam = 23;
				outParam = "23";

				if (refParam == 23)
				{
					throw new Exception("intentional exception");
				}

				return 42;
			}
		}

		public class ExceptionCatchInterceptor : StandardInterceptor
		{
			public Exception Exception { get; set; }

			protected override void PerformProceed(IInvocation invocation)
			{
				try
				{
					base.PerformProceed(invocation);
				}
				catch (Exception e)
				{
					Exception = e;
				}
				finally
				{
					invocation.ReturnValue = 42;
				}
			}
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

		[Test]
		public void CanAffectValueOfOutParameter()
		{
			int i;
			var interceptor =
				new WithCallbackInterceptor(delegate(IInvocation invocation) { invocation.Arguments[0] = 5; });
			var proxy = (IWithRefOut)generator.CreateInterfaceProxyWithoutTarget(typeof(IWithRefOut), interceptor);
			proxy.Do(out i);
			Assert.AreEqual(5, i);
		}

		[Test]
		public void CanCallMethodWithOutParameter()
		{
			int i;
			var interceptor = new WithCallbackInterceptor(delegate { });
			var proxy = (IWithRefOut)generator.CreateInterfaceProxyWithoutTarget(typeof(IWithRefOut), interceptor);
			proxy.Do(out i);
		}

		[Test]
		public void CanCreateComplexOutRefProxyOnClass()
		{
			var i = 3;
			var s1 = "2";
			string s2;
			var interceptor = new WithCallbackInterceptor(delegate(IInvocation invocation)
			{
				invocation.Arguments[0] = 5;
				invocation.Arguments[1] = "aaa";
				invocation.Arguments[3] = "bbb";
			});
			var proxy = (MyClass)generator.CreateClassProxy(typeof(MyClass), interceptor);
			proxy.MyMethod(out i, ref s1, 1, out s2);
			Assert.AreEqual(5, i);
			Assert.AreEqual(s1, "aaa");
			Assert.AreEqual(s2, "bbb");
		}

		[Test]
		public void CanCreateProxyOfInterfaceWithOutParameter()
		{
			var interceptor = new KeepDataInterceptor();
			var proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(IWithRefOut), interceptor);
			Assert.IsNotNull(proxy);
		}

		[Test]
		public void CanCreateProxyWithRefParam()
		{
			var i = 3;
			var interceptor =
				new WithCallbackInterceptor(delegate(IInvocation invocation) { invocation.Arguments[0] = 5; });
			var proxy = (IWithRefOut)generator.CreateInterfaceProxyWithoutTarget(typeof(IWithRefOut), interceptor);
			proxy.Did(ref i);
			Assert.AreEqual(5, i);
		}

		[Test]
		public void CanCreateProxyWithStructRefParam()
		{
			var s = new MyStruct(10);
			var proxy = (MyClass)generator.CreateClassProxy(typeof(MyClass), new StandardInterceptor());
			proxy.MyMethodWithStruct(ref s);
			Assert.AreEqual(20, s.Value);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_class_proxy_caught_by_interceptor()
		{
			var proxy = generator.CreateClassProxy<ClassHasMethodThrowException>(new ExceptionCatchInterceptor());

			var param1 = 1;
			var param2 = "1";

			var retVal = proxy.MethodWithRefParam(ref param1, out param2);

			Assert.AreEqual(42, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_class_proxy_mixin_uncaught()
		{
			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new ClassHasMethodThrowException());
			var proxy = generator.CreateClassProxy<object>(options, new StandardInterceptor()) as IClassHasMethodThrowException;

			var param1 = 1;
			var param2 = "1";
			var exMsg = "";
			var retVal = 1;

			try
			{
				retVal = proxy.MethodWithRefParam(ref param1, out param2);
			}
			catch (Exception ex)
			{
				exMsg = ex.Message;
			}

			Assert.AreEqual("intentional exception", exMsg);
			Assert.AreEqual(1, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_class_proxy_uncaught()
		{
			var proxy = generator.CreateClassProxy<ClassHasMethodThrowException>(new StandardInterceptor());

			var param1 = 1;
			var param2 = "1";
			var exMsg = "";
			var retVal = 1;

			try
			{
				retVal = proxy.MethodWithRefParam(ref param1, out param2);
			}
			catch (Exception ex)
			{
				exMsg = ex.Message;
			}

			Assert.AreEqual("intentional exception", exMsg);
			Assert.AreEqual(1, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_class_proxy_with_target_caught_by_interceptor()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassHasMethodThrowException(), new ExceptionCatchInterceptor());

			var param1 = 1;
			var param2 = "1";

			var retVal = proxy.MethodWithRefParam(ref param1, out param2);

			Assert.AreEqual(42, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_class_proxy_with_target_uncaught()
		{
			var proxy = generator.CreateClassProxyWithTarget(new ClassHasMethodThrowException(), new StandardInterceptor());

			var param1 = 1;
			var param2 = "1";
			var exMsg = "";
			var retVal = 1;

			try
			{
				retVal = proxy.MethodWithRefParam(ref param1, out param2);
			}
			catch (Exception ex)
			{
				exMsg = ex.Message;
			}

			Assert.AreEqual("intentional exception", exMsg);
			Assert.AreEqual(1, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_interface_proxy_with_target_caught_by_interceptor()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<IClassHasMethodThrowException>(new ClassHasMethodThrowException(),
				                                                                        new ExceptionCatchInterceptor());

			var param1 = 1;
			var param2 = "1";

			var retVal = proxy.MethodWithRefParam(ref param1, out param2);

			Assert.AreEqual(42, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_interface_proxy_with_target_interface_caught_by_interceptor()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTargetInterface<IClassHasMethodThrowException>(
					new ClassHasMethodThrowException(), new ExceptionCatchInterceptor());

			var param1 = 1;
			var param2 = "1";

			var retVal = proxy.MethodWithRefParam(ref param1, out param2);

			Assert.AreEqual(42, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_interface_proxy_with_target_interface_uncaught()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTargetInterface<IClassHasMethodThrowException>(
					new ClassHasMethodThrowException(), new StandardInterceptor());

			var param1 = 1;
			var param2 = "1";
			var exMsg = "";
			var retVal = 1;

			try
			{
				retVal = proxy.MethodWithRefParam(ref param1, out param2);
			}
			catch (Exception ex)
			{
				exMsg = ex.Message;
			}

			Assert.AreEqual("intentional exception", exMsg);
			Assert.AreEqual(1, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}

		[Test]
		public void Exception_during_method_out_ref_arguments_set_interface_proxy_with_target_uncaught()
		{
			var proxy =
				generator.CreateInterfaceProxyWithTarget<IClassHasMethodThrowException>(new ClassHasMethodThrowException(),
				                                                                        new StandardInterceptor());

			var param1 = 1;
			var param2 = "1";
			var exMsg = "";
			var retVal = 1;

			try
			{
				retVal = proxy.MethodWithRefParam(ref param1, out param2);
			}
			catch (Exception ex)
			{
				exMsg = ex.Message;
			}

			Assert.AreEqual("intentional exception", exMsg);
			Assert.AreEqual(1, retVal);
			Assert.AreEqual(23, param1);
			Assert.AreEqual("23", param2);
		}
	}
}