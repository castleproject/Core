// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace TableHierarchySample
{
	using System;
	using System.Collections.Generic;
	using Castle.ActiveRecord;

	[ActiveRecord("People")]
	public class Person : ActiveRecordBase<Person>
	{
		private int _id;
		private String _name;
		private IList<Company> _companies = new List<Company>();

		[PrimaryKey]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[HasAndBelongsToMany(Table="PeopleCompanies", ColumnRef="company_id", ColumnKey="person_id" )]
		public IList<Company> Companies
		{
			get { return _companies; }
			set { _companies = value; }
		}
	}
}
