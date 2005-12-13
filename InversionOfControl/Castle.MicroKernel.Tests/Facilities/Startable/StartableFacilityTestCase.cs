// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using Castle.Model.Configuration;
	using NUnit.Framework;

	using Castle.MicroKernel;

	using Castle.Facilities.Startable.Tests.Components;


	[TestFixture]
	public class StartableFacilityTestCase
	{
		[Test]
		public void SimpleCase()
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
        public void NoInterfaceCase()
        {
            IKernel kernel = new DefaultKernel();
            this.GetConfig(kernel);

            kernel.AddFacility( "startable", new StartableFacility() );

            kernel.AddComponent( "b", typeof(NoInterfaceStartableComponent) );

            NoInterfaceStartableComponent component = kernel["b"] as NoInterfaceStartableComponent;

            Assert.IsNotNull(component);
            Assert.IsTrue( component.Started );
            Assert.IsFalse( component.Stopped );

            kernel.ReleaseComponent(component);
            Assert.IsTrue( component.Stopped );
        }

        [Test]
        public void OddMethodsCase()
        {
            IKernel kernel = new DefaultKernel();
            this.GetConfig2(kernel);
            
            kernel.AddFacility( "startable", new StartableFacility() );

            kernel.AddComponent( "c", typeof(OddStartStopMethodsComponent) );

            OddStartStopMethodsComponent component = kernel["c"] as OddStartStopMethodsComponent;

            Assert.IsNotNull(component);
            Assert.IsTrue( component.Started );
            Assert.IsFalse( component.Stopped );

            kernel.ReleaseComponent(component);
            Assert.IsTrue( component.Stopped );
        }

        private void GetConfig(IKernel kernel)
        {
            MutableConfiguration confignode = new MutableConfiguration("startables");

            MutableConfiguration startableNode = new MutableConfiguration("startable");
            
            startableNode.Attributes.Add("id", "do I need this");
            startableNode.Attributes.Add("type", "Castle.Facilities.Startable.Tests.Components.NoInterfaceStartableComponent, Castle.MicroKernel.Tests");

            confignode.Children.Add(startableNode);


            kernel.ConfigurationStore.AddFacilityConfiguration("startable", confignode);
        }

        private void GetConfig2(IKernel kernel)
        {
            MutableConfiguration confignode = new MutableConfiguration("startables");

            MutableConfiguration startableNode = new MutableConfiguration("startable");
            
            startableNode.Attributes.Add("id", "do I need this");
            startableNode.Attributes.Add("type", "Castle.Facilities.Startable.Tests.Components.OddStartStopMethodsComponent, Castle.MicroKernel.Tests");
            startableNode.Attributes.Add("startMethod", "Starter");
            startableNode.Attributes.Add("stopMethod", "Stopper");

            confignode.Children.Add(startableNode);


            kernel.ConfigurationStore.AddFacilityConfiguration("startable", confignode);
        }
	}
}
