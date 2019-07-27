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
	using System.Collections.Generic;
	using System.Data;
	using System.Runtime.InteropServices;

#if FEATURE_CUSTOMMODIFIERS
	using System.Reflection;
	using System.Runtime.CompilerServices;
#endif

	using Castle.DynamicProxy.Tests.Interceptors;
	using Castle.DynamicProxy.Tests.Interfaces;

	using NUnit.Framework;

	[TestFixture]
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

		[Test]
		public void CanGetCorrectValuesFromIntPtr()
		{
			var o = (IFooWithIntPtr) generator
			                         	.CreateInterfaceProxyWithoutTarget(typeof (IFooWithIntPtr),
			                         	                                   new SetReturnValueInterceptor(IntPtr.Zero));
			var buffer = o.Buffer(15);
			Assert.AreEqual(IntPtr.Zero, buffer);
		}

#if FEATURE_TEST_DATASET
		[Test]
		public void CanProxyDataSet()
		{
			generator.CreateClassProxy(typeof (DataSet), new Type[0], new StandardInterceptor());
		}
#endif

#if FEATURE_CUSTOMMODIFIERS
		private Type iHaveMethodWithModOptsType;

		[OneTimeSetUp]
		public void GenerateDynamicAssemblyHavingModopts()
		{
			// One test below tries to proxy a type that has a method whose signature
			// involves an optional modifier (modopt). These are never produced by the
			// C# nor VB.NET compilers, but the C++/CLI compiler produces those. However,
			// if we added a C++/CLI project to this solution, it could only be compiled
			// on Windows (since the only extant C++/CLI compiler is MSVC). So what we do
			// instead to get some modopts is to generate a dynamic test assembly.
			//
			// Instead of using System.Reflection.AssemblyBuilder directly, we use
			// Castle's own ModuleScope since that seems the easiest way to get a strong-
			// named persistent assembly that can be referenced by DynamicProxy.

			const string assemblyName = "Rhino.Mocks.CPP.Interfaces";
			const string assemblyFileName = "Rhino.Mocks.CPP.Interfaces.dll";

			var moduleScope = new ModuleScope(
				savePhysicalAssembly: true,
				disableSignedModule: false,
				namingScope: new Generators.NamingScope(),
				strongAssemblyName: assemblyName,
				strongModulePath: assemblyFileName,
				weakAssemblyName: assemblyName,
				weakModulePath: assemblyFileName);

			// This is the C++/CLI type we want to generate:
			//
			//   namespace RhinoMocksCPPInterfaces {
			//       public interface class IHaveMethodWithModOpts {
			//	         virtual void StartLiveOnSlot(long int slotNumber);
			//       };
			//   }
			//
			// which corresponds to this IL:
			//
			//   .class interface public abstract auto ansi beforefieldinit RhinoMocksCPPInterfaces.IHaveMethodWithModOpts
			//   {
			//       .method public hidebysig newslot abstract virtual instance void StartLiveOnSlot(int32 modopt([mscorlib]System.Runtime.CompilerServices.IsLong) slotNumber) cil managed
			//       { }
			//   }

			var typeBuilder = moduleScope.DefineType(
				true,
				"RhinoMocksCPPInterfaces.IHaveMethodWithModOpts",
				TypeAttributes.Class | TypeAttributes.Interface | TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.AutoLayout | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit);

			var methodBuilder = typeBuilder.DefineMethod(
				"StartLiveOnSlot",
				MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual,
				returnType: typeof(void),
				returnTypeRequiredCustomModifiers: null,
				returnTypeOptionalCustomModifiers: null,
				parameterTypes: new[]
				{
					typeof(int)
				},
				parameterTypeRequiredCustomModifiers: null,
				parameterTypeOptionalCustomModifiers: new[]
				{
					new[] { typeof(IsLong) }
				},
				callingConvention: CallingConventions.Standard);

#if FEATURE_LEGACY_REFLECTION_API
			var iHaveMethodWithModOptsType = typeBuilder.CreateType();
#else
			var iHaveMethodWithModOptsType = typeBuilder.CreateTypeInfo().AsType();
#endif

#if FEATURE_ASSEMBLYBUILDER_SAVE && FEATURE_TEST_PEVERIFY
			// Let's persist and PE-verify the dynamic assembly before it gets used in tests:
			var assemblyPath = moduleScope.SaveAssembly();
			base.RunPEVerifyOnGeneratedAssembly(assemblyPath);
#endif

			this.iHaveMethodWithModOptsType = iHaveMethodWithModOptsType;
		}

		[Test]
		public void CanProxyMethodWithModOpt()
		{
			Assume.That(iHaveMethodWithModOptsType != null);

			var proxy = generator.CreateInterfaceProxyWithoutTarget(iHaveMethodWithModOptsType, new DoNothingInterceptor());
			var startLiveOnSlotMethod = iHaveMethodWithModOptsType.GetMethod("StartLiveOnSlot");
			startLiveOnSlotMethod.Invoke(proxy, new object[] { 4 });
		}
#endif

		[Test]
		public void CanProxyMethodWithOutIntPtrParameter()
		{
			var o = (IFooWithOutIntPtr) generator.CreateInterfaceProxyWithoutTarget(
				typeof (IFooWithOutIntPtr), new Type[0], new SkipCallingMethodInterceptorWithOutputParams());
			Assert.IsNotNull(o);
			IntPtr i;
			o.Bar(out i);
		}

		[Test]
		public void CanSetOutputParameterForDecimal()
		{
			var target =
				generator.CreateInterfaceProxyWithoutTarget<IDecimalOutParam>(new SetOutputParameter(1.234M));
			decimal fuel;
			target.Dance(out fuel);
			Assert.AreEqual(1.234M, fuel);
		}

		[Test]
		public void CanSetOutputParameterForDecimal_UsingGenericMethods()
		{
			var target =
				generator.CreateInterfaceProxyWithoutTarget<IDecimalOutParam>(new SetOutputParameter(1.234M));
			decimal fuel;
			target.Run(out fuel);
			Assert.AreEqual(1.234M, fuel);
		}

		[Test]
		public void GenericClassWithGenericMethod()
		{
			var logger = new LogInvocationInterceptor();
			var proxy =
				(IDoubleGeneric<int>) generator.CreateInterfaceProxyWithTarget(typeof (IDoubleGeneric<int>),
				                                                               new DoubleGenericImpl<int>(), logger);
			proxy.Call(1, "");
			Assert.AreEqual("Call", logger.Invocations[0]);
		}

		[Test]
		public void GenericClassWithGenericMethodWitoutTarget()
		{
			var interceptor = new SetReturnValueInterceptor(3);
			var proxy =
				(IDoubleGeneric<int>) generator.CreateInterfaceProxyWithoutTarget(typeof (IDoubleGeneric<int>),
				                                                                  interceptor);
			var o = proxy.Call(1, "");
			Assert.AreEqual(3, o);
		}

		[Test]
		public void GenericMethodReturningGenericArray()
		{
			generator.CreateInterfaceProxyWithoutTarget(
				typeof (IStore1),
				new DoNothingInterceptor());
		}

		[Test]
		public void InternalClassWithInternalMethodAndProperty()
		{

			var logging = new LogInvocationInterceptor();
			var o =
				(InternalClassWithInternalMembers) generator.CreateClassProxy(typeof (InternalClassWithInternalMembers),
				                                                              new Type[0], logging);
			Assert.IsNotNull(o);
			o.TestMethod();
			Assert.AreEqual(1, logging.Invocations.Count);
			var t = o.TestProperty;
			Assert.AreEqual(2, logging.Invocations.Count);
		}

		[Test]
		public void NeedingToCreateNewMethodTableSlot()
		{
			generator.CreateClassProxy(typeof (MultiClass), new[] {typeof (ISpecialMulti)});
		}

		[Test]
		public void ProxyInternalMethod()
		{
			var logging = new LogInvocationInterceptor();
			var o = (WithInternalMethod) generator.CreateClassProxy(typeof (WithInternalMethod),
			                                                        new Type[0], logging);
			o.Foo();
			Assert.AreEqual("Foo ", logging.LogContents);
		}

#if FEATURE_TEST_COM
		[Test]
		public void ProxyingComInteraces()
		{
			var o = generator
				.CreateInterfaceProxyWithoutTarget(typeof (IComServiceProvider), new StandardInterceptor());
			Assert.IsNotNull(o);
		}
#endif

		[Test]
		public void ProxyingGenericClassWithGenericClassConstraint()
		{
			var o = generator
				.CreateInterfaceProxyWithoutTarget(typeof (IFactory2), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingInterfaceWithGuid()
		{
			var o = generator.CreateInterfaceProxyWithoutTarget(typeof (IWithGuid), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingInternalInterface()
		{
			var o = generator.CreateInterfaceProxyWithoutTarget(typeof (IInternal), new StandardInterceptor());
			Assert.IsNotNull(o);
		}

		[Test]
		public void ProxyingProtectedInternalAbstractMethod()
		{
			var logging = new LogInvocationInterceptor();
			var o =
				(HasProtectedInternalAbstractMethod)
				generator.CreateClassProxy(typeof (HasProtectedInternalAbstractMethod),
				                           new Type[0], logging);
			Assert.IsNotNull(o);
		}

		[Test]
		public void UsingEvents_Class()
		{
			var logger = new LogInvocationInterceptor();
			var proxy = (FakeWithEvents) generator.CreateClassProxy(
				typeof (FakeWithEvents),
				ProxyGenerationOptions.Default,
				logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test]
		public void UsingEvents_Interface()
		{
			var logger = new LogInvocationInterceptor();

			var proxy = (IWithEvents) generator.CreateInterfaceProxyWithTarget(typeof (IWithEvents),
			                                                                   new FakeWithEvents(),
			                                                                   logger);

			Assert.IsNotNull(proxy);

			proxy.Foo += null;
			proxy.Foo -= null;

			Assert.AreEqual(2, logger.Invocations.Count);
		}

		[Test]
		public void VirtualMethodCallsFromTheConstructor()
		{
			var logging = new LogInvocationInterceptor();
			var o = (MakeVirtualCallFromCtor) generator.CreateClassProxy(typeof (MakeVirtualCallFromCtor),
			                                                             new Type[0], logging);
			Assert.AreEqual(1, logging.Invocations.Count);
			Assert.IsNotNull(o);
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
			i = (IntPtr) Test();
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
