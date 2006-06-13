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
#if DOTNET2
namespace Castle.ActiveRecord.Tests.Model.GenericModel
{
	using System;

	using Nullables;

	[ActiveRecord]
	public class GenericNullableModel : ActiveRecordBase<GenericNullableModel>
	{
		private int id;
        private Int32? age;
		private DateTime? completion;
		private Boolean? accepted;

        public GenericNullableModel()
		{
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public Int32? Age
		{
			get { return age; }
			set { age = value; }
		}

		[Property]
        public DateTime? Completion
		{
			get { return completion; }
			set { completion = value; }
		}

		[Property]
        public Boolean? Accepted
		{
			get { return accepted; }
			set { accepted = value; }
		}

	}
}
#endif