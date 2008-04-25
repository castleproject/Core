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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.Components.Validator;
	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// Extends <see cref="ActiveRecordBase"/> adding automatic validation support.
	/// </summary>
	/// <example>
	/// <code>
	/// using Castle.Components.Validator;
	/// 
	/// public class Customer : ActiveRecordBase
	/// {
	///		...
	///		
	///		[Property, ValidateNonEmpty]
	///		public int Name
	///		{
	///			get { return _name; }
	///			set { _name = value; }
	///		}
	///		
	///		[Property, ValidateNonEmpty, ValidateEmail]
	///		public int Email
	///		{
	///			get { return _email; }
	///			set { _email = value; }
	///		}
	///	</code>
	/// </example>
	[Serializable]
	public abstract class ActiveRecordValidationBase : ActiveRecordBase, IValidationProvider 
	{
		/// <summary>
		/// Field for <see cref="ActualValidator"/>.
		/// </summary>
		[NonSerialized]
		private IValidationProvider _actualValidator;

		/// <summary>
		/// Constructs an ActiveRecordValidationBase
		/// </summary>
		protected ActiveRecordValidationBase()
		{
		}


		/// <summary>
		/// Gets the <see cref="IValidationProvider"/> that actually validates this AR object.
		/// Normally returns a <see cref="ActiveRecordValidator"/>, but you can override this
		/// to return a custom validator.
		/// </summary>
		/// <value>The validator.</value>
		[System.Xml.Serialization.XmlIgnore]
		protected virtual IValidationProvider ActualValidator
		{
			get
			{
				if (_actualValidator == null)
				{
					_actualValidator = new ActiveRecordValidator(this);
				}
				return _actualValidator;
			}
		}

		#region IValidationProvider Members

		/// <summary>
		/// Performs the fields validation. Returns true if no 
		/// validation error was found.
		/// </summary>
		/// <returns></returns>
		/// <remarks>Forwards the call to <see cref="ActualValidator"/>.</remarks>
		public virtual bool IsValid()
		{
			return ActualValidator.IsValid();
		}

		/// <summary>
		/// Performs the fields validation for the specified action.
		/// </summary>
		/// <param name="runWhen">Use validators appropriate to the action being performed.</param>
		/// <returns>True if no validation error was found</returns>
		/// <remarks>Forwards the call to <see cref="ActualValidator"/>.</remarks>
		public virtual bool IsValid(RunWhen runWhen)
		{
			return ActualValidator.IsValid(runWhen);
		}

		/// <summary>
		/// Returns a list of current validation errors messages.
		/// </summary>
		/// <remarks>Forwards the call to <see cref="ActualValidator"/>.</remarks>
		public virtual String[] ValidationErrorMessages
		{
			get { return ActualValidator.ValidationErrorMessages; }
		}

		/// <summary>
		/// Maps a specific PropertyInfo to a list of
		/// error messages. Useful for frameworks.
		/// </summary>
		/// <remarks>Forwards the call to <see cref="ActualValidator"/>.</remarks>
		[System.Xml.Serialization.XmlIgnore]
		public virtual IDictionary PropertiesValidationErrorMessages
		{
			get { return ActualValidator.PropertiesValidationErrorMessages; }
		}

		#endregion

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
		/// <remarks>Forwards the call to <see cref="ActualValidator"/>.</remarks>
		protected virtual void OnNotValid()
		{
			ActiveRecordValidator.ThrowNotValidException(ValidationErrorMessages, PropertiesValidationErrorMessages);
		}

	}
}
