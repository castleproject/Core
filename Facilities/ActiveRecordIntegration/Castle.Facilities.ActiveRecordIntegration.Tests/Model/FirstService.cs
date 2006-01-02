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

namespace Castle.Facilities.ActiveRecordIntegration.Tests.Model
{
	using System;

	using Castle.Services.Transaction;


	[Transactional]
	public class FirstService
	{
		private readonly BlogService blogService;
		private readonly PostService postService;

		public FirstService(BlogService blogService, PostService postService)
		{
			this.blogService = blogService;
			this.postService = postService;
		}
	
		[Transaction(TransactionMode.Requires)]
		public virtual void CreateBlogAndPost()
		{
			Blog blog = blogService.Create( "blog", "author" );
			Post post = postService.Create( blog, "title", "contents", "cat" );
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void CreateBlogAndPost2()
		{
			Blog blog = blogService.Create( "blog", "author" );
			Post post = postService.Create( blog, "title", "contents", "cat" );

			throw new Exception("Dohhh!!!");
		}
	}
}
