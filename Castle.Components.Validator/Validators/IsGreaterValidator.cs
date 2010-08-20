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
	/// Specifies the data type the <see cref="IsGreaterValidator"/>
	/// is dealing with.
	/// </summary>
	public enum IsGreaterValidationType
	{
		/// <summary>
		/// Value compare as Integer
		/// </summary>
		Integer,

		/// <summary>
		/// Value compare as Decimal
		/// </summary>
		Decimal,

		/// <summary>
		/// Value compare as Date
		/// </summary>
		Date,

		/// <summary>
		/// Value compare as DateTime
		/// </summary>
		DateTime
	}

	/// <summary>
	/// Comparing properties value and make sure it greater than one another.
	/// </summary>
	public class IsGreaterValidator : AbstractCrossReferenceValidator
	{
		#region Private variables

		private readonly IsGreaterValidationType validationType;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a IsGreaterValidator of the given type and target property.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="propertyToCompare"></param>
		public IsGreaterValidator(IsGreaterValidationType type, string propertyToCompare)
			: base(propertyToCompare)
		{
			this.validationType = type;
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the validation type for this validator. 
		/// </summary>
		public IsGreaterValidationType ValidationType
		{
			get { return validationType; }
		}

		#endregion

		#region Object Overrides

		/// <summary>
		/// Validate that the property value greater than the value requirements.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fieldValue"><c>true</c> if the field value is greater than the target property</param>
		/// <returns></returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			object refValue = GetReferenceValue(instance);

			if ((fieldValue == null || fieldValue.ToString() == "") || 
				(refValue == null || refValue.ToString() == ""))
			{
				return true;
			}

			bool valid = false;

			switch (validationType)
			{
				case IsGreaterValidationType.Integer:
					int intRefVal;
					int intFieldValue;

					int.TryParse(refValue.ToString(), out intRefVal);
					int.TryParse(fieldValue.ToString(), out intFieldValue);

					valid = intFieldValue > intRefVal;

					break;

				case IsGreaterValidationType.Decimal:
					decimal decRefVal;
					decimal decFieldVal;

					decimal.TryParse(refValue.ToString(), out decRefVal);
					decimal.TryParse(fieldValue.ToString(), out decFieldVal);

					valid = decFieldVal > decRefVal;

					break;

				case IsGreaterValidationType.DateTime:
				case IsGreaterValidationType.Date:

					DateTime dateRefVal;
					DateTime dateFieldVal;

					DateTime.TryParse(refValue.ToString(), out dateRefVal);
					DateTime.TryParse(fieldValue.ToString(), out dateFieldVal);

					if (validationType == IsGreaterValidationType.DateTime)
					{
						valid = dateFieldVal.CompareTo(dateRefVal) > 0;
					}

					if (validationType == IsGreaterValidationType.Date)
					{
						valid = dateFieldVal.Subtract(dateRefVal).TotalDays > 0;
					}

					break;

				default:
					break;
			}

			return valid;
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
		public override void ApplyBrowserValidation( BrowserValidationConfiguration config, InputElementType inputType,
													IBrowserValidationGenerator generator, IDictionary attributes,
													string target )
		{
			base.ApplyBrowserValidation( config, inputType, generator, attributes, target );

			generator.SetAsGreaterThan( target, PropertyToCompare, validationType, BuildErrorMessage() );
		}

		/// <summary>
		/// Returns the key used to internationalize error messages
		/// </summary>
		/// <value></value>
		protected override string MessageKey
		{
			get { return MessageConstants.GreaterThanMessage; }
		}

		/// <summary>
		/// Browser validation support
		/// </summary>
		public override bool SupportsBrowserValidation
		{
			get { return true; }
		}

		#endregion
	}
}
