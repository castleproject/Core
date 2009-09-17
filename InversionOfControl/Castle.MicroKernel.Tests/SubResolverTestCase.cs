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

namespace Castle.MicroKernel.Tests
{
	using Core;
	using NUnit.Framework;

	[TestFixture]
	public class SubResolverTestCase
	{
		[Test]
		public void WillAskResolverWhenTryingToResolveDependencyAfterAnotherHandlerWasRegistered()
		{
			FooBarResolver resolver = new FooBarResolver();

			IKernel kernel = new DefaultKernel();
			kernel.Resolver.AddSubResolver(resolver);

			kernel.AddComponent("foo", typeof(Foo));
			IHandler handler = kernel.GetHandler("foo");
			Assert.AreEqual(HandlerState.WaitingDependency, handler.CurrentState);

			resolver.Result = 15;

			//should force reevaluation of state
			kernel.RaiseHandlerRegistered(null);
			kernel.RaiseHandlersChanged();

			Assert.AreEqual(HandlerState.Valid, handler.CurrentState);
		}
	}

	public class Foo
	{
		private int bar;

		public Foo(int bar)
		{
			this.bar = bar;
		}
	}

	public class FooBarResolver : ISubDependencyResolver
	{
		public int? Result;

		public object Resolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
		                      DependencyModel dependency)
		{
			return Result.Value;
		}

		public bool CanResolve(CreationContext context, ISubDependencyResolver contextHandlerResolver, ComponentModel model,
		                       DependencyModel dependency)
		{
			return Result != null;
		}
	}
}