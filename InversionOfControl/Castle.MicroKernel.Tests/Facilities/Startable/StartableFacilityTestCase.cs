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

namespace Castle.Facilities.Startable.Tests
{
	using System;
	using Castle.Core.Configuration;
	using NUnit.Framework;

	using Castle.MicroKernel;

	using Castle.Facilities.Startable.Tests.Components;


	[TestFixture]
	public class StartableFacilityTestCase
	{
		[Test]
		public void TestInterfaceBasedStartable()
		{
			IKernel kernel = new DefaultKernel();

			kernel.AddFacility( "startable", new StartableFacility() );

			kernel.AddComponent( "a", typeof(StartableComponent) );

			StartableComponent component = kernel["a"] as StartableComponent;

			Assert.IsNotNull(component);
			Assert.IsTrue( component.Started );
			Assert.IsFalse( component.Stopped );

			kernel.ReleaseComponent(component);
			Assert.IsTrue( component.Stopped );
		}

        [Test]
        public void TestComponentWithNoInterface()
        {
            IKernel kernel = new DefaultKernel();

			MutableConfiguration compNode = new MutableConfiguration("component");
			compNode.Attributes["id"] = "b";
			compNode.Attributes["startable"] = "true";
			compNode.Attributes["startMethod"] = "Start";
			compNode.Attributes["stopMethod"] = "Stop";

			kernel.ConfigurationStore.AddComponentConfiguration("b", compNode);

            kernel.AddFacility( "startable", new StartableFacility() );
            kernel.AddComponent( "b", typeof(NoInterfaceStartableComponent) );
            NoInterfaceStartableComponent component = kernel["b"] as NoInterfaceStartableComponent;

            Assert.IsNotNull(component);
            Assert.IsTrue( component.Started );
            Assert.IsFalse( component.Stopped );

            kernel.ReleaseComponent(component);
            Assert.IsTrue( component.Stopped );
        }
	}
}
