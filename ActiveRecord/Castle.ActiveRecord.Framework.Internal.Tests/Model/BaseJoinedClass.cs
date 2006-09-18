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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	
#if DOTNET2

	[ActiveRecord("disctable"), JoinedBase]
	public abstract class BaseJoinedClass : ActiveRecordValidationBase<GenClassJoinedSubClassParent>
	{
		private int id;
		private string name;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
	}

	[ActiveRecord("disctablea")]
	public class SubClassJoinedClass : BaseJoinedClass
	{
		private int aid;
		private int age;

		[JoinedKey]
		public int AId
		{
			get { return aid; }
			set { aid = value; }
		}

		[Property]
		public int Age
		{
			get { return age; }
			set { age = value; }
		}
	}
	
#endif
}
