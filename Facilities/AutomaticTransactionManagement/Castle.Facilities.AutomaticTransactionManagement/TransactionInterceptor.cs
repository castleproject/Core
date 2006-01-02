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

namespace Castle.Facilities.AutomaticTransactionManagement
{
	using System;
	using System.Reflection;

	using Castle.Model.Interceptor;

	using Castle.MicroKernel;

	using Castle.Services.Transaction;

	/// <summary>
	/// Summary description for TransactionInterceptor.
	/// </summary>
	public class TransactionInterceptor : MarshalByRefObject, IMethodInterceptor
	{
		private IKernel _kernel;

		public TransactionInterceptor(IKernel kernel)
		{
            _kernel = kernel;
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			MethodInfo methodInfo = invocation.MethodInvocationTarget;

			if (!methodInfo.IsDefined( typeof(TransactionAttribute), true ))
			{
				return invocation.Proceed(args);
			}
			else
			{
				object[] attrs = methodInfo.GetCustomAttributes( typeof(TransactionAttribute), true );

				TransactionAttribute transactionAtt = (TransactionAttribute) attrs[0];

				ITransactionManager manager = (ITransactionManager) _kernel[ typeof(ITransactionManager) ];

				ITransaction transaction = 
					manager.CreateTransaction( 
						transactionAtt.TransactionMode, transactionAtt.IsolationMode );

				if (transaction == null)
				{
					return invocation.Proceed(args);
				}

				object value = null;

				transaction.Begin();

				try
				{
					value = invocation.Proceed(args);

					transaction.Commit();
				}
				catch(TransactionException)
				{
					// Whoops. Special case, let's throw without 
					// attempt to rollback anything

					throw;
				}
				catch(Exception)
				{
					transaction.Rollback();

					throw;
				}
				finally
				{
					manager.Dispose(transaction); 
				}

				return value;
			}
		}
	}
}