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

	using Castle.Core;
	using Castle.Core.Interceptor;

	using Castle.MicroKernel;

	using Castle.Services.Transaction;

	/// <summary>
	/// Intercepts call for transactional components, coordinating
	/// the transaction creation, commit/rollback accordingly to the 
	/// method execution. Rollback is invoked if an exception is threw.
	/// </summary>
	[Transient]
	public class TransactionInterceptor : MarshalByRefObject, IMethodInterceptor, IOnBehalfAware
	{
		private readonly IKernel kernel;
		private readonly TransactionMetaInfoStore infoStore;
		private TransactionMetaInfo metaInfo;

		public TransactionInterceptor(IKernel kernel, TransactionMetaInfoStore infoStore)
		{
            this.kernel = kernel;
			this.infoStore = infoStore;
		}

		#region MarshalByRefObject

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		#region IOnBehalfAware

		public void SetInterceptedComponentModel(ComponentModel target)
		{
			metaInfo = infoStore.GetMetaFor(target.Implementation);
		}

		#endregion

		public object Intercept(IMethodInvocation invocation, params object[] args)
		{
			MethodInfo methodInfo = invocation.MethodInvocationTarget;

			if (metaInfo == null || !metaInfo.Contains(methodInfo))
			{
				return invocation.Proceed(args);
			}
			else
			{
				TransactionAttribute transactionAtt = metaInfo.GetTransactionAttributeFor(methodInfo);

				ITransactionManager manager = (ITransactionManager) kernel[ typeof(ITransactionManager) ];

				ITransaction transaction = 
					manager.CreateTransaction( 
						transactionAtt.TransactionMode, transactionAtt.IsolationMode );

				if (transaction == null)
				{
					return invocation.Proceed(args);
				}

				object value = null;

				transaction.Begin();

				bool rolledback = false;

				try
				{
					value = invocation.Proceed(args);

					if (transaction.IsRollbackOnlySet)
					{
						rolledback = true;
						transaction.Rollback();
					}
					else
					{
						transaction.Commit();
					}
				}
				catch(TransactionException)
				{
					// Whoops. Special case, let's throw without 
					// attempt to rollback anything

					throw;
				}
				catch(Exception)
				{
					if (!rolledback)
					{
						transaction.Rollback();
					}

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