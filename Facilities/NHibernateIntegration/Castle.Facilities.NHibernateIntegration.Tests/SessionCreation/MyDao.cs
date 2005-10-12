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

namespace Castle.Facilities.NHibernateIntegration.Tests.SessionCreation
{
	using System;
	using NHibernate;
	using NUnit.Framework;

	public class MyDao
	{
		private readonly MySecondDao otherDao;
		private readonly ISessionManager sessManager;

		public MyDao(MySecondDao otherDao, ISessionManager sessManager)
		{
			this.sessManager = sessManager;
			this.otherDao = otherDao;
		}

		public void PerformComplexOperation1()
		{
			using(ISession sess = sessManager.OpenSession())
			{
				Assert.IsNotNull(sess);

				otherDao.PerformPieceOfOperation(sess);
			}
		}

		public void PerformComplexOperation2()
		{
			ISession prev = null;

			using(ISession sess = sessManager.OpenSession())
			{
				prev = sess;
			}

			otherDao.PerformPieceOfOperation2(prev);
		}

		public void DoOpenCloseAndDispose()
		{
			using(ISession sess = sessManager.OpenSession())
			{
				Assert.IsTrue(sess.IsConnected);
				Assert.IsTrue(sess.IsOpen);

				sess.Close();
				
				Assert.IsFalse(sess.IsConnected);
				Assert.IsFalse(sess.IsOpen);
			}
		}
	}
}
