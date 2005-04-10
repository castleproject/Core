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

namespace Extending2.Dao
{
	using System;

	/// <summary>
	/// Note that Post is not a component.
	/// 
	/// With a different design the Post could be made into
	/// a component, but we would use a factory pattern to keep things tidy.
	/// The factory would internally invoke the container, but for our application
	/// perspective, its just an ordinary factory.
	/// </summary>
	public class Post
	{
		private int _id;
		private Blog _blog;
		private String _title;
		private String _contents;

		public Post()
		{
		}

		public Post(Blog blog, string title, string contents)
		{
			_blog = blog;
			_title = title;
			_contents = contents;
		}

		public Post(int id, Blog blog, string title, string contents) : this(blog, title, contents)
		{
			_id = id;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public Blog Blog
		{
			get { return _blog; }
			set { _blog = value; }
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
	}
}
