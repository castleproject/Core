// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using Castle.ActiveRecord.Framework.Internal;
	using NHibernate;
	using NHibernate.Classic;
	using NHibernate.Criterion;
	using Castle.Components.Validator;


	/// <summary>
	/// Validate that the property's value is unique in the database when saved
	/// </summary>
	[Serializable]
	public class IsUniqueValidator : AbstractValidator
	{
		[ThreadStatic] 
		private static object fieldValue;

		[ThreadStatic] 
		private static PrimaryKeyModel pkModel;

		/// <summary>
		/// Initializes a new instance of the <see cref="IsUniqueValidator"/> class.
		/// </summary>
		public IsUniqueValidator()
		{
		}

		/// <summary>
		/// Perform the check that the property value is unqiue in the table
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			Type instanceType = instance.GetType();
			ActiveRecordModel model = ActiveRecordBase.GetModel(instance.GetType());

			while(model != null)
			{
				if (model.PrimaryKey != null)
				{
					pkModel = model.PrimaryKey;
				}

				model = model.Parent;
			}

			if (pkModel == null)
			{
				throw new ValidationFailure("We couldn't find the primary key for " + instanceType.FullName +
				                            " so we can't ensure the uniqueness of any field. Validatior failed");
			}

			IsUniqueValidator.fieldValue = fieldValue;

			SessionScope scope = null;

			if (SessionScope.Current == null ||
			    SessionScope.Current.ScopeType != SessionScopeType.Transactional)
			{
				scope = new SessionScope();
			}

			try
			{
				return (bool) ActiveRecordMediator.Execute(instanceType, CheckUniqueness, instance);
			}
			finally
			{
				if (scope != null)
				{
					scope.Dispose();
				}
			}
		}

		private object CheckUniqueness(ISession session, object instance)
		{
			ICriteria criteria = session.CreateCriteria(instance.GetType());

			if (Property.Name.Equals(pkModel.Property.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				// IsUniqueValidator is on the PrimaryKey Property, simplify query
				criteria.Add(Expression.Eq(Property.Name, fieldValue));
			}
			else
			{
				object id = pkModel.Property.GetValue(instance, new object[0]);
				ICriterion pKeyCriteria = (id == null)
				                          	? Expression.IsNull(pkModel.Property.Name)
				                          	: Expression.Eq(pkModel.Property.Name, id);
				criteria.Add(Expression.And(Expression.Eq(Property.Name, fieldValue), Expression.Not(pKeyCriteria)));
			}
			return criteria.List().Count == 0;
		}

		/// <summary>
		/// Builds the error message when the property value is not unique
		/// </summary>
		/// <returns></returns>
		protected override string BuildErrorMessage()
		{
			return String.Format("{0} is currently in use. Please pick up a new {0}.", Property.Name);
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return false; }
		}

		/// <summary>
		/// Applies the browser validation by setting up one or
		/// more input rules on <see cref="IBrowserValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		/// <param name="target">The target.</param>
		public override void ApplyBrowserValidation(BrowserValidationConfiguration config, InputElementType inputType,
		                                            IBrowserValidationGenerator generator, IDictionary attributes, string target)
		{
		}
	}
}