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
	/// Ensures that a property's string representation validates as an ABN.
	/// See: http://en.wikipedia.org/wiki/ABN
	/// </summary>
	[Serializable]
	public class AustralianBusinessNumberValidator : AbstractValidator
	{

		public override bool Perform(object instance, object fieldValue)
		{
			//If the input is null then there's nothing to validate here
			if (fieldValue == null)
				return true;

			//Get the raw string
			string abnString = fieldValue.ToString();

			//Strip any spaces or dashes
			int[] abnNumber = new int[11];
			int counter = 0;
			foreach (char digit in abnString.ToCharArray())
			{
				if (char.IsNumber(digit))
				{
					//Keep the number
					abnNumber[counter++] = digit - '0';
				}
				else if (digit == ' ')
				{
					//Skip the space
				}
				else
				{
					//If it's not one of the above then it shouldn't be here
					return false;
				}
			}
			
			if (counter != 11) return false;

			//Reduce the first digit by 1
			abnNumber[0]--;

			//Apply the weightings and sum the resultant
			int[] weight = { 10, 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
			int checkSum = 0;
			for (counter = 0; counter < 11; counter++)
				checkSum += weight[counter] * abnNumber[counter];

			//Confirm that the checksum is perfectly divisible by 89
			return (checkSum % 89 == 0);
		}


		protected override string BuildErrorMessage()
		{
			return String.Format("{0} does not appear to be a valid Australian Business Number.", Property.Name);
		}
	}
}