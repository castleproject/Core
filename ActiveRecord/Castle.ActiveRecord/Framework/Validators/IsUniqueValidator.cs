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
			ActiveRecordValidationBase arInstance = (ActiveRecordValidationBase) instance;
			ActiveRecordModel model = ActiveRecordBase.GetModel( arInstance.GetType() );

			while (model != null)
			{
				if (model.Ids.Count != 0)
				{
					_pkModel = model.Ids[0] as PrimaryKeyModel;
				}

				model = model.Parent;
			}

			if (_pkModel == null)
			{
				throw new ValidationFailure("We couldn't find the primary key for " + arInstance.GetType().FullName + 
					" so we can't ensure the uniqueness of any field. Validatior failed");
			}

			_fieldValue = fieldValue;

			return (bool) arInstance.Execute( new NHibernateDelegate(CheckUniqueness) );
		}

		private object CheckUniqueness(ISession session, object instance)
		{
			ICriteria criteria = session.CreateCriteria( instance.GetType() );

			object id = _pkModel.Property.GetValue( instance, new object[0] );

			criteria.Add( Expression.And(
				Expression.Eq(Property.Name, _fieldValue), 
				Expression.Not(Expression.Eq(_pkModel.Property.Name, id))) );

			return criteria.List().Count == 0;
		}

		protected override string BuildErrorMessage()
		{
			return String.Format("{0} is currently in use. Please pick up a new {0}.", Property.Name);
		}
	}
}
