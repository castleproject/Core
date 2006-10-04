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

namespace Castle.Facilities.Db4oIntegration.Tests.Components
{
	using System;

	using com.db4o;

	using Castle.Services.Transaction;

	[Transactional]
	public class BeerBox
	{
		private readonly BeerTransactionalDao _dao;

		public BeerBox(BeerTransactionalDao dao)
		{
			_dao = dao;
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void AddBeer(Beer b)
		{
			_dao.Create(b);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void RemoveBeer(Beer b)
		{
			_dao.Remove(b);
		}

		[Transaction(TransactionMode.Requires)]
		public virtual void AddAndBroke(Beer b)
		{
			_dao.Create(b);

			throw new ApplicationException("Rollback It!!");
		}

		public Beer Load(Guid id)
		{
			return _dao.Load(id);
		}

		[CLSCompliant(false)]
		public ObjectSet GetAll()
		{
			return _dao.FindAll();
		}
	}
}
