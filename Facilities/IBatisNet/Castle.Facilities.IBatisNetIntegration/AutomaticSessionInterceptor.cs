#region Licence
/// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
///  
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///  
/// http://www.apache.org/licenses/LICENSE-2.0
///  
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// -- 
/// 
/// This facility was a contribution kindly 
/// donated by Gilles Bayon <gilles.bayon@gmail.com>
/// 
/// --
#endregion

#region Using
using System;
using System.Reflection;

using log4net;

using Castle.Model.Interceptor;
using Castle.MicroKernel;
using Castle.Services.Transaction;
using Transaction = Castle.Services.Transaction.ITransaction;

using IBatisNet.DataMapper;
#endregion

namespace Castle.Facilities.IBatisNetIntegration
{

	/// <summary>
	/// Summary description for AutomaticSessionInterceptor.
	/// </summary>
	public class AutomaticSessionInterceptor : IMethodInterceptor
	{
		private IKernel _kernel = null;
		private static readonly ILog _logger = LogManager.GetLogger( System.Reflection.MethodBase.GetCurrentMethod().DeclaringType );

		public AutomaticSessionInterceptor(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			// Get custom attributes from the method   
			#region Important remark
			// For line 62, I have first tried 
			// invocation.Method.IsDefined( typeof(NoSessionAttribute), true )
			// but this not not work, don't know why (perhaps because I use an interface IService for my Service class on test...)
			// This bug may be affected also the TransactionAttribute as the Intercept method used the same method
			// (99% of chance)
			// Must be investigate
			#endregion

			MethodInfo info = invocation.Method;
			// MethodInfo info = invocation.InvocationTarget.GetType().GetMethod(invocation.Method.Name);

			if (info.IsDefined( typeof(NoSessionAttribute), true ))
			{
				return invocation.Proceed(args);
			}

			String key = ObtainSqlMapKeyFor(info);

			SqlMapper sqlMap = ObtainSqlMapperFor(key);

			if (sqlMap.IsSessionStarted)
			{
				return invocation.Proceed(args);
			}

			if (_logger.IsDebugEnabled) 
			{
				_logger.Debug("Automatic Open connection on method :" + invocation.Method.Name);
			}

			sqlMap.OpenConnection();

			if (EnlistSessionIfHasTransactionActive(key, sqlMap))
			{
				return invocation.Proceed(args);
			}

			try
			{
				return invocation.Proceed(args);
			}
			finally
			{
				if (_logger.IsDebugEnabled) 
				{
					_logger.Debug("Close connection on method :" + invocation.Method.Name);
				}
				sqlMap.CloseConnection();
			}
		}

		private bool EnlistSessionIfHasTransactionActive(string key, SqlMapper sqlMap)
		{
			if (!_kernel.HasComponent(typeof(ITransactionManager))) return false;

			bool enlisted = false;

			if (key == null) key = "iBATIS.DataMapper";

			ITransactionManager manager = (ITransactionManager) _kernel[ typeof(ITransactionManager) ];

			Transaction transaction = manager.CurrentTransaction;

			if (transaction != null)
			{
				if (!transaction.Context.Contains(key))
				{
					transaction.Context[key] = true;
					transaction.Enlist(new ResourceSqlMapAdapter(sqlMap.BeginTransaction(false)));
					transaction.RegisterSynchronization( new SqlMapKeeper(sqlMap) );
					enlisted = true;
				}
			}

			_kernel.ReleaseComponent(manager);

			return enlisted;
		}

		protected string ObtainSqlMapKeyFor(MethodInfo info)
		{
			String sqlMapID = String.Empty;

			if ( info.IsDefined(typeof(SessionAttribute),true) )  
			{
				SessionAttribute[] attributs = 
					info.GetCustomAttributes(typeof(SessionAttribute), true) as SessionAttribute[];
				sqlMapID = attributs[0].SqlMapId;
			}

			return sqlMapID;
		}

		protected SqlMapper ObtainSqlMapperFor(String key)
		{
			// Use the key specified in the attribute - if any
			if (String.Empty.Equals(key))
			{
				return (SqlMapper) _kernel[ typeof(SqlMapper) ];
			}
			else
			{
				return (SqlMapper) _kernel[ key ];
			}
		}
	}
}
