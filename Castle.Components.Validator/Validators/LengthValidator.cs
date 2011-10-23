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
	/// is within the desired length limitations.
	/// </summary>
	[Serializable]
	public class LengthValidator : AbstractValidator
	{
		private int exactLength = int.MinValue;
		private int minLength = int.MinValue;
		private int maxLength = int.MaxValue;

		/// <summary>
		/// Initializes a new exact length validator.
		/// </summary>
		/// <param name="exactLength">The exact length required.</param>
		public LengthValidator(int exactLength)
		{
			if (minLength != int.MinValue && minLength < 0)
			{
				throw new ArgumentOutOfRangeException("The exactLength parameter must be set to a non-negative number.");
			}

			this.exactLength = exactLength;
		}

		/// <summary>
		/// Initializes a new range based length validator.
		/// </summary>
		/// <param name="minLength">The minimum length, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="maxLength">The maximum length, or <c>int.MaxValue</c> if this should not be tested.</param>
		public LengthValidator(int minLength, int maxLength)
		{
			if (minLength == int.MinValue && maxLength == int.MaxValue)
			{
				throw new ArgumentException(
					"Both minLength and maxLength were set in such as way that neither would be tested. At least one must be tested.");
			}

			if (minLength > maxLength)
			{
				throw new ArgumentException("The maxLength parameter must be greater than the minLength parameter.");
			}

			if (minLength != int.MinValue && minLength < 0)
			{
				throw new ArgumentOutOfRangeException(
					"The minLength parameter must be set to either int.MinValue or a non-negative number.");
			}

			if (maxLength < 0)
			{
				throw new ArgumentOutOfRangeException(
					"The maxLength parameter must be set to either int.MaxValue or a non-negative number.");
			}

			this.minLength = minLength;
			this.maxLength = maxLength;
		}


		/// <summary>
		/// Gets or sets the exact length to validate.
		/// </summary>
		/// <value>The exact length to validate.</value>
		public int ExactLength
		{
			get { return exactLength; }
			set { exactLength = value; }
		}

		/// <summary>
		/// Gets or sets the minimun length to validate.
		/// </summary>
		/// <value>The minimun length to validate.</value>
		public int MinLength
		{
			get { return minLength; }
			set { minLength = value; }
		}

		/// <summary>
		/// Gets or sets the maximum length to validate.
		/// </summary>
		/// <value>The maximum length to validate.</value>
		public int MaxLength
		{
			get { return maxLength; }
			set { maxLength = value; }
		}

		/// <summary>
		/// Validate that the property value matches the length requirements.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if (fieldValue == null) return true;

			int length = fieldValue.ToString().Length;
			if (length == 0) return true;

			if (exactLength != int.MinValue)
			{
				return (length == exactLength);
			}
			else if (minLength != int.MinValue || maxLength != int.MaxValue)
			{
				if (minLength != int.MinValue && length < minLength) return false;
				if (maxLength != int.MaxValue && length > maxLength) return false;

				return true;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports browser validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if browser validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return true; }
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
			base.ApplyBrowserValidation(config, inputType, generator, attributes, target);

			if (exactLength != int.MinValue)
			{
				string message = string.Format(GetString(MessageConstants.ExactLengthMessage), exactLength);
				generator.SetExactLength(target, exactLength, ErrorMessage ?? message);
			}
			else
			{
				if (minLength != int.MinValue && maxLength != int.MaxValue)
				{
					string message = string.Format(GetString(MessageConstants.LengthInRangeMessage), minLength, maxLength);
					generator.SetLengthRange(target, minLength, maxLength, ErrorMessage ?? message);
				}
				else
				{
					if (minLength != int.MinValue)
					{
						string message = string.Format(GetString(MessageConstants.LengthTooShortMessage), minLength);
						generator.SetMinLength(target, minLength, ErrorMessage ?? message);
					}
					if (maxLength != int.MaxValue)
					{
						string message = string.Format(GetString(MessageConstants.LengthTooLongMessage), maxLength);
						generator.SetMaxLength(target, maxLength, ErrorMessage ?? message);
					}
				}
			}
		}

		/// <summary>
		/// Builds the error message.
		/// </summary>
		/// <returns></returns>
		protected override string BuildErrorMessage()
		{
			if (exactLength != int.MinValue)
			{
				return string.Format(GetString(MessageConstants.ExactLengthMessage), exactLength);
			}
			else if (minLength == int.MinValue && maxLength != int.MaxValue)
			{
				return string.Format(GetString(MessageConstants.LengthTooLongMessage), maxLength);
			}
			else if (minLength != int.MinValue && maxLength == int.MaxValue)
			{
				return string.Format(GetString(MessageConstants.LengthTooShortMessage), minLength);
			}
			else if (minLength != int.MinValue || maxLength != int.MaxValue)
			{
				return
					string.Format(GetString(MessageConstants.LengthInRangeMessage), minLength, maxLength);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
	}
}