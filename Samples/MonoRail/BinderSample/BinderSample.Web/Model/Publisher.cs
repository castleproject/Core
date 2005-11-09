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

namespace BinderSample.Web.Model
{
	using System;

	using Castle.ActiveRecord;

	using Iesi.Collections;


	[ActiveRecord]
	public class Publisher : ActiveRecordBase
	{
		private int id;
		private String name;
		private ISet books = new HashedSet();

		public Publisher()
		{
		}

		public Publisher(string name)
		{
			this.name = name;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[HasMany( typeof(Book) )]
		public ISet Books
		{
			get { return books; }
			set { books = value; }
		}

		public override string ToString()
		{
			return "" + name;
		}

		public static Publisher Find(int id)
		{
			return (Publisher) FindByPrimaryKey( typeof(Publisher), id );
		}

		public static Publisher[] FindAll()
		{
			return (Publisher[]) FindAll( typeof(Publisher) );
		}
	}
}
