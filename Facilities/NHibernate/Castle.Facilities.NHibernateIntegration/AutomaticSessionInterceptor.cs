// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using Castle.Model.Interceptor;

	using Castle.MicroKernel;

	using Castle.Facilities.NHibernateExtension;

	using NHibernate;

	/// <summary>
	/// Summary description for AutomaticSessionInterceptor.
	/// </summary>
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

			ISessionFactory sessionFactory = 
				ObtainSessionFactoryFor(invocation.InvocationTarget);

			SessionManager.CurrentSession = sessionFactory.OpenSession();

			try
			{
				return invocation.Proceed(args);
			}
			finally
			{
				SessionManager.CurrentSession.Flush();
				SessionManager.CurrentSession.Close();
				SessionManager.CurrentSession = null;
			}
		}

		protected ISessionFactory ObtainSessionFactoryFor(object target)
		{
			// TODO: Use the key specified in the attribute - if any

			// Returns the first ISessionFactory registered, which might
			// be wrong if more than one factory was registered.
			return (ISessionFactory) _kernel[ typeof(ISessionFactory) ];
		}
	}
}
