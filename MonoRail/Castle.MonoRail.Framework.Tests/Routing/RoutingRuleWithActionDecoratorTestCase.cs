// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Routing
{
	using Castle.MonoRail.Framework.Routing;
	using NUnit.Framework;
	using Rhino.Mocks;

	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class RoutingRuleWithActionDecoratorTestCase
	{
		private MockRepository mockRepository = new MockRepository();

		[Test]
		public void DoesNotRunActionIfThereIsNoMatching()
		{
			IRoutingRule mockRule = mockRepository.CreateMock<IRoutingRule>();

			using(mockRepository.Record())
			{
				Expect.Call(mockRule.Matches(null, null, null)).
					IgnoreArguments().
					Return(false);
			}

			using(mockRepository.Playback())
			{
				bool wasCalled = false;

				RoutingRuleWithActionDecorator dec = 
					new RoutingRuleWithActionDecorator(mockRule,
						delegate(IRouteContext ctx, RouteMatch match) { wasCalled = true; });

				dec.Matches(null, null, new RouteMatch());

				Assert.IsFalse(wasCalled);
			}
		}

		[Test]
		public void RunsActionIfRuleMatches()
		{
			IRoutingRule mockRule = mockRepository.CreateMock<IRoutingRule>();

			using(mockRepository.Record())
			{
				Expect.Call(mockRule.Matches(null, null, null)).
					IgnoreArguments().
					Return(true);
			}

			using(mockRepository.Playback())
			{
				bool wasCalled = false;

				RoutingRuleWithActionDecorator dec =
					new RoutingRuleWithActionDecorator(mockRule,
						delegate(IRouteContext ctx, RouteMatch match) { wasCalled = true; });

				dec.Matches(null, null, new RouteMatch());

				Assert.IsTrue(wasCalled);
			}
		}
	}
}
