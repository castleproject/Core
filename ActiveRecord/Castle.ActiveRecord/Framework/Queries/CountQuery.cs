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

namespace Castle.ActiveRecord.Framework.Queries
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Queries;
	
	using NHibernate;

	public class CountQuery : HqlBasedQuery
	{
		public CountQuery(Type targetType, string filter, params object[] parameters)
			: base(targetType,  "SELECT COUNT(*) FROM " + targetType.Name + " WHERE " + filter, parameters) 
		{
		}

		public CountQuery(Type targetType) : this(targetType, "1=1", null) 
		{
		}

		protected override object InternalExecute(ISession session)
		{
			IQuery q = base.CreateQuery(session);

			IList result = q.List();
				
			return (int) result[0];
		}
	}
}
