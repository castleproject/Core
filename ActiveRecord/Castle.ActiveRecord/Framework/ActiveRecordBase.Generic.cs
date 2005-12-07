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

#if dotNet2

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections.Generic;

	using NHibernate;
	using NHibernate.Expression;

	public class ActiveRecordBase<T> where T : class
	{
		#region instance methods

		public virtual void Save()
		{
			ActiveRecordBase.Save( this );
		}

		public virtual void Create()
		{
			ActiveRecordBase.Create( this );
		}

		public virtual void Update()
		{
			ActiveRecordBase.Update( this );
		}

		public virtual void Delete()
		{
			ActiveRecordBase.Delete( this );
		}

		public virtual object Execute( NHibernateDelegate call )
		{
			return ActiveRecordBase.Execute( typeof( T ), call, this );
		} 

		#endregion

		#region static methods copied from Mediator

		public static T Find( object id )
		{
			return (T)ActiveRecordBase.FindByPrimaryKey( typeof( T ), id, true );
		}

		public static T TryFind( object id )
		{
			return (T)ActiveRecordBase.FindByPrimaryKey( typeof( T ), id, false );
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof( T ) );
		}

		public static T[] FindAll()
		{
			return (T[])ActiveRecordBase.FindAll( typeof( T ) );
		}

		public static T[] FindAll( params ICriterion[] criterias )
		{
			return (T[])ActiveRecordBase.FindAll( typeof( T ), criterias );
		}

		public static T[] FindAll( ICriterion criteria, int firstResult, int maxresults )
		{
			return FindAll( new ICriterion[] { criteria }, firstResult, maxresults );
		}

		public static T[] FindAll( ICriterion[] criterias, int firstResult, int maxresults )
		{
			return (T[])ActiveRecordBase.SlicedFindAll( typeof( T ), firstResult, maxresults, criterias );
		}

		public static T[] FindAll( ICriterion[] criterias, Order[] orders )
		{
			return (T[])ActiveRecordBase.FindAll( typeof( T ), orders, criterias );
		}

		public static T[] FindAll( ICriterion[] criterias, Order[] orders, int firstResult, int maxresults )
		{
			return (T[])ActiveRecordBase.SlicedFindAll( typeof( T ), firstResult, maxresults, orders, criterias );
		}

		//This may return more than one result, and more than one type,
		//so this is here just to complement the non-generic version, instead of
		//enhancing it, like the rest of the methods here.
		public static object ExecuteQuery( IActiveRecordQuery q )
		{
			return ActiveRecordBase.ExecuteQuery( q );
		}

		#endregion
	}
}
#endif