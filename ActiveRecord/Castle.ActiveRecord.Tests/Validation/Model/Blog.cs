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

namespace Castle.ActiveRecord.Tests.Validation.Model
{
	using System;

	[ActiveRecord("Blogs2")]
	public class Blog2 : ActiveRecordValidationBase
	{
		private int _id;
		private String _name;
		private String _author;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property, ValidateIsUnique]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[Property]
		public String Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll( typeof(Blog2) );
		}

		public static Blog2[] FindAll()
		{
			return (Blog2[]) FindAll( typeof(Blog2) );
		}

		public static Blog2 Find(int id)
		{
			return (Blog2) FindByPrimaryKey( typeof(Blog2), id );
		}
	}
	
	[ActiveRecord("Blogs2")]
	public class Blog2B : ActiveRecordValidationBase
	{
		private int _id;
		private String _name;
		private String _author;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[Property]
		public String Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll( typeof(Blog2) );
		}

		public static Blog2[] FindAll()
		{
			return (Blog2[]) FindAll( typeof(Blog2) );
		}

		public static Blog2 Find(int id)
		{
			return (Blog2) FindByPrimaryKey( typeof(Blog2), id );
		}
	}

	[ActiveRecord("Blogs3")]
	public class Blog3 : ActiveRecordValidationBase
	{
		private String _id;
		private String _name;
		private String _author;

		[PrimaryKey(PrimaryKeyType.Assigned)]
		[ValidateIsUnique("The ID you specified already exists.")]
		public String Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[Property]
		public String Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll( typeof(Blog3) );
		}

		public static Blog3[] FindAll()
		{
			return (Blog3[]) FindAll( typeof(Blog3) );
		}

		public static Blog3 Find(int id)
		{
			return (Blog3) FindByPrimaryKey( typeof(Blog3), id );
		}
	}

	[ActiveRecord("Blogs4")]
	public class Blog4 : ActiveRecordValidationBase
	{
		private int? _id;
		private String _name;
		private String _author;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int? Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property, ValidateIsUnique]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[Property]
		public String Author
		{
			get { return _author; }
			set { _author = value; }
		}

		public static void DeleteAll()
		{
			DeleteAll(typeof(Blog4));
		}

		public static Blog4[] FindAll()
		{
			return (Blog4[])FindAll(typeof(Blog4));
		}

		public static Blog4 Find(int id)
		{
			return (Blog4)FindByPrimaryKey(typeof(Blog4), id);
		}
	}

	[ActiveRecord("Blogs2")]
	public class Blog5 : ActiveRecordBase
	{
		private int _id;
		private String _name;
		private String _author;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[Property]
		public String Author
		{
			get { return _author; }
			set { _author = value; }
		}

	}
}
