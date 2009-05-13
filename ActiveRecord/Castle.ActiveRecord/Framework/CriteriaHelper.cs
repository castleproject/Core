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

namespace Castle.ActiveRecord
{
	using System;

	using NHibernate;
	using NHibernate.Criterion;

	internal class CriteriaHelper
	{
		private CriteriaHelper()
		{
			throw new Exception("Instances Not Supported");
		}

		/// <summary>
		/// Adds a collection of ICriterion to an ICriteria.
		/// </summary>
		/// <param name="criteria">The ICriteria that will be modified.</param>
		/// <param name="criterions">The collection of Criterion.</param>
		internal static void AddCriterionToCriteria(ICriteria criteria, ICriterion[] criterions)
		{
			if (criterions != null)
			{
				foreach (ICriterion cond in criterions)
				{
					criteria.Add(cond);
				}
			}
		}

		/// <summary>
		/// Adds a collection of Order (sort by) specifiers to an ICriteria.
		/// </summary>
		/// <param name="criteria">The ICriteria that will be modified.</param>
		/// <param name="orders">The collection of Order specifiers.</param>
		internal static void AddOrdersToCriteria(ICriteria criteria, Order[] orders)
		{
			if (orders != null)
			{
				foreach (Order order in orders)
				{
					criteria.AddOrder(order);
				}
			}
		}
	}
}
