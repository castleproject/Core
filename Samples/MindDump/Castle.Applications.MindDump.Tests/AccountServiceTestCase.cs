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

	using NUnit.Framework;

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Model;
	using Castle.Applications.MindDump.Services;

	[TestFixture]
	public class AccountServiceTestCase : BaseMindDumpTestCase
	{
		[Test]
		public void CreateAuthorAndBlog()
		{
			ResetDatabase();

			AccountService service = (AccountService) Container[ typeof(AccountService) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog = new Blog("hammett's blog", "Description", "default", author);

			service.CreateAccountAndBlog( blog );

			AuthorDao dao = (AuthorDao) Container[ typeof(AuthorDao) ];
			Assert.AreEqual( 1, dao.Find().Count );
			BlogDao blogdao = (BlogDao) Container[ typeof(BlogDao) ];
			Assert.AreEqual( 1, blogdao.Find().Count );
		}

		[Test]
		public void TestRollback()
		{
			ResetDatabase();

			AccountService service = (AccountService) Container[ typeof(AccountService) ];

			Author author = new Author("this is an incredible big, I mean, huge name for an author, don't you think? If you don't, please state why, cause I have all day to hear your arguments", "hammett", "mypass");
			Blog blog = new Blog("hammett's blog", "Description", "default", author);

			try
			{
				service.CreateAccountAndBlog( blog );
				Assert.Fail("Expected exception");
			}
			catch(Exception)
			{
				// Expected
			}

			AuthorDao dao = (AuthorDao) Container[ typeof(AuthorDao) ];
			Assert.AreEqual( 0, dao.Find().Count );
			BlogDao blogdao = (BlogDao) Container[ typeof(BlogDao) ];
			Assert.AreEqual( 0, blogdao.Find().Count );
		}

		[Test]
		public void TestRollback2()
		{
			ResetDatabase();

			AccountService service = (AccountService) Container[ typeof(AccountService) ];

			Author author = new Author("hamilton verissimo", "hammett", "mypass");
			Blog blog = new Blog("this is an incredible big, I mean, huge name for an author, don't you think? If you don't, please state why, cause I have all day to hear your arguments", "Description", "default", author);

			try
			{
				service.CreateAccountAndBlog( blog );
				Assert.Fail("Expected exception");
			}
			catch(Exception)
			{
				// Expected
			}

			AuthorDao dao = (AuthorDao) Container[ typeof(AuthorDao) ];
			Assert.AreEqual( 0, dao.Find().Count );
			BlogDao blogdao = (BlogDao) Container[ typeof(BlogDao) ];
			Assert.AreEqual( 0, blogdao.Find().Count );
		}
	}
}
