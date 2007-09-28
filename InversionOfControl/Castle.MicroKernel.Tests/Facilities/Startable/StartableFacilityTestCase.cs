// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Facilities.Startable.Tests.Components;
	using Castle.MicroKernel;
	using NUnit.Framework;

	[TestFixture]
	public class StartableFacilityTestCase
	{
		private bool startableCreatedBeforeResolved;

		[SetUp]
		public void SetUp()
		{
			startableCreatedBeforeResolved = false;
		}

		[Test]
		public void TestInterfaceBasedStartable()
		{
			IKernel kernel = new DefaultKernel();
			kernel.ComponentCreated += new ComponentInstanceDelegate(OnStartableComponentStarted);

			kernel.AddFacility("startable", new StartableFacility());

			kernel.AddComponent("a", typeof(StartableComponent));

			Assert.IsTrue(startableCreatedBeforeResolved, "Component was not properly started");

			StartableComponent component = kernel["a"] as StartableComponent;

			Assert.IsNotNull(component);
			Assert.IsTrue(component.Started);
			Assert.IsFalse(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.IsTrue(component.Stopped);
		}

		[Test]
		public void TestComponentWithNoInterface()
		{
			IKernel kernel = new DefaultKernel();
			kernel.ComponentCreated += new ComponentInstanceDelegate(OnNoInterfaceStartableComponentStarted);

			MutableConfiguration compNode = new MutableConfiguration("component");
			compNode.Attributes["id"] = "b";
			compNode.Attributes["startable"] = "true";
			compNode.Attributes["startMethod"] = "Start";
			compNode.Attributes["stopMethod"] = "Stop";

			kernel.ConfigurationStore.AddComponentConfiguration("b", compNode);

			kernel.AddFacility("startable", new StartableFacility());
			kernel.AddComponent("b", typeof(NoInterfaceStartableComponent));

			Assert.IsTrue(startableCreatedBeforeResolved, "Component was not properly started");

			NoInterfaceStartableComponent component = kernel["b"] as NoInterfaceStartableComponent;

			Assert.IsNotNull(component);
			Assert.IsTrue(component.Started);
			Assert.IsFalse(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.IsTrue(component.Stopped);
		}

		[Test]
		public void TestStartableWithCustomDependencies()
		{
			IKernel kernel = new DefaultKernel();
			kernel.ComponentCreated += new ComponentInstanceDelegate(OnStartableComponentStarted);

			kernel.AddFacility("startable", new StartableFacility());

			Hashtable dependencies = new Hashtable();
			dependencies.Add("config", 1);
			kernel.AddComponent("a", typeof(StartableComponentWithCustomDependencies));
			kernel.RegisterCustomDependencies(typeof(StartableComponentWithCustomDependencies), dependencies);

			Assert.IsTrue(startableCreatedBeforeResolved, "Component was not properly started");

			StartableComponentWithCustomDependencies component = kernel["a"] as StartableComponentWithCustomDependencies;

			Assert.IsNotNull(component);
			Assert.IsTrue(component.Started);
			Assert.IsFalse(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.IsTrue(component.Stopped);
		}

		private void OnStartableComponentStarted(ComponentModel mode, object instance)
		{
			StartableComponent startable = instance as StartableComponent;

			Assert.IsNotNull(startable);
			Assert.IsTrue(startable.Started);
			Assert.IsFalse(startable.Stopped);

			startableCreatedBeforeResolved = true;
		}

		private void OnNoInterfaceStartableComponentStarted(ComponentModel mode, object instance)
		{
			NoInterfaceStartableComponent startable = instance as NoInterfaceStartableComponent;

			Assert.IsNotNull(startable);
			Assert.IsTrue(startable.Started);
			Assert.IsFalse(startable.Stopped);

			startableCreatedBeforeResolved = true;
		}
	}
}