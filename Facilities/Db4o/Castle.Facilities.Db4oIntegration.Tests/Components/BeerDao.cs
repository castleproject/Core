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

namespace Castle.Facilities.Db4oIntegration.Tests.Components
{
	using System;

	using com.db4o;
	using com.db4o.query;
	
	public class BeerDao
	{
		private readonly ObjectContainer _objContainer;

		public BeerDao(ObjectContainer objContainer)
		{
			_objContainer = objContainer;
		}

		public virtual void Create(Beer beer)
		{
			_objContainer.set(beer);
		}

		public virtual void Remove(Beer beer)
		{
			_objContainer.delete(beer);
		}

		public virtual Beer Load(object id)
		{
			Query query = _objContainer.query();

			query.constrain(typeof(Beer));
			query.descend("_id").constrain(id);

			if (query.execute().size() == 0)
			{
				return null;
			}
			else
			{
				return (Beer) query.execute().next();
			}
		}

		public virtual ObjectSet FindAll()
		{
			return _objContainer.get(typeof(Beer));
		}
	}
}
