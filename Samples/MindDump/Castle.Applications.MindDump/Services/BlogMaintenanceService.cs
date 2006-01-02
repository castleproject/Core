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

namespace Castle.Applications.MindDump.Services
{
	using System;
	using System.Collections;

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Model;


	public class BlogMaintenanceService
	{
		private PostDao _postDao;
		private BlogDao _blogDao;
		private AuthorDao _authorDao;

		public BlogMaintenanceService(AuthorDao authorDao, BlogDao blogDao, PostDao postDao)
		{
			_authorDao = authorDao;
			_blogDao = blogDao;
			_postDao = postDao;
		}

		public Post CreateNewPost(Blog blog, Post post)
		{
			post.Blog = blog;
			
			try
			{
				return _postDao.Create(post);
			}
			catch(Exception ex)
			{
				throw new ApplicationException("Could not create post", ex);
			}
		}

		public void UpdatePost(Blog blog, long postId, Post post)
		{
			Post originalPost = ObtainPost(blog, postId);

			originalPost.Title = post.Title;
			originalPost.Contents = post.Contents;
			
			try
			{
				_postDao.Update(originalPost);
			}
			catch(Exception ex)
			{
				throw new ApplicationException("Could not update post", ex);
			}
		}

		public Post ObtainPost( Blog blog, long postId )
		{
			try
			{
				Post post = _postDao.Find(postId);

				if (post != null)
				{
					if (post.Blog.Id != blog.Id)
					{
						throw new ApplicationException("The post requested belongs " + 
							"to a different blog");
					}
				}

				return post;
			}
			catch(ApplicationException ex)
			{
				throw ex;
			}
			catch(Exception ex)
			{
				throw new ApplicationException("Could not find post", ex);
			}
		}

		public IList ObtainPosts(Blog blog)
		{
			return _postDao.Find(blog);
		}

		public Blog ObtainBlogByAuthorName(string authorName)
		{
			Author author = _authorDao.Find(authorName);
			
			if (author == null)
			{
				return null;
			}

			return author.Blogs[0] as Blog;
		}
	}
}
