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

namespace Castle.Facilities.ActiveRecord
{
	using System;


	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class PrimaryKeyAttribute : Attribute
	{
		private PrimaryKeyType _generator = PrimaryKeyType.Native;
		private string _column;
		private string _unsavedValue;
		private string _access;

		public PrimaryKeyAttribute()
		{
		}

		public PrimaryKeyType Generator
		{
			get { return _generator; }
			set { _generator = value; }
		}

		public string Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public string UnsavedValue
		{
			get { return _unsavedValue; }
			set { _unsavedValue = value; }
		}

		public string Access
		{
			get { return _access; }
			set { _access = value; }
		}
	}

	public enum PrimaryKeyType
	{
		None,
		Identity,
		Sequence,
		HiLo,
		SeqHiLo,
		UuidHex,
		UuidString,
		Guid,
		GuidComb,
		Native,
		Assigned,
		Foreign
	}
}