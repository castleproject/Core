// Copyright 2004-2017 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;

	using Castle.DynamicProxy.Tests.Interceptors;

	using NUnit.Framework;

	[TestFixture]
	public class AbstractInvocationTestCase
	{
		[Test]
		public void CouldArgumentValueHaveChanged_DoesNotTrackModifications_IfMethodHasNoByRefParameters()
		{
			// For methods where all parameters are passed by value, `CouldArgumentValueHaveChanged` will always
			// returns true, even when we haven't do anything at all with the invocation. This allows us to
			// optimize for this very common case.

			var invocation = GetAbstractInvocationFor(nameof(FakeProxy.MethodIII), 1, 2, 3);

			Assert.True(invocation.CouldArgumentValueHaveChanged(0));
			Assert.True(invocation.CouldArgumentValueHaveChanged(1));
			Assert.True(invocation.CouldArgumentValueHaveChanged(2));
		}

		[Test]
		public void CouldArgumentValueHaveChanged_TracksModifications_IfMethodHasByRefParameters()
		{
			// For methods where at least one parameter is passed by reference, `CouldArgumentValueHaveChanged`
			// can compare whether invocation arguments were modified since the start of the invocation. This
			// test demonstrates that if arguments are not modified, then this can be detected unlike in the
			// above test:

			var invocation = GetAbstractInvocationFor(nameof(FakeProxy.MethodIRO), 1, 2, 3);

			Assert.False(invocation.CouldArgumentValueHaveChanged(0));
			Assert.False(invocation.CouldArgumentValueHaveChanged(1));
			Assert.False(invocation.CouldArgumentValueHaveChanged(2));
		}

		[Test]
		public void CouldArgumentValueHaveChanged_TracksModificatonsViaArgumentsAndSetArgumentValue_IfMethodHasByRefParameters()
		{
			// This demonstrates that is doesn't matter how an argument is modified (either through `Arguments`
			// or with `SetArgumentValue`):

			var invocation = GetAbstractInvocationFor(nameof(FakeProxy.MethodIRO), 1, 2, 3);

			invocation.SetArgumentValue(0, 11);
			invocation.Arguments[1] = 22;
			invocation.SetArgumentValue(2, 33);

			Assert.True(invocation.CouldArgumentValueHaveChanged(0));
			Assert.True(invocation.CouldArgumentValueHaveChanged(1));
			Assert.True(invocation.CouldArgumentValueHaveChanged(2));
		}

		private static AbstractInvocation GetAbstractInvocationFor(string methodName, params object[] arguments)
		{
			var proxy = new FakeProxy();
			var interceptors = new IInterceptor[] { new DoNothingInterceptor() };
			var proxiedMethod = typeof(FakeProxy).GetMethod(methodName);
			return new SomeAbstractInvocation(proxy, interceptors, proxiedMethod, arguments);
		}

		public class FakeProxy
		{
			public virtual void MethodIII(int arg1, int arg2, int arg3) { }
			public virtual void MethodIRO(int arg1, ref int arg2, out int arg3) { arg3 = default(int); }
		}

		public sealed class SomeAbstractInvocation : AbstractInvocation
		{
			public SomeAbstractInvocation(object proxy, IInterceptor[] interceptors, MethodInfo proxiedMethod, object[] arguments)
				: base(proxy, interceptors, proxiedMethod, arguments)
			{
			}

			public override object InvocationTarget => throw new NotSupportedException();

			public override Type TargetType => throw new NotSupportedException();

			public override MethodInfo MethodInvocationTarget => throw new NotSupportedException();

			protected override void InvokeMethodOnTarget()
			{
				throw new NotSupportedException();
			}
		}
	}
}
