// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

	/// <summary>
	/// Summary description for Post.
	/// </summary>
	public class Post
	{
		private long _id;
		private String _title;
		private String _contents;
		private Author _author;

		public Post()
		{
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

		public Author Author
		{
			get { return _author; }
			set { _author = value; }
		}
	}
}
