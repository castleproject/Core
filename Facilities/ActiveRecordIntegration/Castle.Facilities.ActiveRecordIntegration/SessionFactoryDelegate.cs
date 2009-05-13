// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace Castle.Facilities.ActiveRecordIntegration
{
	using System;
	using System.Data;

	using NHibernate;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Implements <see cref="ISessionFactory"/> allowing 
	/// it to be used by the container as an ordinary component.
	/// However only <see cref="ISessionFactory.OpenSession(IDbConnection)"/>
	/// is implemented
	/// </summary>
	public static class SessionFactoryDelegate
	{
		private static readonly ProxyGenerator generator = new ProxyGenerator();

		/// <summary>
		/// Creates the specified session factory delegate from the holder and root type
		/// </summary>
		/// <param name="sessionFactoryHolder">The session factory holder.</param>
		/// <param name="arType">Type of the AR entity.</param>
		public static ISessionFactory Create(ISessionFactoryHolder sessionFactoryHolder, Type arType)
		{
			object proxy = generator.CreateInterfaceProxyWithoutTarget(typeof(ISessionFactory), new SessionFactoryDelegateProxy(sessionFactoryHolder, arType));
			return (ISessionFactory)proxy;
		}

		private class SessionFactoryDelegateProxy : Castle.Core.Interceptor.IInterceptor
		{
			private readonly ISessionFactoryHolder sessionFactoryHolder;
			private readonly Type arType;

			public SessionFactoryDelegateProxy(ISessionFactoryHolder sessionFactoryHolder, Type arType)
			{
				this.sessionFactoryHolder = sessionFactoryHolder;
				this.arType = arType;
			}

			public void Intercept(IInvocation invocation)
			{
				if (invocation.Method.Name == "OpenSession" && invocation.Arguments.Length == 0)
				{
					ISession session = sessionFactoryHolder.CreateSession(arType);
					invocation.ReturnValue = new SafeSessionProxy(sessionFactoryHolder, session);
					return;
				}
				throw new NotImplementedException("SessionFactoryDelegate: not implemented");
			}
		}
	}
}
