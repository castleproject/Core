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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;

	using NHibernate;
	using NHibernate.Cfg;
	using NHibernate.Expression;


	public abstract class ActiveRecordBase
	{
		protected internal static SessionFactoryHolder _holder;

		public static Configuration DefineConfiguration(IDictionary dict)
		{
			Configuration cfg = new Configuration();

			cfg.Properties.Add("hibernate.connection.driver_class", 
				"NHibernate.Driver.SqlClientDriver");
			
			cfg.Properties.Add("hibernate.dialect", 
				"NHibernate.Dialect.MsSql2000Dialect");
			
			cfg.Properties.Add("hibernate.connection.provider", 
				"NHibernate.Connection.DriverConnectionProvider");
			
			cfg.Properties.Add("hibernate.connection.connection_String", 
				"UID=susa;Password=OverKkk;Initial Catalog=test;Data Source=.");

			return cfg;
		}

		public ActiveRecordBase()
		{
		}

		protected internal virtual bool BeforeSave(IDictionary state)
		{
			return false;
		}

		protected static object FindByPrimaryKey(Type targetType, object id)
		{
			ISession sess = _holder.CreateSession( targetType );

			try
			{
				return sess.Load( targetType, id );
			}
			finally
			{
				_holder.ReleaseSession(sess);
			}
		}

		protected static Array FindAll(Type t, Order[] orders, params Expression[] expressions)
		{
			ISession sess = _holder.CreateSession( t );

			ICriteria criteria = sess.CreateCriteria(t);

			if (orders != null)
			{
				foreach( Order order in orders )
				{
					criteria.AddOrder( order );
				}
			}

			IList result = criteria.List();
		
			Array array = Array.CreateInstance(t, result.Count);
	
			int index = 0;

			foreach(object item in result)
			{
				array.SetValue(item, index++);
			}

			_holder.ReleaseSession(sess);

			return array;
		}

		protected static Array FindAll(Type t, params Expression[] expressions)
		{
			ISession sess = _holder.CreateSession( t );

			ICriteria criteria = sess.CreateCriteria(t);

			foreach( Expression ex in expressions )
			{
				criteria.Add( ex );
			}

			IList result = criteria.List();
		
			Array array = Array.CreateInstance(t, result.Count);
	
			int index = 0;

			foreach(object item in result)
			{
				array.SetValue(item, index++);
			}

			_holder.ReleaseSession(sess);

			return array;
		}

		public virtual void Save()
		{
			Save(this);
		}

		protected void Save(object obj)
		{
			ISession sess = _holder.CreateSession( obj.GetType() );

			try
			{
				sess.Save(obj);

				sess.Flush();
			}
			catch(Exception ex)
			{
				// Convert to AR Save Exception or something like that

				Console.WriteLine( ex.StackTrace );

				throw ex;
			}
			finally
			{
				_holder.ReleaseSession(sess);
			}
		}

		protected static void DeleteAll(Type type)
		{
			ISession sess = _holder.CreateSession( type );

			try
			{
				sess.Delete( String.Format("from {0}", type.Name) );

				sess.Flush();
			}
			catch(Exception ex)
			{
				// Convert to AR Save Exception or something like that

				Console.WriteLine( ex.StackTrace );

				throw ex;
			}
			finally
			{
				_holder.ReleaseSession(sess);
			}
		}
	}
}


