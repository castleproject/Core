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

namespace TestScaffolding.Model
{
	using System;

	using Castle.ActiveRecord;

	[ActiveRecord]
	public class BlogCategory : ActiveRecordBase
	{
		private int id;
		private Blog blog;
		private Category category;

		[PrimaryKey(PrimaryKeyType.Identity)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[BelongsTo("blogid", NotNull=true)]
		public Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		[BelongsTo("categoryid", NotNull=true)]
		public Category Category
		{
			get { return category; }
			set { category = value; }
		}
	}
}
