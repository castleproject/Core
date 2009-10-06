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
namespace Castle.Components.Binder
{
	using System;

	internal class ArrayConverter : TypeConverterBase
	{
		private readonly IConverter parent;

		public ArrayConverter(IConverter parent)
		{
			this.parent = parent;
		}

		public override object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			conversionSucceeded = (input != null && input.ToString() == "");
			if (input == null)
			{
				return null;
			}

			Type elemType = desiredType.GetElementType();

			input = ConverterUtil.FixInputForMonoIfNeeded(elemType, input);

			var values = input as Array;
			Array result = Array.CreateInstance(elemType, values.Length);

			for (int i = 0; i < values.Length; i++)
			{
				bool elementConversionSucceeded;

				result.SetValue(parent.Convert(elemType, null, values.GetValue(i), out elementConversionSucceeded), i);

				// if at least one array element get converted 
				// we consider the conversion a success
				if (elementConversionSucceeded)
				{
					conversionSucceeded = true;
				}
			}

			return result;
		}

		public override bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			return IsTypeConvertible(desiredType, inputType, input, out exactMatch) && desiredType.IsArray;
		}
	}
}