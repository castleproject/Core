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

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Model;

	using Castle.Services.Transaction;

	[Transactional]
	public class AccountService
	{
		private AuthorDao _authorDao;
		private BlogDao _blogDao;
		private IMindDumpEventPublisher _publisher;

		public AccountService(AuthorDao authorDao, BlogDao blogDao)
		{
			_authorDao = authorDao;
			_blogDao = blogDao;
		}

		public IMindDumpEventPublisher EventPublisher
		{
			get { return _publisher; }
			set { _publisher = value; }
		}

		/// <summary>
		/// This method creates an author and a blog for him,
		/// if something fails, we need to recover both. Thus
		/// we require that this method is transactional.
		/// </summary>
		/// <param name="blog"></param>
		[Transaction(TransactionMode.Requires)]
		public virtual void CreateAccountAndBlog( Blog blog )
		{
			_authorDao.Create( blog.Author );
			_blogDao.Create( blog );

			if (EventPublisher != null) EventPublisher.NotifyBlogAdded( blog );
		}

		public virtual void UpdateAccount( Author author )
		{
			_authorDao.Update( author );
		}

		public virtual void UpdateBlog( Blog blog )
		{
			_blogDao.Update( blog );
		}

		public virtual Author ObtainAuthor( String login )
		{
			return _authorDao.Find(login);
		}
	}
}
