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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Collections;
	
	using NUnit.Framework;
	
	using Castle.Windsor;
	using Castle.Model.Configuration;
	using Castle.MicroKernel.SubSystems.Configuration;

	using NHibernate;

	/// <summary>
	/// Summary description for NHibernateFacilityTestCase.
	/// </summary>
	// [TestFixture]
	public class NHibernateFacilityTestCase : AbstractNHibernateTestCase
	{
		[Test]
		public void Usage()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility("nhibernate", new NHibernateFacility());

			ISessionFactory factory = (ISessionFactory) container["sessionFactory1"];
			ISession session = factory.OpenSession();

			const string BlogName = "hammett's blog";

			Blog blog = new Blog();
			blog.Name = BlogName;
			blog.Items = new ArrayList();

			ITransaction tx = null;

			try
			{
				tx = session.BeginTransaction();
				session.Save(blog);
				tx.Commit();
			}
			catch (HibernateException e)
			{
				if (tx != null) tx.Rollback();
				throw e;
			}
			finally
			{
				session.Close();
			}

			session = factory.OpenSession();

			try
			{
				blog = (Blog) 
					session.Find("from Blog as b where b.Name=:name", BlogName, NHibernate.String)[0];
				Assert.IsNotNull(blog);
				Assert.AreEqual(0, blog.Items.Count);
			}
			finally
			{
				session.Close();
			}
		}

	}
}