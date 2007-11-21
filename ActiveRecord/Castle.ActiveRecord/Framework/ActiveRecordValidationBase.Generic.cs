// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Validator;

	/// <summary>
	/// Extends <see cref="ActiveRecordBase"/> adding automatic validation support.
	/// <seealso cref="ActiveRecordValidationBase.IsValid()"/>
	/// </summary>
	/// <example>
	/// <code>
	/// public class Customer : ActiveRecordBase
	/// {
	///		...
	///		
	///		[Property, ValidateNotEmpty]
	///		public int Name
	///		{
	///			get { return _name; }
	///			set { _name = value; }
	///		}
	///		
	///		[Property, ValidateNotEmpty, ValidateEmail]
	///		public int Email
	///		{
	///			get { return _email; }
	///			set { _email = value; }
	///		}
	///	</code>
	/// </example>
	[Serializable]
	public abstract class ActiveRecordValidationBase<T> : ActiveRecordBase<T> where T : class
	{
		[NonSerialized]
		private ValidatorRunner __runner = new ValidatorRunner(ActiveRecordModelBuilder.ValidatorRegistry);

		[System.Xml.Serialization.XmlIgnore]
		private IDictionary __failedProperties;

		/// <summary>
		/// Constructs an ActiveRecordValidationBase
		/// </summary>
		public ActiveRecordValidationBase()
		{
		}

		/// <summary>
		/// Performs the fields validation. Returns true if no 
		/// validation error was found.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsValid()
		{
			return IsValid(RunWhen.Everytime);
		}

		/// <summary>
		/// Performs the fields validation for the specified action.
		/// </summary>
		/// <param name="runWhen">Use validators appropriate to the action being performed.</param>
		/// <returns>True if no validation error was found</returns>
		public virtual bool IsValid(RunWhen runWhen)
		{
			__failedProperties = new Hashtable();

			if (__runner == null)
			{
				__runner = new ValidatorRunner(ActiveRecordModelBuilder.ValidatorRegistry);
			}

			bool returnValue = __runner.IsValid(this, runWhen);

			foreach (PropertyInfo propinfo in ActiveRecordValidationBase.GetNestedPropertiesToValidate(this))
			{
				object propval = propinfo.GetValue(this, null);

				if (propval != null)
				{
					bool tmp = __runner.IsValid(propval, runWhen);
					if (!tmp)
						__failedProperties.Add(propinfo,
						                       new ArrayList(__runner.GetErrorSummary(propval).GetErrorsForProperty(propinfo.Name)));
					returnValue &= tmp;
				}
			}
		
			if (!returnValue)
			{
				Type type = GetType();
				ErrorSummary summary = __runner.GetErrorSummary(this);

				foreach (string property in summary.InvalidProperties)
				{
					__failedProperties.Add(type.GetProperty(property), new ArrayList(summary.GetErrorsForProperty(property)));
				}
			}

			return returnValue;
		}

		/// <summary>
		/// Returns a list of current validation errors messages.
		/// </summary>
		public virtual String[] ValidationErrorMessages
		{
			get
			{
				if (__runner == null)
				{
					__runner = new ValidatorRunner(ActiveRecordModelBuilder.ValidatorRegistry);
				}

				if (__runner.GetErrorSummary(this) == null)
				{
					IsValid();
				}

				List<string> errorMessages = new List<string>(__runner.GetErrorSummary(this).ErrorMessages);

				ActiveRecordValidationBase.AddNestedPropertyValidationErrorMessages(errorMessages, this, __runner);

				return errorMessages.ToArray();
			}
		}

		/// <summary>
		/// Maps a specific PropertyInfo to a list of
		/// error messages. Useful for frameworks.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		public virtual IDictionary PropertiesValidationErrorMessage
		{
			get { return __failedProperties; }
		}

		/// <summary>
		/// Override the base hook to call validators required for create.
		/// </summary>
		/// <param name="state">The current state of the object</param>
		/// <returns>Returns true if the state has changed otherwise false</returns>
		protected internal override bool BeforeSave(IDictionary state)
		{
			if (!IsValid(RunWhen.Insert))
			{
				OnNotValid();
			}

			return base.BeforeSave(state);
		}

		/// <summary>
		/// Override the base hook to call validators required for update.
		/// </summary>
		/// <param name="id">object id</param>
		/// <param name="previousState">The previous state of the object</param>
		/// <param name="currentState">The current state of the object</param>
		/// <param name="types">Property types</param>
		/// <returns>Returns true if the state has changed otherwise false</returns>
		protected internal override bool OnFlushDirty(object id, IDictionary previousState, IDictionary currentState, NHibernate.Type.IType[] types)
		{
			if (!IsValid(RunWhen.Update))
			{
				OnNotValid();
			}

			return base.OnFlushDirty(id, previousState, currentState, types);
		}

		/// <summary>
		/// Throws an exception explaining why the save or update
		/// cannot be executed when fields are not ok to pass.
		/// </summary>
		/// <remarks>
		/// You can override this method to declare a better behavior.
		/// </remarks>
		protected virtual void OnNotValid()
		{
			throw new ValidationException(
				"Can't save or update as there is one (or more) field that has not passed the validation test",
				ValidationErrorMessages);
		}
	}
}
