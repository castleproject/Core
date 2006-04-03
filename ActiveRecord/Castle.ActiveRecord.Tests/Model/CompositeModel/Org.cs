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

namespace Castle.ActiveRecord.Tests.Model.CompositeModel
{
	using System;
	using System.Collections;

	[ActiveRecord("Orgs")]
	public class Org : ActiveRecordBase
	{
		private string _id;
		private String _description;
		private DateTime _created;
		private IList _agents;
		private int _version;

		public Org()
		{
			_agents = new ArrayList();
			_created = DateTime.Now;
			_version = -1;
		}

		public Org(string id, String description) : this()
		{
			_id = id;
			_description = description;
		}

		[PrimaryKey(PrimaryKeyType.Assigned)]
		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Version(UnsavedValue="negative")]
		public int Version
		{
			get{ return _version; }
			set{ _version = value; }
		}

		[Property(ColumnType="StringClob")]
		public String Contents
		{
			get { return _description; }
			set { _description = value; }
		}

		[HasMany(typeof(Agent),
			 Inverse=true,
			 Lazy=true)]
		public IList Agents
		{
			get { return _agents; }
			set { _agents = value; }
		}

		[Property]
		public DateTime Created
		{
			get { return _created; }
			set { _created = value; }
		}

		protected override bool BeforeSave(IDictionary state)
		{
			state["Created"] = DateTime.Now;
			return true;
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Org) );
		}

		public static Org[] FindAll()
		{
			return (Org[]) ActiveRecordBase.FindAll( typeof(Org) );
		}

		public static Org Find(string id)
		{
			return (Org) ActiveRecordBase.FindByPrimaryKey( typeof(Org), id );
		}

		public static int FetchCount()
		{
			return ActiveRecordBase.CountAll(typeof(Org));
		}

		public static int FetchCount(string filter, params object[] args)
		{
			return ActiveRecordBase.CountAll(typeof(Org), filter, args);
		}

		public void SaveWithException()
		{
			Save();

			throw new ApplicationException("Fake Exception");
		}
	}
}
