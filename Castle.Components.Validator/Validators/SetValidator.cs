// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;

	/// <summary>
	/// Ensures that a property's string representation 
	/// is within a given set of values.
	/// </summary>
	[Serializable]
	public class SetValidator : AbstractValidator
	{
		private string[] setdata;

		/// <summary>
		/// Initializes a set-based validator with an empty set.
		/// </summary>
		public SetValidator()
		{
			setdata = null;
		}

		/// <summary>
		/// Initializes a set-based validator.
		/// </summary>
		/// <param name="set">The set of values to validate against.</param>
		public SetValidator(params string[] set)
		{
			setdata = set;
		}

		/// <summary>
		/// Initializes a set-based validator.
		/// </summary>
		/// <param name="set">The set of values to validate against.</param>
		public SetValidator(params int[] set)
		{
			string[] stringset = new string[set.Length];

			for (int i = 0; i < set.Length; i++)
			{
				stringset[i] = set[i].ToString();
			}

			setdata = stringset;
		}

		/// <summary>
		/// Initializes a set-based validator.
		/// </summary>
		/// <param name="type">The <see cref="System.Type" /> of an <c>enum</c> class.
		/// The enum names will be added to the contents of the set.</param>
		public SetValidator(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("The 'type' parameter can not be 'null'.");
			if (!type.IsEnum)
				throw new ArgumentException("The 'type' parameter must be of type System.Enum");

			setdata = Enum.GetNames(type);
		}

		/// <summary>
		/// Gets or sets the set of values to validate against.
		/// </summary>
		public string[] Set
		{
			get { return setdata; }
			set { setdata = value; }
		}

		/// <summary>
		/// Validate that the property value matches the set requirements.
		/// </summary>
		/// <param name="instance">The target type instance</param>
		/// <param name="fieldValue">The property/field value. It can be null.</param>
		/// <returns><c>true</c> if the value is accepted (has passed the validation test)</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if ((fieldValue == null) || (String.IsNullOrEmpty(fieldValue.ToString())))
				return true;

			// no value can pass if the set is null
			if (setdata == null)
				return false;

			Type fieldType = fieldValue.GetType();
			if (fieldType.IsEnum)
			{
				fieldValue = fieldValue.ToString();

				string[] fieldValues = ((string)fieldValue).Split(new string[] {", "}, StringSplitOptions.RemoveEmptyEntries);
				if (fieldValues.Length > 1)
				{
					foreach (string value in fieldValues)
					{
						if (IsValid(instance, value))
							return true;
					}

					return false;
				}
			}

			try
			{
				bool found = false;
				for(int i = 0; i < setdata.Length; i++)
				{
					if (String.Equals(fieldValue.ToString(), setdata[i]))
					{
						found = true;
						break;
					}
				}

				return found;
			}
			catch
			{
			}

			return false;
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
		                                            IBrowserValidationGenerator generator, IDictionary attributes,
		                                            string target)
		{
		}

		/// <summary>
		/// Builds the error message.
		/// </summary>
		/// <returns></returns>
		protected override string BuildErrorMessage()
		{
			return string.Format(GetString(MessageConstants.InvalidSetMessage));
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <returns></returns>
		protected override string MessageKey
		{
			get { return MessageConstants.InvalidSetMessage; }
		}
	}
}
