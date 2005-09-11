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

namespace Castle.Facilities.BatchRegistration.Tests
{
	using System;
	using System.IO;
	using System.Configuration;

	using NUnit.Framework;

	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Configuration;

	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Configuration.Sources;

	using Castle.Facilities.BatchRegistration.Tests.Components;

	[TestFixture]
	public class BatchRegistrationFacilityTestCase
	{
		private IKernel _kernel;

		[SetUp]
		public void Init()
		{
			_kernel = new DefaultKernel();
		}

		[TearDown]
		public void Finish()
		{
			_kernel.Dispose();
		}

		[Test]
		public void UsingAttributes()
		{
			String xml = 
				"<configuration>" + 
				"	<facilities>" + 
				"		<facility id=\"batchregistration\">" + 
				"			<assemblyBatch name=\"Castle.Facilities.BatchRegistration.Tests\" useAttributes=\"true\" />" + 
				"		</facility>" + 
				"	</facilities>" + 
				"</configuration>";

			XmlInterpreter interpreter = new XmlInterpreter( new StaticContentSource(xml) );
			interpreter.Process(_kernel.ConfigurationStore);

			_kernel.AddFacility( "batchregistration", new BatchRegistrationFacility() );

			Assert.IsTrue( _kernel.HasComponent("comp1") );
			Assert.IsTrue( _kernel.HasComponent("comp2") );
			Assert.IsTrue( _kernel.HasComponent( typeof(Component1) ) );
			Assert.IsTrue( _kernel.HasComponent( typeof(Component2) ) );
		}

		[Test]
		public void UsingAttributesWithExcludes()
		{
			String xml = 
				"<configuration>" + 
				"	<facilities>" + 
				"		<facility id=\"batchregistration\">" + 
				"			<assemblyBatch name=\"Castle.Facilities.BatchRegistration.Tests\" useAttributes=\"true\" >" + 
				"				<exclude type=\"Castle.Facilities.BatchRegistration.Tests.Components.Component2\" />" + 
				"			</assemblyBatch>" + 
				"		</facility>" + 
				"	</facilities>" + 
				"</configuration>";

			XmlInterpreter interpreter = new XmlInterpreter( new StaticContentSource(xml) );
			interpreter.Process(_kernel.ConfigurationStore);

			_kernel.AddFacility( "batchregistration", new BatchRegistrationFacility() );

			Assert.IsTrue( _kernel.HasComponent("comp1") );
			Assert.IsFalse( _kernel.HasComponent("comp2") );
			Assert.IsTrue( _kernel.HasComponent( typeof(Component1) ) );
			Assert.IsFalse( _kernel.HasComponent( typeof(Component2) ) );
		}

		[Test]
		public void Includes()
		{
			String xml = 
				"<configuration>" + 
				"	<facilities>" + 
				"		<facility id=\"batchregistration\">" + 
				"			<assemblyBatch name=\"Castle.Facilities.BatchRegistration.Tests\" useAttributes=\"false\" >" + 
				"				<include key=\"other\" component=\"Castle.Facilities.BatchRegistration.Tests.Components.OtherComponent\" />" + 
				"			</assemblyBatch>" + 
				"		</facility>" + 
				"	</facilities>" + 
				"</configuration>";

			XmlInterpreter interpreter = new XmlInterpreter( new StaticContentSource(xml) );
			interpreter.Process(_kernel.ConfigurationStore);

			_kernel.AddFacility( "batchregistration", new BatchRegistrationFacility() );

			Assert.IsTrue( _kernel.HasComponent("other") );
			Assert.IsFalse( _kernel.HasComponent("comp2") );
			Assert.IsTrue( _kernel.HasComponent( typeof(OtherComponent) ) );
			Assert.IsFalse( _kernel.HasComponent( typeof(Component2) ) );
		}

		[Test]
		[ExpectedException( typeof(ConfigurationException) )]
		public void InvalidAssemblyName()
		{
			String xml = 
				"<configuration>" + 
				"	<facilities>" + 
				"		<facility id=\"batchregistration\">" + 
				"			<assemblyBatch name=\"MyCastle.Facilities.BatchRegistration.Tests\" useAttributes=\"false\" >" + 
				"				<include key=\"other\" component=\"Castle.Facilities.BatchRegistration.Tests.Components.OtherComponent\" />" + 
				"			</assemblyBatch>" + 
				"		</facility>" + 
				"	</facilities>" + 
				"</configuration>";

			XmlInterpreter interpreter = new XmlInterpreter( new StaticContentSource(xml) );
			interpreter.Process(_kernel.ConfigurationStore);

			_kernel.AddFacility( "batchregistration", new BatchRegistrationFacility() );
		}

		[Test]
		public void AddFacilities()
		{
			String xml = 
				"<configuration>" + 
				"	<facilities>" + 
				"		<facility id=\"batchregistration\">" + 
				"			<addFacility id=\"facility1\" type=\"Castle.Facilities.BatchRegistration.Tests.Facilities.Facility1, Castle.Facilities.BatchRegistration.Tests\" />" + 
				"			<addFacility id=\"facility2\" type=\"Castle.Facilities.BatchRegistration.Tests.Facilities.Facility2, Castle.Facilities.BatchRegistration.Tests\" />" + 
				"		</facility>" + 
				"	</facilities>" + 
				"</configuration>";

			XmlInterpreter interpreter = new XmlInterpreter( new StaticContentSource(xml) );
			interpreter.Process(_kernel.ConfigurationStore);

			_kernel.AddFacility( "batchregistration", new BatchRegistrationFacility() );

			IFacility[] facilities = _kernel.GetFacilities();
			Assert.AreEqual( 3, facilities.Length );				
		}
	}
}
