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

namespace Castle.Facilities.NHibernateIntegration.Internal
{
	using NHibernate;

	using Castle.Services.Transaction;

	/// <summary>
	/// 
	/// </summary>
	public class SessionDisposeSynchronization : ISynchronization
	{
		private readonly SessionDelegate session;

		/// <summary>
		/// Initializes a new instance of the <see cref="SessionDisposeSynchronization"/> class.
		/// </summary>
		/// <param name="session">The session.</param>
		public SessionDisposeSynchronization(SessionDelegate session)
		{
			this.session = session;
		}

		/// <summary>
		/// Implementors may have code executing
		/// just before the transaction completes
		/// </summary>
		public void BeforeCompletion()
		{
			
		}

		/// <summary>
		/// Implementors may have code executing
		/// just after the transaction completes
		/// </summary>
		public void AfterCompletion()
		{
			session.InternalClose(false);
		}
	}
}
