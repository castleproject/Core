// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Internal;

	using NUnit.Framework;

	/// <summary>
	///   This fixture checks which <see cref="IInvocation"/> types get used for proxied methods.
	///   Usually, DynamicProxy generates a separate implementation type per proxied method, but
	///   in some cases, it can reuse predefined implementation types. Because this is beneficial
	///   for runtime performance (as it reduces the amount of dynamic type generation performed),
	///   we want to ensure that those predefined types do in fact get picked when they should be.
	/// </summary>
	[TestFixture]
	public class InvocationTypeReuseTestCase : BasePEVerifyTestCase
	{
		[Test]
		public void Non_generic_method_of_interface_proxy_without_target__uses__InterfaceMethodWithoutTargetInvocation()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IWithNonGenericMethod>(recorder);
			proxy.Method();

			Assert.AreEqual(typeof(InterfaceMethodWithoutTargetInvocation), recorder.InvocationType);
		}

		[Test]
		public void Generic_method_of_interface_proxy_without_target__uses__InterfaceMethodWithoutTargetInvocation()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateInterfaceProxyWithoutTarget<IWithGenericMethod>(recorder);
			proxy.Method(42);

			Assert.AreEqual(typeof(InterfaceMethodWithoutTargetInvocation), recorder.InvocationType);
		}

		[Test]
		public void Non_generic_abstract_method_of_class_proxy__uses__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithNonGenericAbstractMethod>(recorder);
			proxy.Method();

			Assert.AreEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Non_generic_protected_abstract_method_of_class_proxy__uses__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithNonGenericProtectedAbstractMethod>(recorder);
			proxy.InvokeMethod();

			Assert.AreEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Non_generic_virtual_method_of_class_proxy__does_not_use__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithNonGenericVirtualMethod>(recorder);
			proxy.Method();

			Assert.AreNotEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Non_generic_protected_virtual_method_of_class_proxy__does_not_use__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithNonGenericProtectedVirtualMethod>(recorder);
			proxy.InvokeMethod();

			Assert.AreNotEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Generic_abstract_method_of_class_proxy__uses__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithGenericAbstractMethod>(recorder);
			proxy.Method(42);

			Assert.AreEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Generic_protected_abstract_method_of_class_proxy__uses__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithGenericProtectedAbstractMethod>(recorder);
			proxy.InvokeMethod(42);

			Assert.AreEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Generic_virtual_method_of_class_proxy__does_not_use__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithGenericVirtualMethod>(recorder);
			proxy.Method(42);

			Assert.AreNotEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		[Test]
		public void Generic_protected_virtual_method_of_class_proxy__does_not_use__InheritanceInvocationWithoutTarget()
		{
			var recorder = new InvocationTypeRecorder();

			var proxy = generator.CreateClassProxy<WithGenericProtectedVirtualMethod>(recorder);
			proxy.InvokeMethod(42);

			Assert.AreNotEqual(typeof(InheritanceInvocationWithoutTarget), recorder.InvocationType);
		}

		public interface IWithNonGenericMethod
		{
			void Method();
		}

		public interface IWithGenericMethod
		{
			void Method<T>(T arg);
		}

		public abstract class WithNonGenericAbstractMethod
		{
			public abstract void Method();
		}

		public abstract class WithNonGenericProtectedAbstractMethod
		{
			protected abstract void Method();
			public void InvokeMethod() => Method();
		}

		public class WithNonGenericVirtualMethod
		{
			public virtual void Method() { }
		}

		public class WithNonGenericProtectedVirtualMethod
		{
			protected virtual void Method() { }
			public void InvokeMethod() => Method();
		}

		public abstract class WithGenericAbstractMethod
		{
			public abstract void Method<T>(T arg);
		}

		public abstract class WithGenericProtectedAbstractMethod
		{
			protected abstract void Method<T>(T arg);
			public void InvokeMethod<T>(T arg) => Method(arg);
		}

		public class WithGenericVirtualMethod
		{
			public virtual void Method<T>(T arg) { }
		}

		public class WithGenericProtectedVirtualMethod
		{
			protected virtual void Method<T>(T arg) { }
			public void InvokeMethod<T>(T arg) => Method(arg);
		}

		private sealed class InvocationTypeRecorder : IInterceptor
		{
			public Type InvocationType { get; private set; }

			public void Intercept(IInvocation invocation)
			{
				InvocationType = invocation.GetType();
			}
		}
	}
}
