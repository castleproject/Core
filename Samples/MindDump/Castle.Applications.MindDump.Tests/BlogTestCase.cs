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

namespace Castle.Applications.MindDump.Tests
{
	using System;
	using System.Collections;

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Model;

	using NUnit.Framework;


	[TestFixture]
	public class BlogTestCase : BaseMindDumpTestCase
	{
		[Test]
		public void Create()
		{
			ResetDatabase();

			AuthorDao authorDao = (AuthorDao) Container[ typeof(AuthorDao) ];
			BlogDao blogDao = (BlogDao) Container[ typeof(BlogDao) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog = new Blog("hammett's blog", "my thoughts.. ugh!", "default", author);

			authorDao.Create( author );
			blogDao.Create( blog );

			IList blogs = blogDao.Find();
			Assert.AreEqual( 1, blogs.Count );

			Blog comparisson = (Blog) blogs[0];
			Assert.AreEqual( blog.Name, comparisson.Name );
			Assert.AreEqual( blog.Description, comparisson.Description );
			Assert.AreEqual( blog.Theme, comparisson.Theme );
		}

		[Test]
		public void FindAll()
		{
			ResetDatabase();

			AuthorDao authorDao = (AuthorDao) Container[ typeof(AuthorDao) ];
			BlogDao blogDao = (BlogDao) Container[ typeof(BlogDao) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog1 = new Blog("hammett's blog", "my thoughts.. ugh!", "default", author);
			Blog blog2 = new Blog("hammett's personal blog", "more thoughts.. ugh!", "default", author);

			authorDao.Create( author );
			blogDao.Create( blog1 );
			blogDao.Create( blog2 );

			IList blogs = blogDao.Find();
			Assert.AreEqual( 2, blogs.Count );
		}
	}
}
