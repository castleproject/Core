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
	/// Specifies the data type the <see cref="RangeValidator"/>
	/// is dealing with.
	/// </summary>
	public enum RangeValidationType
	{
		/// <summary>
		/// <see cref="RangeValidator"/> is dealing with a range of integers
		/// </summary>
		Integer,
		/// <summary>
		/// <see cref="RangeValidator"/> is dealing with a range of longs
		/// </summary>
		Long,
		/// <summary>
		/// <see cref="RangeValidator"/> is dealing with a range of decimals
		/// </summary>
		Decimal,
		/// <summary>
		/// <see cref="RangeValidator"/> is dealing with a range of dates
		/// </summary>
		DateTime,
		/// <summary>
		/// <see cref="RangeValidator"/> is dealing with a range of strings
		/// </summary>
		String
	}

	/// <summary>
	/// Ensures that a property's string representation 
	/// is within the desired value limitations.
	/// </summary>
	[Serializable]
	public class RangeValidator : AbstractValidator
	{
		private object min, max;
		private RangeValidationType type;

		/// <summary>
		/// Initializes an integer-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>int.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>int.MaxValue</c> if this should not be tested.</param>
		public RangeValidator(int min, int max)
		{
			AssertValid(max, min);

			type = RangeValidationType.Integer;
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Initializes an long-based (<see cref="long"/>) range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>long.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>long.MaxValue</c> if this should not be tested.</param>
		public RangeValidator(long min, long max)
		{
			AssertValid(max, min);

			type = RangeValidationType.Long;
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Initializes an decimal-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>decimal.MinValue</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>decimal.MaxValue</c> if this should not be tested.</param>
		public RangeValidator(decimal min, decimal max)
		{
			AssertValid(max, min);

			type = RangeValidationType.Decimal;
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
			AssertValid(max, min);

			type = RangeValidationType.DateTime;
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Initializes a string-based range validator.
		/// </summary>
		/// <param name="min">The minimum value, or <c>String.Empty</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>String.Empty</c> if this should not be tested.</param>
		public RangeValidator(string min, string max)
		{
			AssertValid(max, min);

			type = RangeValidationType.String;
			this.min = min;
			this.max = max;
		}

		/// <summary>
		/// Initializes a range validator of the given type with the given minimum and maximum values.
		/// </summary>
		/// <param name="type">The type of range validator.</param>
		/// <param name="min">The minimum value, or <c>null</c> if this should not be tested.</param>
		/// <param name="max">The maximum value, or <c>null</c> if this should not be tested.</param>
		public RangeValidator(RangeValidationType type, object min, object max)
		{
			this.type = type;
			this.min = GetMinValue(min);
			this.max = GetMaxValue(max);
		}

		/// <summary>
		/// Gets or sets the range validation type for this validator. If the type is changed,
		/// the minimum and maximum values are reset to null-equivalent values (i.e. appropriate
		/// minimum and maximum values for the data type).
		/// </summary>
		public RangeValidationType Type
		{
			get { return type; }
			set
			{
				if (value != type)
				{
					type = value;
					min = GetMinValue(null);
					max = GetMaxValue(null);
				}
			}
		}


		/// <summary>
		/// Internal method that checks a given maximum value's data type and converts
		/// null values to the proper maximum value for the data type.
		/// </summary>
		/// <param name="max">The maximum value to be processed.</param>
		/// <returns>The maximum value with appropriate null-converted minimum values.</returns>
		private object GetMaxValue(object max)
		{
			try
			{
				//check properties for valid types
				switch(type)
				{
					case RangeValidationType.Integer:
						return GetIntValue(max, int.MaxValue);
					case RangeValidationType.Long:
						return GetLongValue(max, int.MaxValue);
					case RangeValidationType.Decimal:
						return GetDecimalValue(max, decimal.MaxValue);
					case RangeValidationType.DateTime:
						return GetDateTimeValue(max, DateTime.MaxValue);
					case RangeValidationType.String:
						return (max == null || String.IsNullOrEmpty(max.ToString()) ? String.Empty : max.ToString());
					default:
						throw new ArgumentException("Unknown RangeValidatorType found.");
				}
			}
			catch(InvalidCastException)
			{
				throw new ArgumentException(
					"RangeValidator's maximum value data type is incompatible with the RangeValidationType specified.");
			}
		}

		/// <summary>
		/// Validate that the property value matches the value requirements.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"></param>
		/// <returns><c>true</c> if the field is OK</returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			if ((fieldValue == null) || (String.IsNullOrEmpty(fieldValue.ToString())))
			{
				return true;
			}

			bool valid = false;

			try
			{
				switch(type)
				{
					case RangeValidationType.Integer:
						int intValue;
						try
						{
							intValue = (int) fieldValue;
							valid = intValue >= (int) min && intValue <= (int) max;
						}
						catch
						{
							if (int.TryParse(fieldValue.ToString(), out intValue))
							{
								valid = intValue >= (int) min && intValue <= (int) max;
							}
						}
						break;
					case RangeValidationType.Long:
						long longValue;
						try
						{
							longValue = (long)fieldValue;
							valid = longValue >= (long)min && longValue <= (long)max;
						}
						catch
						{
							if (long.TryParse(fieldValue.ToString(), out longValue))
							{
								valid = longValue >= (long)min && longValue <= (long)max;
							}
						}
						break;
					case RangeValidationType.Decimal:
						decimal decimalValue;
						try
						{
							decimalValue = (decimal) fieldValue;
							valid = decimalValue >= (decimal) min && decimalValue <= (decimal) max;
						}
						catch
						{
							if (decimal.TryParse(fieldValue.ToString(), out decimalValue))
							{
								valid = decimalValue >= (decimal) min && decimalValue <= (decimal) max;
							}
						}
						break;
					case RangeValidationType.DateTime:
						DateTime dtValue;
						try
						{
							dtValue = (DateTime) fieldValue;
							valid = dtValue >= (DateTime) min && dtValue <= (DateTime) max;
						}
						catch
						{
							if (DateTime.TryParse(fieldValue.ToString(), out dtValue))
							{
								valid = dtValue >= (DateTime) min && dtValue <= (DateTime) max;
							}
						}
						break;
					case RangeValidationType.String:
						string stringValue = fieldValue.ToString();
						string minv = min.ToString();
						string maxv = max.ToString();
						valid = (
						        	(String.IsNullOrEmpty(minv) ||
						        	 String.Compare(stringValue, minv, StringComparison.InvariantCultureIgnoreCase) >= 0)
						        	&&
						        	(String.IsNullOrEmpty(maxv) ||
						        	 String.Compare(stringValue, maxv, StringComparison.InvariantCultureIgnoreCase) <= 0)
						        );
						break;
					default:
						valid = false;
						break;
				}
			}
			catch
			{
			}

			return valid;
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

			switch(type)
			{
				case RangeValidationType.Integer:
					generator.SetValueRange(target, (int) min, (int) max, BuildErrorMessage());
					break;
				case RangeValidationType.Long:
					generator.SetValueRange(target, (long)min, (long)max, BuildErrorMessage());
					break;
				case RangeValidationType.Decimal:
					generator.SetValueRange(target, (decimal) min, (decimal) max, BuildErrorMessage());
					break;
				case RangeValidationType.DateTime:
					generator.SetValueRange(target, (DateTime) min, (DateTime) max, BuildErrorMessage());
					break;
				case RangeValidationType.String:
					generator.SetValueRange(target, (string) min, (string) max, BuildErrorMessage());
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		/// <summary>
		/// Builds the error message.
		/// </summary>
		/// <returns></returns>
		protected override string BuildErrorMessage()
		{
			if (!String.IsNullOrEmpty(ErrorMessage))
			{
				return ErrorMessage;
			}

			if (type == RangeValidationType.DateTime)
			{
				return BuildDateTimeErrorMessage((DateTime) min, (DateTime) max);
			}

			if (type == RangeValidationType.String)
			{
				return BuildStringErrorMessage(min.ToString(), max.ToString());
			}

			if (type == RangeValidationType.Integer)
			{
				return BuildIntegerErrorMessage((int) min, (int) max);
			}

			if (type == RangeValidationType.Long)
			{
				return BuildLongErrorMessage((long)min, (long)max);
			}

			if (type == RangeValidationType.Decimal)
			{
				return BuildDecimalErrorMessage((decimal) min, (decimal) max);
			}

			throw new InvalidOperationException();
		}

		/// <summary>
		/// Gets the error message string for Integer validation
		/// </summary>
		/// <returns>an error message</returns>
		protected string BuildIntegerErrorMessage(int min, int max)
		{
			if (min == int.MinValue && max != int.MaxValue)
			{
				// range against max value only
				return string.Format(GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if (min != int.MinValue && max == int.MaxValue)
			{
				// range against min value only
				return string.Format(GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if (min != int.MinValue || max != int.MaxValue)
			{
				return string.Format(GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets the error message string for long validation
		/// </summary>
		/// <returns>an error message</returns>
		protected string BuildLongErrorMessage(long min, long max)
		{
			if (min == long.MinValue && max != long.MaxValue)
			{
				// range against max value only
				return string.Format(GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if (min != long.MinValue && max == long.MaxValue)
			{
				// range against min value only
				return string.Format(GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if (min != long.MinValue || max != long.MaxValue)
			{
				return string.Format(GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets the error message string for Decimal validation
		/// </summary>
		/// <returns>an error message</returns>
		protected string BuildDecimalErrorMessage(decimal min, decimal max)
		{
			if (min == decimal.MinValue && max != decimal.MaxValue)
			{
				// range against max value only
				return string.Format(GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if (min != decimal.MinValue && max == decimal.MaxValue)
			{
				// range against min value only
				return string.Format(GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if (min != decimal.MinValue || max != decimal.MaxValue)
			{
				return string.Format(GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}


		/// <summary>
		/// Gets the error message string for DateTime validation
		/// </summary>
		/// <returns>an error message</returns>
		protected string BuildDateTimeErrorMessage(DateTime min, DateTime max)
		{
			if (min == DateTime.MinValue && max != DateTime.MaxValue)
			{
				// range against max value only
				return string.Format(GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if (min != DateTime.MinValue && max == DateTime.MaxValue)
			{
				// range against min value only
				return string.Format(GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if (min != DateTime.MinValue || max != DateTime.MaxValue)
			{
				return string.Format(GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Gets the error message string for string validation
		/// </summary>
		/// <returns>an error message</returns>
		protected string BuildStringErrorMessage(string min, string max)
		{
			if (String.IsNullOrEmpty(min) && !String.IsNullOrEmpty(max))
			{
				// range against max value only
				return string.Format(GetString(MessageConstants.RangeTooHighMessage), max);
			}
			else if (!String.IsNullOrEmpty(min) && String.IsNullOrEmpty(max))
			{
				// range against min value only
				return string.Format(GetString(MessageConstants.RangeTooLowMessage), min);
			}
			else if (!String.IsNullOrEmpty(min) || !String.IsNullOrEmpty(max))
			{
				return string.Format(GetString(MessageConstants.RangeTooHighOrLowMessage), min, max);
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

		private static void AssertValid(string max, string min)
		{
			if (String.IsNullOrEmpty(min) && String.IsNullOrEmpty(max))
			{
				throw new ArgumentException(
					"Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}
		}

		private static void AssertValid(int max, int min)
		{
			if (min == int.MinValue && max == int.MaxValue)
			{
				throw new ArgumentException(
					"Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}
			if (min > max)
			{
				throw new ArgumentException("The min parameter must be less than or equal to the max parameter.");
			}
			if (max < min)
			{
				throw new ArgumentException("The max parameter must be greater than or equal to the min parameter.");
			}
		}

		private static void AssertValid(long max, long min)
		{
			if (min == long.MinValue && max == long.MaxValue)
			{
				throw new ArgumentException(
					"Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}
			if (min > max)
			{
				throw new ArgumentException("The min parameter must be less than or equal to the max parameter.");
			}
			if (max < min)
			{
				throw new ArgumentException("The max parameter must be greater than or equal to the min parameter.");
			}
		}

		private static void AssertValid(decimal max, decimal min)
		{
			if (min == decimal.MinValue && max == decimal.MaxValue)
			{
				throw new ArgumentException(
					"Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}
			if (min > max)
			{
				throw new ArgumentException("The min parameter must be less than or equal to the max parameter.");
			}
			if (max < min)
			{
				throw new ArgumentException("The max parameter must be greater than or equal to the min parameter.");
			}
		}

		private static void AssertValid(DateTime max, DateTime min)
		{
			if (min == DateTime.MinValue && max == DateTime.MaxValue)
			{
				throw new ArgumentException(
					"Both min and max were set in such as way that neither would be tested. At least one must be tested.");
			}
			if (min > max)
			{
				throw new ArgumentException("The min parameter must be less than or equal to the max parameter.");
			}
			if (max < min)
			{
				throw new ArgumentException("The max parameter must be greater than or equal to the min parameter.");
			}
		}

		/// <summary>
		/// Internal method that checks a given minimum value's data type and converts
		/// null values to the proper minimum value for the data type.
		/// </summary>
		/// <param name="min">The minimum value to be processed.</param>
		/// <returns>The minimum value with appropriate null-converted minimum values.</returns>
		private object GetMinValue(object min)
		{
			try
			{
				//check properties for valid types
				switch(type)
				{
					case RangeValidationType.Integer:
						return GetIntValue(min, int.MinValue);
					case RangeValidationType.Long:
						return GetLongValue(min, long.MinValue);
					case RangeValidationType.Decimal:
						return GetDecimalValue(min, decimal.MinValue);
					case RangeValidationType.DateTime:
						return GetDateTimeValue(min, DateTime.MinValue);
					case RangeValidationType.String:
						return (min == null || String.IsNullOrEmpty(min.ToString()) ? String.Empty : min.ToString());
					default:
						throw new ArgumentException("Unknown RangeValidatorType found.");
				}
			}
			catch(InvalidCastException)
			{
				throw new ArgumentException(
					"RangeValidator's mininum value data type is incompatible with the RangeValidationType specified.");
			}
		}

		private int GetIntValue(object value, int defaultValue)
		{
			int intValue = defaultValue;
			try
			{
				intValue = (int) value;
			}
			catch
			{
				if (value == null || !int.TryParse(value.ToString(), out intValue))
					value = defaultValue;
			}
			return intValue;
		}

		private long GetLongValue(object value, long defaultValue)
		{
			long longValue = defaultValue;
			try
			{
				longValue = (long)value;
			}
			catch
			{
				if (value == null || !long.TryParse(value.ToString(), out longValue))
					value = defaultValue;
			}
			return longValue;
		}

		private decimal GetDecimalValue(object value, decimal defaultValue)
		{
			decimal decimalValue = defaultValue;
			if (value == null || !decimal.TryParse(value.ToString(), out decimalValue))
				value = defaultValue;
			return decimalValue;
		}

		private DateTime GetDateTimeValue(object value, DateTime defaultValue)
		{
			DateTime dtValue = defaultValue;
			try
			{
				dtValue = (DateTime) value;
			}
			catch
			{
				if (value == null || !DateTime.TryParse(value.ToString(), out dtValue))
					value = defaultValue;
			}
			return dtValue;
		}
	}
}