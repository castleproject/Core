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

namespace Castle.Applications.MindDump.Services
{
	using System;

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Model;


	public class BlogMaintenanceService
	{
		private BlogDao _blogDao;
		private PostDao _postDao;

		public BlogMaintenanceService(BlogDao blogDao, PostDao postDao)
		{
			_blogDao = blogDao;
			_postDao = postDao;
		}

		public Post CreateNewEntry(int blogId, Post post)
		{
			post.Blog = new Blog(blogId);
			
			try
			{
				return _postDao.Create(post);
			}
			catch(Exception ex)
			{
				throw new ApplicationException("Could not create entry", ex);
			}
		}

		public void UpdateEntry(int blogId, int postId, Post post)
		{
			post.Id = postId;
			post.Blog = new Blog(blogId);
			
			try
			{
				_postDao.Update(post);
			}
			catch(Exception ex)
			{
				throw new ApplicationException("Could not update entry", ex);
			}
		}
	}
}
