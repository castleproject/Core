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

	using NHibernate;

	using Castle.ActiveRecord.Framework;


	[ActiveRecord("Agents")]
	public class Agent : ActiveRecordBase
	{
		private AgentKey _userKey;
		private Org _org;
		private IList _groups;
		private int _version;

		public Agent()
		{
			_groups = new ArrayList();
			_version = -1;
		}

		public Agent(AgentKey key) : this()
		{
			_userKey = key;
		}

		[CompositeKey]
		public AgentKey Key
		{
			get { return _userKey; }
			set { _userKey = value; }
		}

		public String OrgId
		{
			get { return _userKey.OrgId; }
		}

		public String Name
		{
			get { return _userKey.Name; }
		}

		[Version(UnsavedValue="negative")]
		public int Version
		{
			get{ return _version; }
			set{ _version = value; }
		}

		[BelongsTo("OrgId", Insert=false, Update=false)]
		public Org Org
		{
			get { return _org; }
			set { _org = value; }
		}

		[HasAndBelongsToMany(typeof (Group),
			 Table = "GroupAgents",
			 ColumnRef = "GroupId",
			 CompositeKeyColumnKeys = new String[]{"OrgId","Name"},
			 Lazy = true,
			 Cascade = ManyRelationCascadeEnum.SaveUpdate)]
		public IList Groups
		{
			get { return _groups; }
			set { _groups = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Agent) );
		}

		public static Agent[] FindAll()
		{
			return (Agent[]) ActiveRecordBase.FindAll( typeof(Agent) );
		}

		public static Agent Find(object key)
		{
			return (Agent) ActiveRecordBase.FindByPrimaryKey( typeof(Agent), key );
		}

		/// <summary>
		/// We make it visible only to for test cases' assertions 
		/// </summary>
		public static ISessionFactoryHolder Holder
		{
			get { return ActiveRecordMediator.GetSessionFactoryHolder(); }
		}

		public void CustomAction()
		{
			Execute(new NHibernateDelegate(MyCustomMethod));
		}

		private object MyCustomMethod(ISession session, object userInstance)
		{
			session.Delete(userInstance);
			session.Flush();
			
			return null;
		}
	}
}
