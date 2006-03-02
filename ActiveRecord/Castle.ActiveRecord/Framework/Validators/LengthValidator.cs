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

namespace Castle.ActiveRecord.Framework.Validators
{
	using System;

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
				throw new ArgumentException("Both minLength and maxLength were set in such as way that neither would be tested. At least one must be tested.");
			}

			if (minLength > maxLength)
			{
				throw new ArgumentException("The maxLength parameter must be greater than the minLength parameter.");
			}

			if (minLength != int.MinValue && minLength < 0)
			{
				throw new ArgumentOutOfRangeException("The minLength parameter must be set to either int.MinValue or a non-negative number.");
			}

			if (maxLength < 0)
			{
				throw new ArgumentOutOfRangeException("The maxLength parameter must be set to either int.MaxValue or a non-negative number.");
			}

			this.minLength = minLength;
			this.maxLength = maxLength;
		}

		public override bool Perform(object instance, object fieldValue)
		{
			//Do we have a value to work with?
			if (fieldValue == null)
				return true;

			int length = fieldValue.ToString().Length;

			//Are we in exact mode?
			if (this.exactLength != int.MinValue)
			{
				//Test it
				return (length == this.exactLength);
			}
			else if (this.minLength != int.MinValue || this.maxLength != int.MaxValue) //Are we in range mode?
			{
				//Test it
				if (this.minLength != int.MinValue && length < this.minLength) return false;
				if (this.maxLength != int.MaxValue && length > this.maxLength) return false;

				return true;
			}
			else
			{
				throw new InvalidOperationException();
			}
		}

		protected override string BuildErrorMessage()
		{
			//Are we in exact mode?
			if (this.exactLength != int.MinValue)
			{
				return String.Format("{0} must be {1} characters long.", Property.Name, this.exactLength);
			}
			else if (this.minLength != int.MinValue || this.maxLength != int.MaxValue) //Are we in range mode?
			{
				return String.Format("{0} must be between {1} and {2} characters long.", Property.Name, this.minLength, this.maxLength);
			}
			else
			{
				throw new InvalidOperationException();
			}
		}
	}
}