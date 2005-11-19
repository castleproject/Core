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

namespace PetStore.Service.DataAccess.AR
{
	using System;
	using System.Collections;

	using NHibernate;

	/// <summary>
	/// ActiveRecord implementation of a data access class 
	/// for the Product entity.
	/// </summary>
	public class ARProductDataAccess : IProductDataAccess
	{
		public ISessionFactory sessionFactory;

		/// <summary>
		/// As you probably know the ISessionFactory is a
		/// NHibernate interface. As we're using ActiveRecord integration
		/// facility its implementation (at least the one we will get here)
		/// is provided by the facility and the only method available 
		/// is OpenSession(). This allow the facility to provide
		/// automatic transaction management and others goodies.
		/// </summary>
		/// <param name="sessionFactory"></param>
		public ARProductDataAccess(ISessionFactory sessionFactory)
		{
			this.sessionFactory = sessionFactory;
		}

		/// <summary>
		/// We need to return only the ids here, 
		/// and there are a bunch of options. We 
		/// choose to use a NH query through ActiveRecord
		/// </summary>
		/// <returns></returns>
		public int[] GetProductIds()
		{
			using(ISession session = sessionFactory.OpenSession())
			{
				// Note: this is HQL
				IQuery query = session.CreateQuery("select p.Id from Product p");

				IList list = query.List();

				int[] intArray = new int[list.Count];

				list.CopyTo(intArray, 0);

				return intArray;
			}
		}
	}
}
