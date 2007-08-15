// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.Synchronize
{
	using System;
	using System.ComponentModel;
	using System.Reflection;
	using System.Threading;
	using Castle.Core;
	using Castle.Core.Interceptor;
	using Castle.MicroKernel;

	/// <summary>
	/// Intercepts calls to synchronized components and ensures
	/// that they execute in the proper synchronization context.
	/// </summary>
	[Transient]
	internal class SynchronizeInterceptor : IInterceptor, IOnBehalfAware
	{
		private readonly IKernel kernel;
		private SynchronizeMetaInfo metaInfo;
		private readonly SynchronizeMetaInfoStore metaStore;
		private readonly InvocationDelegate safeInvokeDelegate = InvokeSafely;
		[ThreadStatic] private SynchronizationContext activeSyncContext;

		private delegate void InvocationDelegate(IInvocation invocation);

		/// <summary>
		/// Initializes a new instance of the <see cref="SynchronizeInterceptor"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="metaStore">The meta store.</param>
		public SynchronizeInterceptor(IKernel kernel, SynchronizeMetaInfoStore metaStore)
		{
			this.kernel = kernel;
			this.metaStore = metaStore;
		}

		#region IOnBehalfAware

		/// <summary>
		/// Sets the intercepted ComponentModel.
		/// </summary>
		/// <param name="target">The targets ComponentModel.</param>
		public void SetInterceptedComponentModel(ComponentModel target)
		{
			metaInfo = metaStore.GetMetaFor(target.Implementation);
		}

		#endregion

		/// <summary>
		/// Intercepts the invocation and applies any necessary
		/// synchronization.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		public void Intercept(IInvocation invocation)
		{
			if (!InvokeInSynchronizationContext(invocation) &&
			    !InvokeUsingSynchronizationTarget(invocation))
			{
				invocation.Proceed();
			}
		}

		/// <summary>
		/// Continues the invocation in a synchronization context
		/// if necessary.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <returns>
		/// 	<c>true</c> if continued; otherwise, <c>false</c>.
		/// </returns>
		private bool InvokeInSynchronizationContext(IInvocation invocation)
		{
			if (metaInfo != null)
			{
				IHandler handler = null;
				MethodInfo methodInfo = invocation.MethodInvocationTarget;
				SynchronizeContextReference syncContextRef = metaInfo.GetSynchronizedContextFor(methodInfo);

				if (syncContextRef == null)
				{
					invocation.Proceed();
					return true;
				}

				switch(syncContextRef.ReferenceType)
				{
					case SynchronizeContextReferenceType.Key:
						handler = kernel.GetHandler(syncContextRef.ComponentKey);
						break;

					case SynchronizeContextReferenceType.Interface:
						handler = kernel.GetHandler(syncContextRef.ServiceType);
						break;
				}

				if (handler == null)
				{
					throw new ApplicationException("The synchronization context could not be resolved");
				}

				SynchronizationContext syncContext = handler.Resolve(CreationContext.Empty)
				                                     as SynchronizationContext;

				if (syncContext == null)
				{
					throw new ApplicationException(string.Format("{0} does not implement {1}",
					                                             syncContextRef, typeof(SynchronizationContext).FullName));
				}

				if (syncContext != activeSyncContext)
				{
					SynchronizationContext prevSyncContext = SynchronizationContext.Current;

					try
					{
						SynchronizationContext.SetSynchronizationContext(syncContext);

						syncContext.Send(delegate
						                 	{
						                 		activeSyncContext = syncContext;

						                 		try
						                 		{
						                 			InvokeSafely(invocation);
						                 		}
						                 		finally
						                 		{
						                 			activeSyncContext = null;
						                 		}
						                 	}, null);
					}
					finally
					{
						SynchronizationContext.SetSynchronizationContext(prevSyncContext);
					}
				}
				else
				{
					invocation.Proceed();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Continues the invocation using the targets implicit
		/// synchronization if necessary.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <returns>
		/// 	<c>true</c> if continued; otherwise, <c>false</c>.
		/// </returns>
		private bool InvokeUsingSynchronizationTarget(IInvocation invocation)
		{
			ISynchronizeInvoke syncTarget = (ISynchronizeInvoke) invocation.InvocationTarget;

			if (syncTarget != null)
			{
				if (syncTarget.InvokeRequired)
				{
					syncTarget.Invoke(safeInvokeDelegate, new object[] {invocation});
				}
				else
				{
					invocation.Proceed();
				}

				return true;
			}

			return false;
		}

		/// <summary>
		/// Used by the safe synchronization delegate.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		private static void InvokeSafely(IInvocation invocation)
		{
			invocation.Proceed();
		}
	}
}