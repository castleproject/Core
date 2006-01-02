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

namespace Castle.Facilities.NHibernateIntegration.Tests.Wiring
{
	using System;

	using NHibernate;
	using NHibernate.Cfg;


	public class MyDao
	{
		private readonly ISessionFactory sessionFactory;
		private readonly Configuration cfg;

		public MyDao(ISessionFactory sessionFactory, Configuration cfg)
		{
			this.sessionFactory = sessionFactory;
			this.cfg = cfg;
		}

		public ISessionFactory SessionFactory
		{
			get { return sessionFactory; }
		}

		public Configuration Cfg
		{
			get { return cfg; }
		}
	}
}
