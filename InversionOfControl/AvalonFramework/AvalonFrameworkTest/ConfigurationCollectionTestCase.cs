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
	using NUnit.Framework;
	using Apache.Avalon.Framework;

	[TestFixture]
	public class ConfigurationCollectionTest
	{
		private IConfiguration[] arrayRange = new IConfiguration[] {
			new DefaultConfiguration("array", "ConfigurationCollectionTest" ),
			new DefaultConfiguration("array", "ConfigurationCollectionTest" ),
			new DefaultConfiguration("array", "ConfigurationCollectionTest" )
		};

		private ConfigurationCollection collRange;

		public ConfigurationCollectionTest()
		{
			collRange = new ConfigurationCollection();
			collRange.Add( new DefaultConfiguration("collection", "ConfigurationCollectionTest") );
			collRange.Add( new DefaultConfiguration("collection", "ConfigurationCollectionTest") );
			collRange.Add( new DefaultConfiguration("collection", "ConfigurationCollectionTest") );
		}

		[Test] public void Constructors()
		{
			ConfigurationCollection collection = new ConfigurationCollection();
			Assertion.AssertEquals( 0, collection.Count );

			collection = new ConfigurationCollection( collRange );
			Assertion.AssertEquals( 3, collection.Count );
			foreach( IConfiguration config in collection )
			{
				Assertion.AssertEquals( "collection", config.Name );
				Assertion.AssertEquals( "ConfigurationCollectionTest", config.Location );
			}

			collection = new ConfigurationCollection( arrayRange );
			Assertion.AssertEquals( 3, collection.Count );
			foreach( IConfiguration config in collection )
			{
				Assertion.AssertEquals( "array", config.Name );
				Assertion.AssertEquals( "ConfigurationCollectionTest", config.Location );
			}
		}

		[Test] public void Index()
		{
			ConfigurationCollection collection = new ConfigurationCollection( arrayRange );
			DefaultConfiguration testconfig = new DefaultConfiguration( "test", "ConfigurationCollectionTest" );
			testconfig.Value = "1";
			collection.Add( testconfig );

			Assertion.AssertEquals( 4, collection.Count );
			IConfiguration config = collection[3]; // 0 based indexes

			Assertion.AssertEquals( "test", config.Name );
			Assertion.AssertEquals( "ConfigurationCollectionTest" , config.Location );

			Assertion.Assert( ! ("1" == collection[0].Value) );
			Assertion.AssertEquals( "1", collection[3].Value );
		}

		[Test] public void Add()
		{
			ConfigurationCollection collection = new ConfigurationCollection();
			collection.Add( new DefaultConfiguration( "test", "ConfigurationCollectionTest" ) );
			Assertion.AssertEquals( 1, collection.Count );
			Assertion.AssertEquals( "test", collection[0].Name );
			Assertion.AssertEquals( "ConfigurationCollectionTest" , collection[0].Location );

			collection.AddRange( arrayRange );
			Assertion.AssertEquals( 4, collection.Count );

			collection.AddRange( collRange );
			Assertion.AssertEquals( 7, collection.Count );

			int place = 0;
			foreach( IConfiguration config in collection )
			{
				Assertion.AssertEquals( "ConfigurationCollectionTest", config.Location );
				switch (place)
				{
					case 0:
						Assertion.AssertEquals( "test", config.Name );
						break;

					case 1:
					case 2:
					case 3:
						Assertion.AssertEquals( "array", config.Name );
						break;

					case 4:
					case 5:
					case 6:
						Assertion.AssertEquals( "collection", config.Name );
						break;
				}
				place++;
			}
		}

		[Test] public void CopyTo()
		{
			ConfigurationCollection collection = new ConfigurationCollection( collRange );
			
			IConfiguration[] array = new IConfiguration[4];
			array[0] = new DefaultConfiguration( "test", "ConfigurationCollectionTest" );

			collection.CopyTo( array, 1 );

			bool isFirst = true;
			foreach ( IConfiguration config in array )
			{
				if (isFirst)
				{
					Assertion.AssertEquals("test", config.Name);
					isFirst = false;
				}
				else
				{
					Assertion.AssertEquals("collection", config.Name);
				}

				Assertion.AssertEquals("ConfigurationCollectionTest", config.Location);
			}
		}

		[Test] public void Contains()
		{
			ConfigurationCollection collection = new ConfigurationCollection( arrayRange );

			foreach ( IConfiguration config in arrayRange )
			{
				Assertion.AssertEquals( true, collection.Contains( config ) );
			}

			foreach ( IConfiguration config in collRange )
			{
				Assertion.AssertEquals( false, collection.Contains( config ) );
			}
		}

		[Test] public void IndexOf()
		{
			ConfigurationCollection collection = new ConfigurationCollection( arrayRange );
			Assertion.AssertEquals( 0, collection.IndexOf( arrayRange[0] ) );
			Assertion.AssertEquals( 2, collection.IndexOf( arrayRange[2] ) );
		}

		[Test] public void InsertRemove()
		{
			ConfigurationCollection collection = new ConfigurationCollection( arrayRange );
			IConfiguration config = new DefaultConfiguration( "test", "ConfigurationCollectionTest" );

			collection.Insert( 1, config );
			Assertion.Assert( collection.Contains( config ) );
			Assertion.AssertEquals( config, collection[1] );
			Assertion.AssertEquals( 4, collection.Count );

			collection.Remove( config );
			Assertion.AssertEquals( 3, collection.Count );
			Assertion.AssertEquals( false, collection.Contains( config ) );
		}
	}
}
