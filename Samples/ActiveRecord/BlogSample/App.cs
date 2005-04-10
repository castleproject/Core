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

namespace BlogSample
{
	using System;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;


	public class App
	{
		public App()
		{
		}

		public static void Main()
		{
			ActiveRecordStarter.Initialize( 
				new XmlConfigurationSource("../appconfig.xml"), 
				typeof(Blog), typeof(Post) );

			// Common usage

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog("somename");
			blog.Author = "hammett";
			blog.Save();

			Post post = new Post(blog, "title", "contents", "castle");
			post.Save();

			Post.DeleteAll();
			Blog.DeleteAll();

			// Using Session Scope

			using(new SessionScope())
			{
				blog = new Blog("somename");
				blog.Author = "hammett";
				blog.Save();

				post = new Post(blog, "title", "contents", "castle");
				post.Save();
			}

			// Using transaction scope

			Post.DeleteAll();
			Blog.DeleteAll();

			using(TransactionScope transaction = new TransactionScope())
			{
				blog = new Blog("somename");
				blog.Author = "hammett";
				blog.Save();

				post = new Post(blog, "title", "contents", "castle");
				post.Save();

				transaction.VoteRollBack();
			}
		}
	}
}
