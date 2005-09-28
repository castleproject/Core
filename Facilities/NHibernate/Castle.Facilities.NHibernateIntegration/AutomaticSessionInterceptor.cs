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

using System.Reflection;
using Transaction = Castle.Services.Transaction.ITransaction;

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using Castle.Model.Interceptor;

	using Castle.MicroKernel;

	using Castle.Facilities.NHibernateExtension;

	using Castle.Services.Transaction;

	using NHibernate;


	/// <summary>
	/// Opens, configure and ensure the proper flush and close
	/// of a NHibernate's Session object.
	/// </summary>
	public class AutomaticSessionInterceptor : IMethodInterceptor
	{
		private const string SessionFactoryKeyDefault = "nhibernate.sessfactory.default";

		private IKernel _kernel;

		public AutomaticSessionInterceptor(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			String key = ObtainSessionFactoryKeyFor(invocation.InvocationTarget);

			if (SessionManager.IsCurrentSessionCompatible(key))
			{
				return invocation.Proceed(args);
			}

			ISessionFactory sessionFactory = ObtainSessionFactoryFor(key);

			ISession session = null;

			if (_kernel.HasComponent( typeof(NHibernate.IInterceptor) ))
			{
				session = sessionFactory.OpenSession( (NHibernate.IInterceptor) _kernel[typeof(NHibernate.IInterceptor)] );
			}
			else
			{
				session = sessionFactory.OpenSession();
			}

			SessionManager.Push(session, key);

			FlushOption flushOption = ExtractFlushOption(invocation.MethodInvocationTarget);

			ConfigureFlushMode(flushOption, session);

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
				if (flushOption == FlushOption.Force)
				{
					session.Flush();
				}
				session.Close();
				SessionManager.Pop(key);
			}
		}

		private static void ConfigureFlushMode(FlushOption flushOption, ISession session)
		{
			switch(flushOption)
			{
				case FlushOption.Auto:
					session.FlushMode = FlushMode.Auto;
					break;
				case FlushOption.Commit:
					session.FlushMode = FlushMode.Commit;
					break;
				case FlushOption.Never:
					session.FlushMode = FlushMode.Never;
					break;
			}
		}

		private bool EnlistSessionIfHasTransactionActive(String key, ISession session)
		{
			if (!_kernel.HasComponent(typeof(ITransactionManager))) return false;

			bool enlisted = false;

			ITransactionManager manager = (ITransactionManager) _kernel[ typeof(ITransactionManager) ];

			Transaction transaction = manager.CurrentTransaction;

			if (transaction != null)
			{
				if (!transaction.Context.Contains(key))
				{
					transaction.Context[key] = true;
					transaction.Enlist(new ResourceSessionAdapter(session.BeginTransaction()));
					transaction.RegisterSynchronization( new SessionKeeper(session, key) );
					enlisted = true;
				}
			}

			_kernel.ReleaseComponent(manager);

			return enlisted;
		}

		protected FlushOption ExtractFlushOption(MethodInfo method)
		{
			object[] attrs = method.GetCustomAttributes( typeof(SessionFlushAttribute), true );

			if (attrs.Length == 0)
			{
				return FlushOption.Auto;
			}
			else
			{
				return (attrs[0] as SessionFlushAttribute).FlushOption;
			}
		}

		protected virtual String ObtainSessionFactoryKeyFor(object target)
		{
			object[] attributes = target.GetType().GetCustomAttributes( 
				typeof(UsesAutomaticSessionCreationAttribute), true );

			UsesAutomaticSessionCreationAttribute attribute = attributes[0] as UsesAutomaticSessionCreationAttribute;

			String key = attribute.SessionFactoryId;

			if (key == null)
			{
				key = SessionFactoryKeyDefault;
			}

			return key;
		}

		protected ISessionFactory ObtainSessionFactoryFor(String key)
		{
			if (key == SessionFactoryKeyDefault)
			{
				return (ISessionFactory) _kernel[ typeof(ISessionFactory) ];
			}
			return (ISessionFactory) _kernel[ key ];
		}
	}
}
