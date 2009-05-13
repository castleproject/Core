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

namespace Castle.Facilities.Startable.Tests
{
	using System.Collections;
	using Castle.Core;
	using Castle.Core.Configuration;
	using Castle.Facilities.Startable.Tests.Components;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;
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
		public void TestStartableWithRegisteredCustomDependencies()
		{
			IKernel kernel = new DefaultKernel();
			kernel.ComponentCreated += new ComponentInstanceDelegate(OnStartableComponentStarted);

			kernel.AddFacility("startable", new StartableFacility());

			Hashtable dependencies = new Hashtable();
			dependencies.Add("config", 1);
			kernel.AddComponent("a", typeof(StartableComponentCustomDependencies));
			kernel.RegisterCustomDependencies(typeof(StartableComponentCustomDependencies), dependencies);

			Assert.IsTrue(startableCreatedBeforeResolved, "Component was not properly started");

			StartableComponentCustomDependencies component = kernel["a"] as StartableComponentCustomDependencies;

			Assert.IsNotNull(component);
			Assert.IsTrue(component.Started);
			Assert.IsFalse(component.Stopped);

			kernel.ReleaseComponent(component);
			Assert.IsTrue(component.Stopped);
		}

		[Test]
		public void TestStartableCustomDependencies()
		{
			IKernel kernel = new DefaultKernel();
			kernel.ComponentCreated += new ComponentInstanceDelegate(OnStartableComponentStarted);

			kernel.AddFacility("startable", new StartableFacility());

			kernel.Register(
				Component.For<StartableComponentCustomDependencies>()
					.Named("a")
					.DependsOn(Property.ForKey("config").Eq(1))
				);
			Assert.IsTrue(startableCreatedBeforeResolved, "Component was not properly started");

			StartableComponentCustomDependencies component = kernel["a"] as StartableComponentCustomDependencies;

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

		/// <summary>
		/// This test has one startable component dependent on another, and both are dependent
		/// on a third generic component - all are singletons. We need to make sure we only get
		/// one instance of each component created.
		/// </summary>
		[Test]
		public void TestStartableChainWithGenerics()
		{
			IKernel kernel = new DefaultKernel();

			kernel.AddFacility("startable", new StartableFacility());

			// Add parent. This has a dependency so won't be started yet.
			kernel.AddComponent("chainparent", typeof(StartableChainParent));

			Assert.AreEqual(0, StartableChainDependency.startcount);
			Assert.AreEqual(0, StartableChainDependency.createcount);

			// Add generic dependency. This is not startable so won't get created. 
			kernel.AddComponent("chaingeneric", typeof(StartableChainGeneric<>));

			Assert.AreEqual(0, StartableChainDependency.startcount);
			Assert.AreEqual(0, StartableChainDependency.createcount);

			// Add dependency. This will satisfy the dependency so everything will start.
			kernel.AddComponent("chaindependency", typeof(StartableChainDependency));

			Assert.AreEqual(1, StartableChainParent.startcount);
			Assert.AreEqual(1, StartableChainParent.createcount);
			Assert.AreEqual(1, StartableChainDependency.startcount);
			Assert.AreEqual(1, StartableChainDependency.createcount);
			Assert.AreEqual(1, StartableChainGeneric<string>.createcount);
		}
	}
}