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

namespace Castle.ActiveRecord.Tests
{
	using System;

	using NUnit.Framework;


	[TestFixture]
	public class ActiveRecordTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void SimpleOperations()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog[] blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 0, blogs.Length );

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			blogs = Blog.FindAll();

			Assert.IsNotNull( blogs );
			Assert.AreEqual( 1, blogs.Length );

			Blog retrieved = blogs[0];
			Assert.IsNotNull( retrieved );

			Assert.AreEqual( blog.Name, retrieved.Name );
			Assert.AreEqual( blog.Author, retrieved.Author );
		}

		[Test]
		public void RelationsOneToMany()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");

			post1.Save();
			post2.Save();

			blog = Blog.Find(blog.Id);

			Assert.IsNotNull(blog);
			Assert.AreEqual(2, blog.Posts.Count);

			foreach(Post post in blog.Posts)
			{
				Assert.AreEqual( blog.Id, post.Blog.Id );
			}
		}

		[Test]
		public void RelationsOneToManyWithWhereAndOrder()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";
			blog.Save();

			Post post1 = new Post(blog, "title1", "contents", "category1");
			Post post2 = new Post(blog, "title2", "contents", "category2");
			Post post3 = new Post(blog, "title3", "contents", "category3");

			post1.Save();
			System.Threading.Thread.Sleep(1000); // Its a smalldatetime (small precision)
			post2.Save();
			System.Threading.Thread.Sleep(1000); // Its a smalldatetime (small precision)
			post3.Published = true;
			post3.Save();

			blog = Blog.Find(blog.Id);

			Assert.IsNotNull(blog);
			Assert.AreEqual(2, blog.UnPublishedPosts.Count);
			Assert.AreEqual(1, blog.PublishedPosts.Count);

			Assert.AreEqual(3, blog.RecentPosts.Count);
			Assert.AreEqual( post3.Id, (blog.RecentPosts[0] as Post).Id );
			Assert.AreEqual( post2.Id, (blog.RecentPosts[1] as Post).Id );
			Assert.AreEqual( post1.Id, (blog.RecentPosts[2] as Post).Id );
		}

		[Test]
		public void FindLoad()
		{
			ActiveRecordStarter.Initialize( GetConfigSource(), typeof(Post), typeof(Blog) );

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = Blog.Find(0);
			Assert.IsNull(blog);
		}
	}
}
