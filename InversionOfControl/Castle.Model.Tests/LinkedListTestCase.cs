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
		public void RemoveBoundary1()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );

			list.Add( "2" );

			list.Add( "3" );

			list.Remove( "1" );

			Assert.AreEqual( "2", list.Head );
			Assert.AreEqual( 2, list.Count );

			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "2,3", String.Join(",", array) );
		}

		[Test]
		public void RemoveBoundary2()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );

			list.Add( "2" );

			list.Add( "3" );

			list.Remove( "3" );

			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 2, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "1,2", String.Join(",", array) );
		}

		[Test]
		public void RemoveBoundary3()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );

			list.Remove( "2" );

			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 1, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "1", String.Join(",", array) );
		}

		[Test]
		public void RemoveMiddle1()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );

			list.Add( "2" );

			list.Add( "3" );

			list.Remove( "2" );

			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 2, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "1,3", String.Join(",", array) );
		}

		[Test]
		public void RemoveMiddle2()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Add( "4" );
			list.Add( "5" );

			list.Remove( "3" );

			Assert.AreEqual( "1", list.Head );
			Assert.AreEqual( 4, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "1,2,4,5", String.Join(",", array) );
		}

		[Test]
		public void IndexOf1()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Add( "4" );
			list.Add( "5" );

			Assert.AreEqual( 0, list.IndexOf("1") );
			Assert.AreEqual( -1, list.IndexOf("10") );
		}

		[Test]
		public void IndexOf2()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Add( "4" );
			list.Add( "5" );

			Assert.AreEqual( 4, list.IndexOf("5") );
			Assert.AreEqual( -1, list.IndexOf("10") );
		}

		[Test]
		public void Insert0()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Insert(0, "x");

			Assert.AreEqual( 4, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "x,1,2,3", String.Join(",", array) );
		}

		[Test]
		public void Insert1()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Insert(1, "x");

			Assert.AreEqual( 4, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "1,x,2,3", String.Join(",", array) );
		}

		[Test]
		public void Insert2()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Insert(2, "x");

			Assert.AreEqual( 4, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "1,2,x,3", String.Join(",", array) );
		}


		[Test]
		public void Insert2bis()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "0" );
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			list.Add( "4" );
			list.Add( "5" );
			list.Insert(2, "x");

			Assert.AreEqual( 7, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "0,1,x,2,3,4,5", String.Join(",", array) );
		}


		[Test]
		public void Replace1()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "0" );
			list.Add( "1" );
			Assert.IsTrue( list.Replace("0", "x") );

			Assert.AreEqual( 2, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "x,1", String.Join(",", array) );
		}

		[Test]
		public void Replace2()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "0" );
			list.Add( "1" );
			Assert.IsTrue( list.Replace("1", "x") ); 

			Assert.AreEqual( 2, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "0,x", String.Join(",", array) );
		}

		[Test]
		public void Replace3()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "0" );
			list.Add( "1" );
			Assert.IsFalse( list.Replace("11", "x") ); 

			Assert.AreEqual( 2, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "0,1", String.Join(",", array) );
		}

		[Test]
		public void Replace4()
		{
			LinkedList list = new LinkedList();
			
			list.Add( "0" );
			list.Add( "1" );
			list.Add( "2" );
			list.Add( "3" );
			Assert.IsTrue( list.Replace("2", "x") ); 

			Assert.AreEqual( 4, list.Count );
			
			String[] array = (String[]) list.ToArray( typeof(String) );
			Assert.AreEqual( "0,1,x,3", String.Join(",", array) );
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
