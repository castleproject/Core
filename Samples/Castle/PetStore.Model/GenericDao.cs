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

namespace PetStore.Model
{
	using System.Linq;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Linq;
	using Castle.Components.Validator;
	using NHibernate;

	public class GenericDao<TClass> :IDao<TClass> where TClass:class, IAggregateRoot
	{
		private IValidatorRegistry registry;
		private IValidatorRunner runner;

		public virtual IValidatorRegistry Registry
		{
			get { return registry; }
			set { registry = value; }
		}

		public virtual IValidatorRunner Runner
		{
			get { return runner; }
			set { runner = value; }
		}

		public GenericDao()
		{
			registry = new CachedValidationRegistry();
			runner = new ValidatorRunner(registry);
		}

		public void Create(TClass t)
		{
			Validate(t);
			ActiveRecordMediator<TClass>.Create(t);
		}

		protected virtual void Validate(TClass t)
		{
			if (!Runner.IsValid(t))
			{
				var errors = Runner.GetErrorSummary(t);
				throw new ValidationException(string.Format("{0} is not valid", typeof (TClass).FullName), errors.ErrorMessages);
			}
		}

		public void Update(TClass t)
		{
			Validate(t);
			ActiveRecordMediator<TClass>.Update(t);
		}

		public void Save(TClass t)
		{
			Validate(t);
			ActiveRecordMediator<TClass>.Save(t);
		}

		public void Delete(TClass t)
		{
			ActiveRecordMediator<TClass>.Delete(t);
		}

		public TClass Find(object id)
		{
			return ActiveRecordMediator<TClass>.FindByPrimaryKey(id);
		}

		public IQueryable<TClass> Linq()
		{
			return ActiveRecordLinq.AsQueryable<TClass>();
		}

		public ICriteria GetCriteria()
		{
			return ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof (TClass)).CreateCriteria<TClass>();
		}

		public ICriteria GetCriteria(string alias)
		{
			return ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof(TClass)).CreateCriteria<TClass>(alias);
		}

		public IQuery GetQuery(string query)
		{
			return ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof(TClass)).CreateQuery(query);
		}

		public IQuery GetNamedQuery(string name)
		{
			return ActiveRecordMediator.GetSessionFactoryHolder().CreateSession(typeof(TClass)).GetNamedQuery(name);
		}
	}
}