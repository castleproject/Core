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

namespace Castle.Facilities.Prevalence.Tests
{
	using System;
	using System.IO;

	using Bamboo.Prevalence;
	using Bamboo.Prevalence.Util;
	
	using Castle.MicroKernel;
	using Castle.Model.Configuration;

	using NUnit.Framework;

	/// <summary>
	/// Summary description for FacilityTestCase.
	/// </summary>
	[TestFixture]
	public class FacilityTestCase
	{
		String _storageDir;

		[SetUp]
		public void Init()
		{
			_storageDir = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "storage" );
		}

		[TearDown]
		public void Clean()
		{
			Directory.Delete(_storageDir, true);
		}

		[Test]
		public void TestUsage()
		{
			IKernel kernel = CreateConfiguredKernel();
			kernel.AddFacility( "prevalence", new PrevalenceFacility() );
		
			// Lookup for the engine
			object engineService = kernel["engineid"];
			Assert.IsNotNull( engineService );
			Assert.IsTrue( engineService is PrevalenceEngine );

			// Lookup for the system
			object system = kernel["systemid"];
			Assert.IsNotNull( system );
			Assert.IsTrue( system is UserDatabase );
		}

		private IKernel CreateConfiguredKernel()
		{
			IKernel kernel = new DefaultKernel();

			MutableConfiguration confignode = new MutableConfiguration("facility");
	
			IConfiguration engines = 
				confignode.Children.Add( new MutableConfiguration("engines") );
	
			IConfiguration engine = 
				engines.Children.Add( new MutableConfiguration("engine") );
	
			engine.Attributes["id"] = "engineid";
			engine.Attributes["systemId"] = "systemid";
			engine.Attributes["systemType"] = typeof(UserDatabase).AssemblyQualifiedName;
			engine.Attributes["storageDir"] = _storageDir;

			kernel.ConfigurationStore.AddFacilityConfiguration( "prevalence", confignode );
			
			return kernel;
		}

		[Test]
		public void TestWithSnapshot()
		{
			IKernel kernel = CreateConfiguredSnapshotKernel();
			kernel.AddFacility( "prevalence", new PrevalenceFacility() );
		
			// Lookup for the engine
			object engineService = kernel["engineid"];
			Assert.IsNotNull( engineService );
			Assert.IsTrue( engineService is PrevalenceEngine );

			// Lookup for the system
			object system = kernel["systemid"];
			Assert.IsNotNull( system );
			Assert.IsTrue( system is UserDatabase );

			// Lookup for SnapshotTaker
			object snapshotTaker = kernel[PrevalenceFacility.SnapShotTakerComponentPropertyKey];
			Assert.IsNotNull( snapshotTaker );
			Assert.IsTrue( snapshotTaker is SnapshotTaker );

			//Cleanup Policy
			object policy = kernel[PrevalenceFacility.CleanupPolicyComponentPropertyKey];
			Assert.IsNotNull( policy );
			Assert.IsTrue( policy is ICleanUpPolicy );

			((UserDatabase)system).Init();;

			kernel.Dispose();

			bool snapshoted = false;

			foreach(string file in Directory.GetFiles(_storageDir))
			{
				if (Path.GetExtension(file).Equals(".snapshot"))
				{
					snapshoted = true;
				}
			}

			Assert.IsTrue(snapshoted);
		}

		private IKernel CreateConfiguredSnapshotKernel()
		{
			IKernel kernel = new DefaultKernel();

			MutableConfiguration confignode = new MutableConfiguration("facility");
	
			IConfiguration engines = 
				confignode.Children.Add( new MutableConfiguration("engines") );
	
			IConfiguration engine = 
				engines.Children.Add( new MutableConfiguration("engine") );
	
			engine.Attributes["id"] = "engineid";
			engine.Attributes["systemId"] = "systemid";
			engine.Attributes["systemType"] = typeof(UserDatabase).AssemblyQualifiedName;
			engine.Attributes["storageDir"] = _storageDir;

			engine.Attributes["snapshotIntervalInHours"] = "1";
			engine.Attributes["cleanupPolicyComponent"] = null;

			kernel.ConfigurationStore.AddFacilityConfiguration( "prevalence", confignode );
			
			return kernel;
		}
	}
}
