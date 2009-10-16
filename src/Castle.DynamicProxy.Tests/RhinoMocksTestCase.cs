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

#if !MONO && !SILVERLIGHT
namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Runtime.InteropServices;
	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Tests.Interceptors;
	using NUnit.Framework;
	using RhinoMocksCPPInterfaces;

	[TestFixture]
	public class RhinoMocksTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void GenericClassWithGenericMethod()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			IDoubleGeneric<int> proxy =
				(IDoubleGeneric<int>) generator.CreateInterfaceProxyWithTarget(typeof (IDoubleGeneric<int>),
				                                                               new DoubleGenericImpl<int>(), logger);
			proxy.Call<string>(1, "");
			Assert.AreEqual("Call", logger.Invocations[0]);
		}

		[Test]
		public void GenericClassWithGenericMethodWitoutTarget()
		{
		    var interceptor = new SetReturnValueInterceptor(3);
		    IDoubleGeneric<int> proxy =
				(IDoubleGeneric<int>) generator.CreateInterfaceProxyWithoutTarget(typeof (IDoubleGeneric<int>),
				                                                                  interceptor);
			object o = proxy.Call(1, "");
			Assert.AreEqual(3, o);
		}

		[Test, Ignore(".Net 3.5 SP 1 broke this one")]
		public void CanProxyMethodWithModOpt()
		{
		    IHaveMethodWithModOpts proxy =
				(IHaveMethodWithModOpts) generator.CreateInterfaceProxyWithoutTarget(typeof (IHaveMethodWithModOpts), new DoNothingInterceptor());
			proxy.StartLiveOnSlot(4);
		}

		[Test]
		public void GenericMethodReturningGenericArray()
		{
			generator.CreateInterfaceProxyWithoutTarget(
				typeof (IStore1),
                new DoNothingInterceptor());
		}

		[Test]
		public void UsingEvents_Interface()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();

			IWithEvents proxy = (IWithEvents) generator.CreateInterfaceProxyWithTarget(typeof (IWithEvents),
			                                                                           new FakeWithEvents(),
			                                                                           logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test]
		public void UsingEvents_Class()
		{
			LogInvocationInterceptor logger = new LogInvocationInterceptor();
			FakeWithEvents proxy = (FakeWithEvents) generator.CreateClassProxy(
			                                        	typeof (FakeWithEvents),
			                                        	ProxyGenerationOptions.Default,
			                                        	logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test]
		public void NeedingToCreateNewMethodTableSlot()
		{
			generator.CreateClassProxy(typeof (MultiClass), new Type[] {typeof (ISpecialMulti)});
		}

		[Test]
		public void ProxyingInterfaceWithGuid()
		{
			object o = generator.CreateInterfaceProxyWithoutTarget(typeof (IWithGuid), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingInternalInterface()
		{
			object o = generator.CreateInterfaceProxyWithoutTarget(typeof (IFoo), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void CanProxyDataSet()
		{
			generator.CreateClassProxy(typeof (DataSet), new Type[0], new StandardInterceptor());
		}

		[Test]
		public void ProxyingComInteraces()
		{
			object o = generator
				.CreateInterfaceProxyWithoutTarget(typeof (IComServiceProvider), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingGenericClassWithGenericClassConstraint()
		{
			object o = generator
				.CreateInterfaceProxyWithoutTarget(typeof (IFactory2), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void CanGetCorrectValuesFromIntPtr()
		{
			IFooWithIntPtr o = (IFooWithIntPtr) generator
			                                    	.CreateInterfaceProxyWithoutTarget(typeof (IFooWithIntPtr),
			                                    	                                   new SetReturnValueInterceptor(IntPtr.Zero));
			IntPtr buffer = o.Buffer(15);
			Assert.AreEqual(IntPtr.Zero, buffer);
		}

		[Test]
		public void ProxyInternalMethod()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			WithInternalMethod o = (WithInternalMethod) generator.CreateClassProxy(typeof (WithInternalMethod),
			                                                                       new Type[0], logging);
			o.Foo();
			Assert.AreEqual("Foo ", logging.LogContents);
		}

		[Test]
		public void ProxyingProtectedInternalAbstractMethod()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			SomeClassWithProtectedInternalAbstractClass o =
				(SomeClassWithProtectedInternalAbstractClass)
				generator.CreateClassProxy(typeof (SomeClassWithProtectedInternalAbstractClass),
				                           new Type[0], logging);
			Assert.IsNotNull(o);
		}

		[Test]
		public void VirtualMethodCallsFromTheConstructor()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			MakeVirtualCallFromCtor o = (MakeVirtualCallFromCtor) generator.CreateClassProxy(typeof (MakeVirtualCallFromCtor),
			                                                                                 new Type[0], logging);
			Assert.AreEqual(1, logging.Invocations.Count);
			Assert.IsNotNull(o);
		}

		[Test]
		public void InternalClassWithInternalMethodAndProperty()
		{
			LogInvocationInterceptor logging = new LogInvocationInterceptor();
			InternalClassWithInternalMembers o =
				(InternalClassWithInternalMembers) generator.CreateClassProxy(typeof (InternalClassWithInternalMembers),
				                                                              new Type[0], logging);
			Assert.IsNotNull(o);
			o.TestMethod();
			Assert.AreEqual(1, logging.Invocations.Count);
			string t = o.TestProperty;
			Assert.AreEqual(2, logging.Invocations.Count);
		}

		[Test]
		public void CanSetOutputParameterForDecimal()
		{
			IDecimalOutParam target =
				generator.CreateInterfaceProxyWithoutTarget<IDecimalOutParam>(new SetOutputParameter(1.234M));
			decimal fuel;
			target.Dance(out fuel);
			Assert.AreEqual(1.234M, fuel);
		}

		[Test]
		public void CanSetOutputParameterForDecimal_UsingGenericMethods()
		{
			IDecimalOutParam target =
				generator.CreateInterfaceProxyWithoutTarget<IDecimalOutParam>(new SetOutputParameter(1.234M));
			decimal fuel;
			target.Run(out fuel);
			Assert.AreEqual(1.234M, fuel);
		}

		[Test]
		public void CanProxyMethodWithOutIntPtrParameter()
		{
			IFooWithOutIntPtr o = (IFooWithOutIntPtr)generator.CreateInterfaceProxyWithoutTarget(
				typeof(IFooWithOutIntPtr), new Type[0], new SkipCallingMethodInterceptorWithOutputParams());
			Assert.IsNotNull(o);
			IntPtr i;
			o.Bar(out i);
		}

		public class SkipCallingMethodInterceptorWithOutputParams : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				invocation.Arguments[0] = IntPtr.Zero;
				invocation.ReturnValue = 5;
			}
		}

		public interface IWithEvents
		{
			event EventHandler Foo;
		}

		public class FakeWithEvents : IWithEvents
		{
			public virtual event EventHandler Foo;

			public void MethodToGetAroundFooNotUsedError()
			{
				Foo(this, EventArgs.Empty);
			}
		}

		public interface IMulti
		{
			void OriginalMethod1();
			void OriginalMethod2();
		}

		public class MultiClass : IMulti
		{
			// NON-virtual method
			public void OriginalMethod1()
			{
			}

			// VIRTUAL method
			public virtual void OriginalMethod2()
			{
			}
		}

		public interface ISpecialMulti : IMulti
		{
			void ExtraMethod();
		}

		[Guid("AFCF8240-61AF-4089-BD98-3CD4D719EDBA")]
		public interface IWithGuid
		{
		}
	}

	internal class SetOutputParameter : IInterceptor
	{
		private readonly decimal x;

		public SetOutputParameter(decimal x)
		{
			this.x = x;
		}

		public void Intercept(IInvocation invocation)
		{
			invocation.Arguments[0] = x;
		}
	}

	public interface IDecimalOutParam
	{
		void Dance(out decimal fuel);
		void Run<T>(out T fuel);
	}

	public class WithInternalMethod
	{
		internal virtual void Foo()
		{
		}
	}

	public interface IDoubleGeneric<One>
	{
		object Call<T>(One one, T two);
	}

	internal interface IFoo
	{
		int Bar();
	}

	public class DoubleGenericImpl<One> : IDoubleGeneric<One>
	{
		public object Call<T>(One one, T two)
		{
			return two;
		}
	}

	[ComImport]
	[Guid("6D5140C1-7436-11CE-8034-00AA006009FA")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	public interface IComServiceProvider
	{
		[return: MarshalAs(UnmanagedType.IUnknown)]
		object QueryService([In] ref Guid guidService, [In] ref Guid riid);
	}

	public interface IFactory2
	{
		T Create<T>() where T : List<T>;
	}

	public interface IFooWithIntPtr
	{
		IntPtr Buffer(UInt32 index);
	}

	public abstract class SomeClassWithProtectedInternalAbstractClass
	{
		protected internal abstract void Quack();
	}

	internal class InternalClassWithInternalMembers
	{
		internal virtual string TestMethod()
		{
			return "TestMethod";
		}

		internal virtual string TestProperty
		{
			get { return "TestProperty"; }
		}
	}

	public class MakeVirtualCallFromCtor
	{
		private string name;

		public MakeVirtualCallFromCtor()
		{
			Name = "Blah";
		}

		public virtual string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

    public interface IFooWithOutIntPtr
	{
		int Bar(out IntPtr i);
	}

	public class Foo : IFooWithOutIntPtr
	{
		public int Bar(out IntPtr i)
		{
			i = (IntPtr) Test();
			return 5;
		}

		private object Test()
		{
			return new IntPtr(3);
		}
	}

	public interface IStore1
	{
		R[] TestMethod<R>();
	}
}

#endif