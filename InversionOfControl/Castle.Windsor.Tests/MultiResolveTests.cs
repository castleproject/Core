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

namespace Castle.Windsor.Tests
{
	using System;
	using Castle.MicroKernel.Handlers;
	using NUnit.Framework;

	[TestFixture]
	public class MultiResolveTests
	{
		[Test]
		public void CanResolveMoreThanSingleComponentForService()
		{
			IWindsorContainer container = new WindsorContainer()
				.AddComponent<IClock, IsraelClock>()
				.AddComponent<IClock, WorldClock>();

			IClock[] clocks = container.ResolveAll<IClock>();

			Assert.AreEqual(2, clocks.Length);
		}

		[Test]
		public void MultiResolveWillResolveInRegistrationOrder()
		{
			IWindsorContainer container = new WindsorContainer()
				.AddComponent<IClock, IsraelClock>()
				.AddComponent<IClock, WorldClock>();

			IClock[] clocks = container.ResolveAll<IClock>();

			Assert.AreEqual(typeof(IsraelClock), clocks[0].GetType());
			Assert.AreEqual(typeof(WorldClock), clocks[1].GetType());

			//reversing order
			container = new WindsorContainer()
				.AddComponent<IClock, WorldClock>()
				.AddComponent<IClock, IsraelClock>();

			clocks = container.ResolveAll<IClock>();

			Assert.AreEqual(typeof(WorldClock), clocks[0].GetType());
			Assert.AreEqual(typeof(IsraelClock), clocks[1].GetType());
		}

		[Test]
		public void CanUseMutliResolveWithGenericSpecialization()
		{
			IWindsorContainer container = new WindsorContainer()
				.AddComponent("demo", typeof (IRepository<>), typeof (DemoRepository<>))
				.AddComponent("trans", typeof(IRepository<>), typeof(TransientRepository<>));

			IRepository<IClock> resolve = container.Resolve<IRepository<IClock>>();
			Assert.IsNotNull(resolve);

			IRepository<IsraelClock>[] repositories = container.ResolveAll<IRepository<IsraelClock>>();
			Assert.AreEqual(2, repositories.Length);
		}
	}

	public interface IClock{}
	public class IsraelClock : IClock{}
	public class WorldClock : IClock{}
	public class DependantClock : IClock
	{
		public DependantClock(IDisposable disposable)
		{
		}
	}
}