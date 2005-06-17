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
	using Castle.Applications.MindDump.Services;

	using NUnit.Framework;


	[TestFixture]
	public class BlogMaintenanceServiceTestCase : BaseMindDumpTestCase
	{
		[Test]
		public void CreateNewPostAndObtainPosts()
		{
			ResetDatabase();

			AccountService account = (AccountService) 
				Container[ typeof(AccountService) ];
			BlogService maintenance = (BlogService) 
				Container[ typeof(BlogService) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog = new Blog("hammett's blog", "my thoughts.. ugh!", "default", author);

			account.CreateAccountAndBlog(blog);
			
			Post post = maintenance.CreateNewPost( 
				blog, new Post("title", "contents", DateTime.Now) );

			Post comparisson = maintenance.ObtainPost( blog, post.Id );
			Assert.AreEqual( post.Id, comparisson.Id );
			Assert.AreEqual( post.Title, comparisson.Title );
			Assert.AreEqual( post.Contents, comparisson.Contents );

			IList posts = maintenance.ObtainPosts(blog);
			Assert.AreEqual( 1, posts.Count );
		}
	}
}
