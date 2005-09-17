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

namespace MoviesDemo.Model
{
	using System;
	using Castle.ActiveRecord;

	[ActiveRecord("tb_movies")]
	public class Movie : ActiveRecordBase
	{
		private int _id;
		private string _title;
		private string _description;
		private DateTime _added;
		private int _score;

		public Movie()
		{
		}

		[PrimaryKey(PrimaryKeyType.Identity)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public String Title
		{
			get { return _title; }
			set { _title = value; }
		}

		[Property]
		public String Description
		{
			get { return _description; }
			set { _description = value; }
		}

		[Property]
		public DateTime Added
		{
			get { return _added; }
			set { _added = value; }
		}

		[Property]
		public int Score
		{
			get { return _score; }
			set { _score = value; }
		}

		public static Movie[] FindAll()
		{
			return (Movie[]) ActiveRecordBase.FindAll( typeof(Movie) );
		}

		public static Movie Find(int id)
		{
			return (Movie) ActiveRecordBase.FindByPrimaryKey( typeof(Movie), id );
		}
	}
}
