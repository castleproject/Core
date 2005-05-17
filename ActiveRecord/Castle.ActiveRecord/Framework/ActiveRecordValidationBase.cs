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

namespace Castle.ActiveRecord
{
	using System;
	using System.Reflection;
	using System.Collections;

	using Castle.ActiveRecord.Framework;

	/// <summary>
	/// 
	/// </summary>
	public abstract class ActiveRecordValidationBase : ActiveRecordBase
	{
		/// <summary>
		/// Just to ensure only one thread creates the validators array
		/// </summary>
		private static readonly object __locker = new object();
		
		/// <summary>
		/// List of validators that should be executed for this class
		/// </summary>
		private IValidator[] __validators;

		/// <summary>
		/// List of error messages
		/// </summary>
		private String[] _errorMessages;

		/// <summary>
		/// Constructs an ActiveRecordValidationBase
		/// </summary>
		public ActiveRecordValidationBase()
		{
			lock(__locker)
			{
				if (__validators == null)
				{
					CollectValidators( this.GetType() );
				}
			}
		}

		/// <summary>
		/// Collect the validations applied to this class properties.
		/// </summary>
		/// <param name="targetType"></param>
		private void CollectValidators( Type targetType )
		{
			ArrayList list = new ArrayList();

			PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public|BindingFlags.Instance);

			foreach(PropertyInfo prop in properties)
			{
				object[] attributes = prop.GetCustomAttributes( typeof(AbstractValidationAttribute), true );
				
				foreach(AbstractValidationAttribute attribute in attributes)
				{
					attribute.Validator.Initialize(prop);

					list.Add( attribute.Validator );
				}
			}

			__validators = (IValidator[]) list.ToArray( typeof(IValidator) );
		}

		/// <summary>
		/// Performs the fields validation. Returns true if no 
		/// validation error was found.
		/// </summary>
		/// <returns></returns>
		public bool IsValid()
		{
			ArrayList errorlist = new ArrayList();

			foreach(IValidator validator in __validators)
			{
				if (!validator.Perform(this))
				{
					errorlist.Add( validator.ErrorMessage );
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
		/// Saves the instance information to the database.
		/// May Create or Update the instance depending 
		/// on whether it has a valid ID.
		/// </summary>
		public override void Save()
		{
			if (IsValid()) 
			{
				base.Save();
			}
			else
			{
				OnNotValid();
			}
		}

		/// <summary>
		/// Creates (Saves) a new instance to the database.
		/// </summary>
		public override void Create()
		{
			if (IsValid()) 
			{
				base.Create();
			}
			else
			{
				OnNotValid();
			}
		}

		/// <summary>
		/// Persists the modification on the instance
		/// state to the database.
		/// </summary>
		public override void Update()
		{
			if (IsValid()) 
			{
				base.Update();
			}
			else
			{
				OnNotValid();
			}
		}

		/// <summary>
		/// Throws an execption explaining why the save or update
		/// cannot be executed while fields aren't ok to pass.
		/// </summary>
		/// <remarks>
		/// You can override this method to declare a better behavior
		/// </remarks>
		protected virtual void OnNotValid()
		{
			throw new ValidationException("Can't save or update as there is one (or more) field that has not passed the validation test");
		}
	}
}
