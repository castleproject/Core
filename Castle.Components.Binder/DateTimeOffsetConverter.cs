// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
// 
#if !MONO
namespace Castle.Components.Binder
{
	using System;

	/// <summary>
	/// The <see cref="DateTimeOffset"/> converter.
	/// </summary>
	internal class DateTimeOffsetConverter : TypeConverterBase
	{
		/// <summary>
		/// Converts the specified desired type.
		/// </summary>
		/// <param name="desiredType">Type of the desired.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="input">The input.</param>
		/// <param name="conversionSucceeded">if set to <c>true</c> [conversion succeeded].</param>
		/// <returns><see cref="DateTimeOffset"/> if conversion is successful, <c>null</c> otherwise.</returns>
		public override object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			conversionSucceeded = input != null;

			if (input == null)
			{
				return null;
			}

			string value = ConverterUtil.NormalizeInput(input);

			if (value == string.Empty)
			{
				conversionSucceeded = false;

				return null;
			}

			return DateTimeOffset.Parse(value);
		}

		/// <summary>
		/// Determines whether this instance can convert the specified desired type.
		/// </summary>
		/// <param name="desiredType">Type of the desired.</param>
		/// <param name="inputType">Type of the input.</param>
		/// <param name="input">The input.</param>
		/// <param name="exactMatch">if set to <c>true</c> [exact match].</param>
		/// <returns>
		/// 	<c>true</c> if this instance can convert the specified desired type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			return IsTypeConvertible(desiredType, inputType, input, out exactMatch) && desiredType == typeof(DateTimeOffset);
		}
	}
}
#endif