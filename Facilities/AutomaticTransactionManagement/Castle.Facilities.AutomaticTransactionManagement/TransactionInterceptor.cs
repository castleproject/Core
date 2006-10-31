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

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionInterceptor"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="infoStore">The info store.</param>
		public TransactionInterceptor(IKernel kernel, TransactionMetaInfoStore infoStore)
		{
            this.kernel = kernel;
			this.infoStore = infoStore;
		}

		#region MarshalByRefObject

		/// <summary>
		/// Obtains a lifetime service object to control the lifetime policy for this instance.
		/// </summary>
		/// <returns>
		/// An object of type <see cref="T:System.Runtime.Remoting.Lifetime.ILease"/> used to control the
		/// lifetime policy for this instance. This is the current lifetime service object for
		/// this instance if one exists; otherwise, a new lifetime service object initialized to the value
		/// of the <see cref="P:System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime" qualify="true"/> property.
		/// </returns>
		/// <exception cref="T:System.Security.SecurityException">The immediate caller does not have infrastructure permission. </exception>
		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		#region IOnBehalfAware

		/// <summary>
		/// Sets the intercepted component's ComponentModel.
		/// </summary>
		/// <param name="target">The target's ComponentModel</param>
		public void SetInterceptedComponentModel(ComponentModel target)
		{
			metaInfo = infoStore.GetMetaFor(target.Implementation);
		}

		#endregion

		/// <summary>
		/// Intercepts the specified invocation and creates a transaction
		/// if necessary.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="args">The args.</param>
		/// <returns></returns>
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