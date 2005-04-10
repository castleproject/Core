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

	[Transactional]
	public class BlogDao
	{
		private IConnectionFactory _connFactory;

		public BlogDao(IConnectionFactory connFactory)
		{
			_connFactory = connFactory;
		}

		[RequiresTransaction]
		public virtual Blog Create(Blog blog)
		{
			using(IDbConnection conn = _connFactory.CreateConnection())
			{
				IDbCommand command = conn.CreateCommand();

				// Not the best way, but the simplest
				command.CommandText = 
					String.Format("INSERT INTO blogs (name, author) values ('{0}', '{1}');select @@identity", 
					blog.Name, blog.Author);

				object result = command.ExecuteScalar();
				blog.Id = Convert.ToInt32(result);
			}

			return blog;
		}

		[RequiresTransaction]
		public virtual void Delete(String name)
		{
			// We pretend to delete the blog here
		}

		public virtual Blog Find(int id)
		{
			using(IDbConnection conn = _connFactory.CreateConnection())
			{
				IDbCommand command = conn.CreateCommand();

				command.CommandText = 
					String.Format("Select id, name, author from blogs where id={0} order by id", id);

				using (IDataReader reader = command.ExecuteReader())
				{
					if(reader.Read())
					{
						return new Blog( 
							reader.GetInt32(0), reader.GetString(1), reader.GetString(2) );
					}
					else
					{
						throw new ApplicationException("Blog not found");
					}
				}
			}
		}

		public virtual IList Find()
		{
			// As you see, no transaction involved here

			IList list = new ArrayList();

			using(IDbConnection conn = _connFactory.CreateConnection())
			{
				IDbCommand command = conn.CreateCommand();

				// Not the best way, but the simplest
				command.CommandText = "Select id, name, author from blogs";

				using (IDataReader reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						Blog blog = new Blog( 
							reader.GetInt32(0), reader.GetString(1), reader.GetString(2) );
						list.Add(blog);
					}
				}
			}

			return list;
		}
	}
}
