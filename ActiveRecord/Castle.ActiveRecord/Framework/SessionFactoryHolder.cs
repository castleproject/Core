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


	public class SessionFactoryHolder
	{
		private Hashtable type2Conf = Hashtable.Synchronized(new Hashtable());
		private Hashtable type2SessFactory = Hashtable.Synchronized(new Hashtable());

		public SessionFactoryHolder()
		{
		}

//		public static ISessionFactory Obtain(Type type)
//		{
//			return null;
//		}

		protected internal void Add(Type rootType, Configuration cfg)
		{
			type2Conf.Add(rootType, cfg);
		}

		protected internal Configuration GetConfiguration(Type type)
		{
			while(type != typeof(object))
			{
				if (type2Conf.ContainsKey(type))
				{
					return (Configuration) type2Conf[type];
				}

				type = type.BaseType;
			}

			return null;
		}

		protected internal ISessionFactory GetSessionFactory(Type type)
		{
			// TODO: Lock?

			if (type2SessFactory.Contains(type))
			{
				return type2SessFactory[type] as ISessionFactory;
			}

			Configuration cfg = GetConfiguration(type);

			ISessionFactory sessFac = cfg.BuildSessionFactory();

			type2SessFactory[type] = sessFac;

			return sessFac;
		}

		public ISession CreateSession(Type type)
		{
			ISessionFactory sessionFactory = GetSessionFactory(type);

			return sessionFactory.OpenSession();
		}

		public void ReleaseSession(ISession session)
		{
			session.Close();
		}
	}
}
