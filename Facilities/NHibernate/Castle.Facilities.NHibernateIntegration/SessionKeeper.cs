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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;
	using Castle.Services.Transaction;

	using Castle.Facilities.NHibernateExtension;

	using NHibernate;


	/// <summary>
	/// Synchronization object to ensure proper close and 
	/// flush of the session
	/// </summary>
	public class SessionKeeper : ISynchronization
	{
		private readonly ISession session;
		private readonly String key;

		public SessionKeeper(ISession session, String key)
		{
			this.session = session;
			this.key = key;
		}

		public void BeforeCompletion()
		{
		}

		public void AfterCompletion()
		{
			session.Close();
			SessionManager.Pop(key);
		}
	}
}
