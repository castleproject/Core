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
	using Apache.Avalon.Framework;
	using NUnit.Framework;

	public abstract class AbstractConfigurationTest
	{
		protected abstract AbstractConfiguration GetConfiguration();

		[Test]
		public void IsReadOnly()
		{
			AbstractConfiguration config = GetConfiguration();
			Assertion.AssertEquals( false, config.IsReadOnly );

			config.MakeReadOnly();

			Assertion.AssertEquals( true, config.IsReadOnly );
		}

		[Test]
		public void Name()
		{
			AbstractConfiguration config = GetConfiguration();

			config.Name = "Name";

			Assertion.AssertEquals( "Name", config.Name );
		}

		[Test]
		public void Location()
		{
			AbstractConfiguration config = GetConfiguration();

			config.Location = "Location";

			Assertion.AssertEquals( "Location", config.Location );
		}

		[Test]
		public void Value()
		{
			AbstractConfiguration config = GetConfiguration();

			config.Value = "Value";

			Assertion.AssertEquals( "Value", config.Value );

			config.Value = "true";

			Assertion.AssertEquals( true, (bool)config.GetValue(typeof( bool ) ) );

			int intValue = (int) config.GetValue(typeof(int), -1);
			Assertion.AssertEquals( -1, intValue );
			
			config.Value = "3";
			intValue = (int) config.GetValue( typeof( int ), -1 );
			Assertion.AssertEquals( 3, intValue );
		}

		[Test]
		public void Namespace()
		{
			AbstractConfiguration config = GetConfiguration();

			config.Namespace = "Namespace";

			Assertion.AssertEquals( "Namespace", config.Namespace );
		}

		[Test]
		public void Prefix()
		{
			AbstractConfiguration config = GetConfiguration();

			config.Prefix = "Prefix";

			Assertion.AssertEquals( "Prefix", config.Prefix );
		}

		[Test]
		public void Children()
		{
			AbstractConfiguration config = GetConfiguration();

			IConfiguration fooBar = config.GetChild("FooBar", false );
			Assertion.AssertNull( fooBar );
			fooBar = config.GetChild("FooBar", true);
			Assertion.AssertNotNull( fooBar );

			Assertion.AssertNotNull( config.Children );

			ConfigurationCollection collection = config.Children;

			for (int i = 0; i < 10; i++)
			{
				AbstractConfiguration child = GetConfiguration();
				child.Name="Child" + i;
				collection.Add( child );
			}

			config.Children = collection;

			Assertion.AssertEquals( 11, config.Children.Count );

			config.Children.Remove( fooBar );
			
			Assertion.AssertEquals( 10, config.Children.Count );

			int x = 0;
			foreach ( AbstractConfiguration child in config.Children )
			{
				Assertion.AssertEquals( "Child" + x, child.Name );
				x++;
			}
		}

		[Test]
		public void Attributes()
		{
			AbstractConfiguration config = GetConfiguration();

			Assertion.AssertEquals( 0, config.Attributes.Count );

			config.Attributes.Add( "Attr1", "Val1" );

			Assertion.AssertEquals( 1, config.Attributes.Count );

			Assertion.AssertEquals( "Val1", config.Attributes[ "Attr1" ] );

			config.Attributes.Add( "ValTest", "true" );
			Assertion.AssertEquals( 2, config.Attributes.Count );

			bool valTest = (bool) config.GetAttribute( "ValTest", typeof ( bool ) );
			Assertion.AssertEquals( true, valTest );

			config.Attributes["ValTest"] = "3";
			Assertion.AssertEquals( 2, config.Attributes.Count );

			int intValTest = (int) config.GetAttribute( "ValTest", typeof( int ), -1 );
			Assertion.AssertEquals( 3, intValTest );

			intValTest = (int) config.GetAttribute( "Attr1", typeof( int ), -1 );
			Assertion.AssertEquals( -1, intValTest );
			
		}

	}
}
