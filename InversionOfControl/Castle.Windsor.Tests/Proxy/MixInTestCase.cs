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

namespace Castle.Windsor.Tests.Proxy
{
	using Castle.MicroKernel.Registration;
	using Castle.Windsor.Tests.Components;
	using NUnit.Framework;

	[TestFixture]
	public class ComponentRegistrationTestCase
	{
		private WindsorContainer container;

		[SetUp]
		public void SetUp()
		{
			container = new WindsorContainer();
		}

		[Test]
		public void AddComponent_WithMixIn_AddsMixin()
		{
			container.Register(Component.For<ICalcService>()
				.ImplementedBy<CalculatorService>()
				.Proxy.MixIns(new SimpleMixIn())
				);

			ICalcService calculator = container.Resolve<ICalcService>();
			Assert.IsInstanceOf(typeof(ISimpleMixIn), calculator);

			ISimpleMixIn mixin = (ISimpleMixIn)calculator;
			mixin.DoSomething();
		}
	}
}