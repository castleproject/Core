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
	public class PostTestCase : BaseMindDumpTestCase
	{
		[Test]
		public void Create()
		{
			ResetDatabase();

			AuthorDao authorDao = (AuthorDao) Container[ typeof(AuthorDao) ];
			BlogDao blogDao = (BlogDao) Container[ typeof(BlogDao) ];
			PostDao postDao = (PostDao) Container[ typeof(PostDao) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog = new Blog("hammett's blog", "my thoughts.. ugh!", "default", author);
			
			Post post = new Post("My first entry", "This is my first entry", DateTime.Now);
			post.Blog = blog;

			authorDao.Create( author );
			blogDao.Create( blog );
			postDao.Create( post );

			IList posts = postDao.Find();
			Assert.AreEqual( 1, posts.Count );

			Post comparisson = (Post) posts[0];
			Assert.AreEqual( post.Title, comparisson.Title );
			Assert.AreEqual( post.Contents, comparisson.Contents );
		}

		[Test]
		public void FindAll()
		{
			ResetDatabase();

			AuthorDao authorDao = (AuthorDao) Container[ typeof(AuthorDao) ];
			BlogDao blogDao = (BlogDao) Container[ typeof(BlogDao) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog = new Blog("hammett's blog", "my thoughts.. ugh!", "default", author);

			authorDao.Create( author );
			blogDao.Create( blog );

			Post post1 = new Post("My first entry", "This is my first entry", DateTime.Now);
			post1.Blog = blog;
			Post post2 = new Post("My second entry", "This is my second entry", DateTime.Now);
			post2.Blog = blog;

			PostDao postDao = (PostDao) Container[ typeof(PostDao) ];
			postDao.Create(post1);
			postDao.Create(post2);

			IList posts = postDao.Find();
			Assert.AreEqual( 2, posts.Count );
		}
	}
}
