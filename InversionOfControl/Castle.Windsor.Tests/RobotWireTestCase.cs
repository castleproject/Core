// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	using NUnit.Framework;

	using Castle.Windsor.Tests.Components;

	/// <summary>
	/// Check for existence of any problem, as 
	/// reported http://forum.castleproject.org/posts/list/214.page
	/// </summary>
	[TestFixture]
	public class RobotWireTestCase
	{
		private IWindsorContainer container;

		[Test]
		public void WireTest()
		{
			container = new WindsorContainer(ConfigHelper.ResolveConfigPath("robotwireconfig.xml"));

			Robot robot = (Robot) container[ typeof(Robot) ];
			
			Assert.IsNotNull(robot);
			Assert.IsNotNull(robot.Left);
			Assert.IsNotNull(robot.Right);

			Assert.AreEqual("PlasmaGunArm", robot.Left.GetType().Name);
			Assert.AreEqual("HumanArm", robot.Right.GetType().Name);
		}
	}
}
