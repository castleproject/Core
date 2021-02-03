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

		public interface IWithNonGenericMethod
		{
			void Method();
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
