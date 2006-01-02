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

namespace Extending2.Dao
{
	using System;

	/// <summary>
	/// Note that Blog is not a component
	/// 
	/// With a different design the Blog could be made into
	/// a component, but we would use a factory pattern to keep things tidy.
	/// The factory would internally invoke the container, but for our application
	/// perspective, its just an ordinary factory.
	/// </summary>
	public class Blog
	{
		private int _id;
		private String _name;
		private String _author;

		public Blog()
		{
		}

		public Blog(string name, string author)
		{
			_name = name;
			_author = author;
		}

		public Blog(int id, string name, string description) : 
			this(name, description)
		{
			_id = id;
		}

		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Author
		{
			get { return _author; }
			set { _author = value; }
		}
	}
}
