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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;
	
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.ActiveRecord.Framework.Scopes;

	using NHibernate;
	using NHibernate.Expression;


	[Serializable]
	public class IsUniqueValidator : AbstractValidator
	{
		[ThreadStatic]
		private static object _fieldValue;
		[ThreadStatic]
		private static PrimaryKeyModel _pkModel;

		public IsUniqueValidator()
		{
		}

		public override bool Perform(object instance, object fieldValue)
		{
			Type instanceType = instance.GetType();
			ActiveRecordModel model = ActiveRecordBase.GetModel(instance.GetType());

			while (model != null)
			{
				if (model.PrimaryKey != null)
				{
					_pkModel = model.PrimaryKey;
				}

				model = model.Parent;
			}

			if (_pkModel == null)
			{
				throw new ValidationFailure("We couldn't find the primary key for " + instanceType.FullName + 
					" so we can't ensure the uniqueness of any field. Validatior failed");
			}
			
			_fieldValue = fieldValue;
			
			SessionScope scope = null;
			if (ThreadScopeAccessor.Instance.GetRegisteredScope() == null
				|| ThreadScopeAccessor.Instance.GetRegisteredScope().GetType() != typeof(TransactionScope))
			{
				scope = new SessionScope();
			}
			try
			{
				return (bool) ActiveRecordMediator.Execute(instanceType, new NHibernateDelegate(CheckUniqueness), instance);
			}
			finally
			{
				if (scope != null) scope.Dispose();
			}
		}

		private object CheckUniqueness(ISession session, object instance)
		{
			ICriteria criteria = session.CreateCriteria( instance.GetType() );
#if DOTNET2
			if (Property.Name.Equals(_pkModel.Property.Name, StringComparison.InvariantCultureIgnoreCase))
#else
			if (Property.Name.ToLower() == _pkModel.Property.Name.ToLower())
#endif
			{
				// IsUniqueValidator is on the PrimaryKey Property, simplify query
				criteria.Add(Expression.Eq(Property.Name, _fieldValue));
			}
			else
			{
				object id = _pkModel.Property.GetValue(instance, new object[0]);
				criteria.Add(Expression.And(Expression.Eq(Property.Name, _fieldValue), Expression.Not(Expression.Eq(_pkModel.Property.Name, id))));
			}
			return criteria.List().Count == 0;
		}

		protected override string BuildErrorMessage()
		{
			return String.Format("{0} is currently in use. Please pick up a new {0}.", Property.Name);
		}
	}
}
