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
	public class BlogService
	{
		[Transaction(TransactionMode.Requires)]
		public virtual Blog Create(String name, String author)
		{
			Blog blog = new Blog();
			blog.Name = name;
			blog.Author = author;
			blog.Save();
			return blog;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual Blog CreateAndThrowException(String name, String author)
		{
			Blog blog = new Blog();
			blog.Name = name;
			blog.Author = author;
			blog.Save();

			throw new Exception("Doh!");
		}
		
		[Transaction(TransactionMode.Requires)]
		public virtual Blog CreateAndThrowException2(String name, String author)
		{
			Create(name, author);

			throw new Exception("Doh!");
		}

		[Transaction(TransactionMode.Requires)]
		public virtual Blog CreateAndThrowException3(String name, String author)
		{
			Create(name, author);

			Blog.FindAll(); // will it flush?

			throw new Exception("Doh!");
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void ModifyAndThrowException(Blog blog, String newName)
		{
			blog.Name = newName;
			
			blog.Save();

			throw new Exception("Doh!");
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void ModifyAndThrowException2(Blog blog, String newName)
		{
			blog.Name = newName;
			
			throw new Exception("Doh!");
		}
	}
}
