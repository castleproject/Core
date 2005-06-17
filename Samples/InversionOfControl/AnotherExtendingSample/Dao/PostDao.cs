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

namespace Extending2.Dao
{
	using System;
	using System.Data;
	using System.Collections;

	/// <summary>
	/// This class uses transaction but 
	/// does not use the attributes to specify them. Instead
	/// it relies on the configuration file, so you dont have
	/// to bind your implementations to a specific API
	/// </summary>
	public class PostDao
	{
		private IConnectionFactory _connFactory;
		private BlogDao _blogDao;

		public PostDao(IConnectionFactory connFactory, BlogDao blogDao)
		{
			_connFactory = connFactory;
			_blogDao = blogDao;
		}

		public virtual Post Create(Post post)
		{
			using(IDbConnection conn = _connFactory.CreateConnection())
			{
				IDbCommand command = conn.CreateCommand();

				// Not the best way, but the simplest
				command.CommandText = 
					String.Format("INSERT INTO posts (title, contents, blogid) values ('{0}', '{1}', {2});select @@identity", 
					post.Title, post.Contents, post.Blog.Id);

				object result = command.ExecuteScalar();
				post.Id = Convert.ToInt32(result);
			}

			return post;
		}

		public virtual Post Update(Post post)
		{
			using(IDbConnection conn = _connFactory.CreateConnection())
			{
				IDbCommand command = conn.CreateCommand();

				command.CommandText = 
					String.Format("UPDATE posts set title = '{0}', contents = '{1}' where id = {2}", 
					post.Title, post.Contents, post.Id);

				command.ExecuteNonQuery();
			}

			return post;
		}

		public virtual IList Find(Blog blog)
		{
			// As you see, no transaction involved here

			IList list = new ArrayList();

			using(IDbConnection conn = _connFactory.CreateConnection())
			{
				IDbCommand command = conn.CreateCommand();

				// Not the best way, but the simplest
				command.CommandText = 
					String.Format("select id, title, contents from posts where blogid = {0} " + 
						"order by id", blog.Id);

				using (IDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						Post post = new Post( 
							reader.GetInt32(0), blog, reader.GetString(1), reader.GetString(2) );
						list.Add(post);
					}
				}
			}

			return list;
		}
	}
}
