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
	using System.Runtime.Remoting;
	using Castle.MicroKernel;
	using NUnit.Framework;

	using Castle.Windsor.Tests.Components;
	
	[TestFixture]
	public class PropertiesInspectionBehaviorTestCase
	{
		[Test]
		public void PropertiesInspectionTestCase()
		{
			IWindsorContainer container;
			
			container = new WindsorContainer("../Castle.Windsor.Tests/propertyInspectionBehavior.xml");
			
			ExtendedComponentWithProperties comp;
			
			comp = (ExtendedComponentWithProperties) container["comp1"]; // None
			Assert.IsNull(comp.Prop1);
			Assert.AreEqual(0, comp.Prop2);
			Assert.AreEqual(0, comp.Prop3);

			comp = (ExtendedComponentWithProperties) container["comp2"]; // All
			Assert.IsNotNull(comp.Prop1);
			Assert.AreEqual(1, comp.Prop2);
			Assert.AreEqual(2, comp.Prop3);

			comp = (ExtendedComponentWithProperties) container["comp3"]; // DeclaredOnly
			Assert.IsNull(comp.Prop1);
			Assert.AreEqual(0, comp.Prop2);
			Assert.AreEqual(2, comp.Prop3);
		}

		[Test, ExpectedException(typeof(KernelException), "Error on properties inspection. Could not convert the inspectionBehavior attribute value into an expected enum value. Value found is 'Invalid' while possible values are 'Undefined,None,All,DeclaredOnly'")]
		public void InvalidOption()
		{
			IWindsorContainer container;
			
			container = new WindsorContainer("../Castle.Windsor.Tests/propertyInspectionBehaviorInvalid.xml");
		}
	}
}
