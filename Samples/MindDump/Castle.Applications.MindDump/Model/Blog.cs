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

namespace Castle.Applications.MindDump.Model
{
	using System;
	using System.Collections;

	public class Blog
	{
		private long _id;
		private String _name;
		private String _description;
		private String _theme;
		private Author _author;
		private IList _posts;

		public Blog()
		{
		}

		public Blog(long id)
		{
			_id = id;
		}

		public Blog(String name, String description, String theme, Author author)
		{
			_name = name;
			_description = description;
			_theme = theme;
			_author = author;
		}

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public string Theme
		{
			get { return _theme; }
			set { _theme = value; }
		}

		public Author Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public IList Posts
		{
			get { return _posts; }
			set { _posts = value; }
		}
	}
}
