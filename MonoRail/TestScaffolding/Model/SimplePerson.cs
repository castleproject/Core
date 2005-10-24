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

	[ActiveRecord("SimplePerson")]
	public class SimplePerson : ActiveRecordValidationBase
	{
		private int _id;
		private String _name;
		private Int32 _age;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property, ValidateRegExpAttribute(@"^(?:\w| )+$")]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[ Property ]
		public Int32 Age
		{
			get { return _age; }
			set { _age = value; }
		}

		public override string ToString()
		{
			return "[" + _id + ":" + _name + ":" + _age + "]";
		}
		
		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(SimplePerson) );
		}

		public static SimplePerson[] FindAll()
		{
			return (SimplePerson[]) ActiveRecordBase.FindAll( typeof(SimplePerson) );
		}

		public static SimplePerson Find(int id)
		{
			return (SimplePerson) ActiveRecordBase.FindByPrimaryKey( typeof(SimplePerson), id );
		}
	}
}
