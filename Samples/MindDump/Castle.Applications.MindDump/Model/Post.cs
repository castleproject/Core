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

namespace Castle.Applications.MindDump.Model
{
	using System;

	public class Post
	{
		private long _id;
		private String _title;
		private String _contents;
		private DateTime _date;
		private Blog _blog;

		public Post()
		{
		}

		public Post(string title, string contents, DateTime date) : this(title, contents)
		{
			_date = date;
		}

		public Post(string title, string contents)
		{
			_title = title;
			_contents = contents;
		}

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Title
		{
			get { return _title; }
			set { _title = value; }
		}

		public string Contents
		{
			get { return _contents; }
			set { _contents = value; }
		}

		public Blog Blog
		{
			get { return _blog; }
			set { _blog = value; }
		}

		public DateTime Date
		{
			get { return _date; }
			set { _date = value; }
		}
	}
}
