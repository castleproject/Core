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

namespace Castle.Applications.MindDump.Tests
{
	using System;
	using System.Collections;

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Model;

	using NUnit.Framework;


	[TestFixture]
	public class AuthorTestCase : BaseMindDumpTestCase
	{
		[Test]
		public void Create()
		{
			ResetDatabase();

			AuthorDao dao = (AuthorDao) Container[ typeof(AuthorDao) ];
			Assert.AreEqual( 0, dao.Find().Count );

			Author author = new Author("hamilton verissimo", "hammett", "mypass");

			dao.Create( author );

			IList authors = dao.Find();
			Assert.AreEqual( 1, authors.Count );

			Author comparisson = (Author) authors[0];
			Assert.AreEqual( author.Name, comparisson.Name );
			Assert.AreEqual( author.Login, comparisson.Login );
			Assert.AreEqual( author.Password, comparisson.Password );
		}

		[Test]
		public void FindAll()
		{
			ResetDatabase();

			AuthorDao dao = (AuthorDao) Container[ typeof(AuthorDao) ];

			Author author1 = new Author("hamilton verissimo", "hammett", "mypass");
			Author author2 = new Author("john turturro", "turturro", "mypass");

			dao.Create( author1 );
			dao.Create( author2 );

			IList authors = dao.Find();
			Assert.AreEqual( 2, authors.Count );
		}

		[Test]
		public void Find()
		{
			ResetDatabase();

			AuthorDao dao = (AuthorDao) Container[ typeof(AuthorDao) ];

			Author author1 = new Author("hamilton verissimo", "hammett", "mypass");
			Author author2 = new Author("john turturro", "turturro", "mypass");

			dao.Create( author1 );
			dao.Create( author2 );

			Author found = dao.Find("hammett");
			Assert.IsNotNull(found);
			Assert.AreEqual( author1.Name, found.Name );
			Assert.AreEqual( author1.Login, found.Login );
			Assert.AreEqual( author1.Password, found.Password );

			found = dao.Find("turturro");
			Assert.IsNotNull(found);
			Assert.AreEqual( author2.Name, found.Name );
			Assert.AreEqual( author2.Login, found.Login );
			Assert.AreEqual( author2.Password, found.Password );
		}
	}
}
