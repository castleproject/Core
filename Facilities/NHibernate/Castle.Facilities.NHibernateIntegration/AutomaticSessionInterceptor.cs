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

using Transaction = Castle.Services.Transaction.ITransaction;

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using Castle.Model.Interceptor;

	using Castle.MicroKernel;

	using Castle.Facilities.NHibernateExtension;

	using Castle.Services.Transaction;

	using NHibernate;


	public class AutomaticSessionInterceptor : IMethodInterceptor
	{
		private IKernel _kernel;

		public AutomaticSessionInterceptor(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			if (SessionManager.CurrentSession != null)
			{
				return invocation.Proceed(args);
			}

			String key = ObtainSessionFactoryKeyFor(invocation.InvocationTarget);
			ISessionFactory sessionFactory = ObtainSessionFactoryFor(key);

			ISession session = sessionFactory.OpenSession();
			SessionManager.CurrentSession = session;

			if (EnlistSessionIfHasTransactionActive(key, session))
			{
				return invocation.Proceed(args);
			}

			try
			{
				return invocation.Proceed(args);
			}
			finally
			{
				session.Flush();
				session.Close();
				session = null;
				SessionManager.CurrentSession = null;
			}
		}

		private bool EnlistSessionIfHasTransactionActive(String key, ISession session)
		{
			if (!_kernel.HasComponent(typeof(ITransactionManager))) return false;

			bool enlisted = false;

			if (key == null) key = "nhibernate.sf";

			ITransactionManager manager = (ITransactionManager) _kernel[ typeof(ITransactionManager) ];

			Transaction transaction = manager.CurrentTransaction;

			if (transaction != null)
			{
				if (!transaction.Context.Contains(key))
				{
					transaction.Context[key] = true;
					transaction.Enlist(new ResourceSessionAdapter(session.BeginTransaction()));
					transaction.RegisterSynchronization( new SessionKeeper(session) );
					enlisted = true;
				}
			}

			_kernel.ReleaseComponent(manager);

			return enlisted;
		}

		protected String ObtainSessionFactoryKeyFor(object target)
		{
			// TODO: Use the key specified in the attribute - if any
			// Returns the first ISessionFactory registered, which might
			// be wrong if more than one factory was registered.

			return null;
		}

		protected ISessionFactory ObtainSessionFactoryFor(String key)
		{
			if (key == null)
			{
				return (ISessionFactory) _kernel[ typeof(ISessionFactory) ];
			}
			return (ISessionFactory) _kernel[ key ];
		}
	}
}
