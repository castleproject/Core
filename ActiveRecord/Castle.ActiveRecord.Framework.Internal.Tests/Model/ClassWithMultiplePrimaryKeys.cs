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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	[ActiveRecord]
	public class ClassWithMultiplePrimaryKeys : ActiveRecordBase
	{
		private int id1;
		private int id2;
		
		[PrimaryKey]
		public int Id1
		{
			get { return id1; }
			set { id1 = value; }
		}

		[PrimaryKey]
		public int Id2
		{
			get { return id2; }
			set { id2 = value; }
		}
	}
}
