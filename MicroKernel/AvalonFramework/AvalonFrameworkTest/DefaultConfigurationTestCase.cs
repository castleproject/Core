// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework.Test
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization;
	using Apache.Avalon.Framework;
	using NUnit.Framework;
	
	[TestFixture]
	public class DefaultConfigurationTest: AbstractConfigurationTest
	{
		protected override AbstractConfiguration GetConfiguration()
		{
			return new DefaultConfiguration("Name", "Location", "Namespace", "Prefix");
		}

		[Test]
		public void DefaultChildren()
		{
			IConfiguration config = GetConfiguration();

			IConfiguration child = config.GetChild("Child", false);
			Assertion.AssertNull( child );

			child = config.GetChild("Child", true);
			Assertion.AssertNotNull( child );

			for( int i = 0; i < 10; i++ )
			{
				DefaultConfiguration testChild = new DefaultConfiguration( "test", "Apache.AvalonFramework.DefaultConfigurationTest" );
				testChild.Value = "value";
				config.Children.Add( testChild );
			}

			Assertion.AssertEquals( 11, config.Children.Count );

			ConfigurationCollection coll = config.GetChildren( "test" );
			Assertion.AssertEquals( 10, coll.Count );

			foreach( IConfiguration testConfig in coll )
			{
				Assertion.AssertEquals( "test", testConfig.Name );
				Assertion.AssertEquals( "value", testConfig.Value );
			}
		}
	}
}
