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

#if DOTNET2
namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Internal;

	/// <summary>
	/// Extends <see cref="ActiveRecordBase"/> adding automatic validation support.
	/// <seealso cref="ActiveRecordValidationBase.IsValid"/>
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
    public abstract class ActiveRecordValidationBase<T> : ActiveRecordBase<T>, NHibernate.IValidatable where T : class
	{
		/// <summary>
		/// List of validators that should be executed for this class
		/// </summary>
		private ArrayList __validators = new ArrayList();

		private IDictionary __failedProperties;

		/// <summary>
		/// List of error messages
		/// </summary>
		private String[] _errorMessages;

		/// <summary>
		/// Constructs an ActiveRecordValidationBase
		/// </summary>
		public ActiveRecordValidationBase()
		{
			CollectValidators( typeof(T) );
		}

		/// <summary>
		/// Collect the validations applied to this class properties.
		/// </summary>
		/// <param name="targetType"></param>
		private void CollectValidators( Type targetType )
		{
            ActiveRecordModel model = Framework.Internal.ActiveRecordModel.GetModel(targetType);

			if (model == null)
			{
				throw new ActiveRecordException("Seems that the framework wasn't initialized properly. (ActiveRecordModel could not obtained)");
			}

			__validators.AddRange( model.Validators );

			while( model.Parent != null )
			{
				__validators.AddRange( model.Parent.Validators );

				model = model.Parent;
			}
		}

		/// <summary>
		/// Performs the fields validation. Returns true if no 
		/// validation error was found.
		/// </summary>
		/// <returns></returns>
		public virtual bool IsValid()
		{
			ArrayList errorlist = new ArrayList();
			__failedProperties = new Hashtable();

			foreach(IValidator validator in __validators)
			{
				if (!validator.Perform(this))
				{
					String errorMessage = validator.ErrorMessage;
					
					errorlist.Add( errorMessage );

					ArrayList items = null;

					if (__failedProperties.Contains(validator.Property))
					{
						items = (ArrayList) __failedProperties[validator.Property];
					}
					else
					{
						items = new ArrayList();

						__failedProperties[validator.Property] = items;
					}

					items.Add(errorMessage);
				}
			}

			_errorMessages = (String[]) errorlist.ToArray( typeof(String) );

			return errorlist.Count == 0;
		}

		/// <summary>
		/// Returns a list of current validation errors messages.
		/// </summary>
		public String[] ValidationErrorMessages
		{
			get
			{
				if (_errorMessages == null)
				{
					IsValid();
				}

				return _errorMessages;
			}
		}

		/// <summary>
		/// Maps a specific PropertyInfo to a list of
		/// error messages. Useful for frameworks.
		/// </summary>
		public IDictionary PropertiesValidationErrorMessage
		{
			get { return __failedProperties; }
		}

		void NHibernate.IValidatable.Validate()
		{
			if (!IsValid())
			{
				OnNotValid();
			}
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
			throw new ValidationException( "Can't save or update as there is one (or more) field that has not passed the validation test", ValidationErrorMessages );
		}
	}
}
#endif