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

namespace Castle.ActiveRecord.Tests.Model
{
	using System;
	using System.Collections;

	[ActiveRecord(DiscriminatorValue="firm")]
	public class Firm : Company
	{
		private IList _clients;

		public Firm()
		{
		}

		public Firm(string name) : base(name)
		{
		}

		[HasMany( typeof(Client), RelationType.Bag, Column="client_of" )]
		public IList Clients
		{
			get { return _clients; }
			set { _clients = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Firm) );
		}

		public static Firm[] FindAll()
		{
			return (Firm[]) ActiveRecordBase.FindAll( typeof(Firm) );
		}

		public static Firm Find(int id)
		{
			return (Firm) ActiveRecordBase.FindByPrimaryKey( typeof(Firm), id );
		}
	}
}
