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

#if !NETCORE
namespace Castle.DynamicProxy.Tests
{
	using System;
	using System.Collections.Generic;
#if !SILVERLIGHT && !NETCORE
	using System.Data;
#endif
	using System.Runtime.InteropServices;

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using RhinoMocksCPPInterfaces;

	using Xunit;

	public class RhinoMocksTestCase : BasePEVerifyTestCase
	{
		public class SkipCallingMethodInterceptorWithOutputParams : IInterceptor
		{
#region IInterceptor Members

			public void Intercept(IInvocation invocation)
			{
				invocation.Arguments[0] = IntPtr.Zero;
				invocation.ReturnValue = 5;
			}

#endregion
		}

		public interface IWithEvents
		{
			event EventHandler Foo;
		}

		public class FakeWithEvents : IWithEvents
		{
#region IWithEvents Members

			public virtual event EventHandler Foo;

#endregion

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

#region IMulti Members

			public void OriginalMethod1()
			{
			}

			// VIRTUAL method
			public virtual void OriginalMethod2()
			{
			}

#endregion
		}

		public interface ISpecialMulti : IMulti
		{
			void ExtraMethod();
		}

		[Guid("AFCF8240-61AF-4089-BD98-3CD4D719EDBA")]
		public interface IWithGuid
		{
		}

		[Fact]
		public void CanGetCorrectValuesFromIntPtr()
		{
			var o = (IFooWithIntPtr)generator
				.CreateInterfaceProxyWithoutTarget(typeof(IFooWithIntPtr),
					new SetReturnValueInterceptor(IntPtr.Zero));
			var buffer = o.Buffer(15);
			Assert.Equal(IntPtr.Zero, buffer);
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void CanProxyDataSet()
		{
			generator.CreateClassProxy(typeof(DataSet), new Type[0], new StandardInterceptor());
		}

		[Fact]
		public void CanProxyMethodWithModOpt()
		{
			var proxy =
				(IHaveMethodWithModOpts)
					generator.CreateInterfaceProxyWithoutTarget(typeof(IHaveMethodWithModOpts), new DoNothingInterceptor());
			proxy.StartLiveOnSlot(4);
		}
#endif

		[Fact]
		public void CanProxyMethodWithOutIntPtrParameter()
		{
			var o = (IFooWithOutIntPtr)generator.CreateInterfaceProxyWithoutTarget(
				typeof(IFooWithOutIntPtr), new Type[0], new SkipCallingMethodInterceptorWithOutputParams());
			Assert.NotNull(o);
			IntPtr i;
			o.Bar(out i);
		}

		[Fact]
		public void CanSetOutputParameterForDecimal()
		{
			var target =
				generator.CreateInterfaceProxyWithoutTarget<IDecimalOutParam>(new SetOutputParameter(1.234M));
			decimal fuel;
			target.Dance(out fuel);
			Assert.Equal(1.234M, fuel);
		}

		[Fact]
		public void CanSetOutputParameterForDecimal_UsingGenericMethods()
		{
			var target =
				generator.CreateInterfaceProxyWithoutTarget<IDecimalOutParam>(new SetOutputParameter(1.234M));
			decimal fuel;
			target.Run(out fuel);
			Assert.Equal(1.234M, fuel);
		}

		[Fact]
		public void GenericClassWithGenericMethod()
		{
			var logger = new LogInvocationInterceptor();
			var proxy =
				(IDoubleGeneric<int>)generator.CreateInterfaceProxyWithTarget(typeof(IDoubleGeneric<int>),
					new DoubleGenericImpl<int>(), logger);
			proxy.Call(1, "");
			Assert.Equal("Call", logger.Invocations[0]);
		}

		[Fact]
		public void GenericClassWithGenericMethodWitoutTarget()
		{
			var interceptor = new SetReturnValueInterceptor(3);
			var proxy =
				(IDoubleGeneric<int>)generator.CreateInterfaceProxyWithoutTarget(typeof(IDoubleGeneric<int>),
					interceptor);
			var o = proxy.Call(1, "");
			Assert.Equal(3, o);
		}

		[Fact]
		public void GenericMethodReturningGenericArray()
		{
			generator.CreateInterfaceProxyWithoutTarget(
				typeof(IStore1),
				new DoNothingInterceptor());
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void InternalClassWithInternalMethodAndProperty()
		{
			var logging = new LogInvocationInterceptor();
			var o =
				(InternalClassWithInternalMembers)generator.CreateClassProxy(typeof(InternalClassWithInternalMembers),
					new Type[0], logging);
			Assert.NotNull(o);
			o.TestMethod();
			Assert.Equal(1, logging.Invocations.Count);
			var t = o.TestProperty;
			Assert.Equal(2, logging.Invocations.Count);
		}
#endif

		[Fact]
		public void NeedingToCreateNewMethodTableSlot()
		{
			generator.CreateClassProxy(typeof(MultiClass), new[] { typeof(ISpecialMulti) });
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void ProxyInternalMethod()
		{
			var logging = new LogInvocationInterceptor();
			var o = (WithInternalMethod)generator.CreateClassProxy(typeof(WithInternalMethod),
				new Type[0], logging);
			o.Foo();
			Assert.Equal("Foo ", logging.LogContents);
		}
#endif

		[Fact]
		public void ProxyingComInteraces()
		{
			var o = generator
				.CreateInterfaceProxyWithoutTarget(typeof(IComServiceProvider), new StandardInterceptor());
			Assert.NotNull(o);
		}

		[Fact]
		public void ProxyingGenericClassWithGenericClassConstraint()
		{
			var o = generator
				.CreateInterfaceProxyWithoutTarget(typeof(IFactory2), new StandardInterceptor());
			Assert.NotNull(o);
		}

		[Fact]
		public void ProxyingInterfaceWithGuid()
		{
			var o = generator.CreateInterfaceProxyWithoutTarget(typeof(IWithGuid), new StandardInterceptor());
			Assert.NotNull(o);
		}

#if !SILVERLIGHT && !NETCORE
		[Fact]
		public void ProxyingInternalInterface()
		{
			var o = generator.CreateInterfaceProxyWithoutTarget(typeof(IInternal), new StandardInterceptor());
			Assert.NotNull(o);
		}
#endif

		[Fact]
		public void ProxyingProtectedInternalAbstractMethod()
		{
			var logging = new LogInvocationInterceptor();
			var o =
				(HasProtectedInternalAbstractMethod)
					generator.CreateClassProxy(typeof(HasProtectedInternalAbstractMethod),
						new Type[0], logging);
			Assert.NotNull(o);
		}

		[Fact]
		public void UsingEvents_Class()
		{
			var logger = new LogInvocationInterceptor();
			var proxy = (FakeWithEvents)generator.CreateClassProxy(
				typeof(FakeWithEvents),
				ProxyGenerationOptions.Default,
				logger);

			Assert.NotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.Equal(2, logger.Invocations.Count);
		}

		[Fact]
		public void UsingEvents_Interface()
		{
			var logger = new LogInvocationInterceptor();

			var proxy = (IWithEvents)generator.CreateInterfaceProxyWithTarget(typeof(IWithEvents),
				new FakeWithEvents(),
				logger);

			Assert.NotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.Equal(2, logger.Invocations.Count);
		}

		[Fact]
		public void VirtualMethodCallsFromTheConstructor()
		{
			var logging = new LogInvocationInterceptor();
			var o = (MakeVirtualCallFromCtor)generator.CreateClassProxy(typeof(MakeVirtualCallFromCtor),
				new Type[0], logging);
			Assert.Equal(1, logging.Invocations.Count);
			Assert.NotNull(o);
		}
	}

	internal class SetOutputParameter : IInterceptor
	{
		private readonly decimal x;

		public SetOutputParameter(decimal x)
		{
			this.x = x;
		}

#region IInterceptor Members

		public void Intercept(IInvocation invocation)
		{
			invocation.Arguments[0] = x;
		}

#endregion
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

	internal interface IInternal
	{
		int Bar();
	}

	public class DoubleGenericImpl<One> : IDoubleGeneric<One>
	{
#region IDoubleGeneric<One> Members

		public object Call<T>(One one, T two)
		{
			return two;
		}

#endregion
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

	public abstract class HasProtectedInternalAbstractMethod
	{
		protected internal abstract void Quack();
	}

	internal class InternalClassWithInternalMembers
	{
		internal virtual string TestProperty
		{
			get { return "TestProperty"; }
		}

		internal virtual string TestMethod()
		{
			return "TestMethod";
		}
	}

	public class MakeVirtualCallFromCtor
	{
		public MakeVirtualCallFromCtor()
		{
			Name = "Blah";
		}

		public virtual string Name { get; set; }
	}

	public class Foo : IFooWithOutIntPtr
	{
#region IFooWithOutIntPtr Members

		public int Bar(out IntPtr i)
		{
			i = (IntPtr)Test();
			return 5;
		}

#endregion

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