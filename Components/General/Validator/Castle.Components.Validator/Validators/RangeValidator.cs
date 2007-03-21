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

namespace Castle.Components.Validator
{
	using System;
	using System.Collections;

	/// <summary>
	/// Ensures that a property's string representation 
	/// is within the desired value limitations.
	/// </summary>
	[Serializable]
	public class RangeValidator : AbstractValidator
	{
		private object min, max;

		/// <summary>
		/// Initializes an integer-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>int.MaxValue</c> if this should not be tested.</param>
		public RangeValidator(int min, int max)
		{
			if (min == int.MinValue && max == int.MaxValue)
			{
				throw new ArgumentException("Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}

			if (min > max)
			{
				throw new ArgumentException("The min parameter must be less than or equal to the max parameter.");
			}

			if (max < min)
			{
				throw new ArgumentException("The max parameter must be greater than or equal to the min parameter.");
			}

			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Initializes a DateTime-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>DateTime.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>DateTime.MaxValue</c> if this should not be tested.</param>
		public RangeValidator(DateTime min, DateTime max)
		{
			if (min == DateTime.MinValue && max == DateTime.MaxValue)
			{
				throw new ArgumentException("Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}

			if (min > max)
			{
				throw new ArgumentException("The min parameter must be less than or equal to the max parameter.");
			}

			if (max < min)
			{
				throw new ArgumentException("The max parameter must be greater than or equal to the min parameter.");
			}

			this.min = min;
			this.max = max;
		}
		
		/// <summary>
		/// Gets or sets the minimun value to validate.
		/// </summary>
		/// <value>The minimun value to validate.</value>
		public object Min
		{
			get { return min; }
			set { min = value; }
		}

		/// <summary>
		/// Gets or sets the maximum value to validate.
		/// </summary>
		/// <value>The maximum value to validate.</value>
		public object Max
		{
			get { return max; }
			set { max = value; }
		}

		/// <summary>
		/// Validate that the property value matches the value requirements.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if (fieldValue == null) return true;

			try
			{
				// check DateTime
				DateTime dtValue;
				if (DateTime.TryParse(fieldValue.ToString(), out dtValue))
				{
					return dtValue >= (DateTime)min && dtValue <= (DateTime)max;
				}

				// check integer
				int intValue;
				if (int.TryParse(fieldValue.ToString(), out intValue))
				{
					return intValue >= (int)min && intValue <= (int)max;
				}

				// try force-casting as integer, good for enums
				intValue = (int)fieldValue;
				return intValue >= (int)min && intValue <= (int)max;
			}
			catch
			{
			}

			return false;
		}

		/// <summary>
		/// Gets a value indicating whether this validator supports web validation.
		/// </summary>
		/// <value>
		/// 	<see langword="true"/> if web validation is supported; otherwise, <see langword="false"/>.
		/// </value>
		public override bool SupportsWebValidation
		{
			get { return false; }
		}

		/// <summary>
		/// Applies the web validation by setting up one or
		/// more input rules on <see cref="IWebValidationGenerator"/>.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="generator">The generator.</param>
		/// <param name="attributes">The attributes.</param>
		public override void ApplyWebValidation(WebValidationConfiguration config, InputElementType inputType,
												IWebValidationGenerator generator, IDictionary attributes)
		{
			
		}

		/// <summary>
		/// Builds the error message.
		/// </summary>
		/// <returns></returns>
		protected override string BuildErrorMessage()
		{
			if (min is DateTime && max is DateTime)
				return BuildDateTimeErrorMessage((DateTime)min, (DateTime)max);

			//otherwise it must be integer
			if ((int)min == int.MinValue && (int)max != int.MaxValue)
			{
				// range against max value only
				return string.Format(GetResourceForCurrentCulture().GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if ((int)min != int.MinValue && (int)max == int.MaxValue)
			{
				// range against min value only
				return string.Format(GetResourceForCurrentCulture().GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if ((int)min != int.MinValue || (int)max != int.MaxValue)
			{
				return string.Format(GetResourceForCurrentCulture().GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets the error message string for DateTime validation
		/// </summary>
		/// <returns></returns>
		protected string BuildDateTimeErrorMessage(DateTime min, DateTime max)
		{
			if (min == DateTime.MinValue && max != DateTime.MaxValue)
			{
				// range against max value only
				return string.Format(GetResourceForCurrentCulture().GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if (min != DateTime.MinValue && max == DateTime.MaxValue)
			{
				// range against min value only
				return string.Format(GetResourceForCurrentCulture().GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if (min != DateTime.MinValue || max != DateTime.MaxValue)
			{
				return string.Format(GetResourceForCurrentCulture().GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
		
		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <value></value>
		protected override string MessageKey
		{
			get { return MessageConstants.InvalidRangeMessage; }
		}
	}
}
