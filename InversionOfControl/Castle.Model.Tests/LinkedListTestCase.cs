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

namespace Castle.Model.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Model.Internal;

	[TestFixture]
	public class LinkedListTestCase
	{

		[Test]
		public void Enumerable()
		{
			LinkedList list = new LinkedList();
			
			list.AddFirst( "third" );
			list.AddFirst( "second" );
			list.AddFirst( "first" );

			int index = 0;

			foreach(String value in list)
			{
				switch(index++)
				{
					case 0:
						Assert.AreEqual( "first", value );
						break;
					case 1:
						Assert.AreEqual( "second", value );
						break;
					case 2:
						Assert.AreEqual( "third", value );
						break;
				}
			}
		}

		[Test]
		public void AddFirst()
		{
			LinkedList list = new LinkedList();
			
			list.AddFirst( "third" );
			Assert.AreEqual( "third", list.Head );
			Assert.AreEqual( 1, list.Count );

			list.AddFirst( "second" );
			Assert.AreEqual( "second", list.Head );
			Assert.AreEqual( 2, list.Count );

			list.AddFirst( "first" );
			Assert.AreEqual( "first", list.Head );
			Assert.AreEqual( 3, list.Count );
		}

		[Test]
		public void Add()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 1, list.Count );

			list.Add( "2" );
			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 2, list.Count );

			list.Add( "3" );
			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 3, list.Count );
		}

		[Test]
		public void ToArray()
		{
			LinkedList list = new LinkedList();
			
			list.AddFirst( "third" );
			list.AddFirst( "second" );
			list.AddFirst( "first" );

			String[] values = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "first", values[0] );
			Assert.AreEqual( "second", values[1] );
			Assert.AreEqual( "third", values[2] );
		}
	}
}
