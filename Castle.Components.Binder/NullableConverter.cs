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

	internal class NullableConverter : TypeConverterBase
	{
		private readonly IConverter parent;

		public NullableConverter(IConverter parent)
		{
			this.parent = parent;
		}

		public override object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			Type underlyingType = Nullable.GetUnderlyingType(desiredType);

			object value = parent.Convert(underlyingType, inputType, input, out conversionSucceeded);

			if (conversionSucceeded && value != null)
			{
				Type typeToConstruct = typeof (Nullable<>).MakeGenericType(underlyingType);

				return Activator.CreateInstance(typeToConstruct, value);
			}
			else if (input != null)
			{
				conversionSucceeded = true;
			}
			else
			{
				conversionSucceeded = false;
			}

			return null;
		}

		public override bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			return IsTypeConvertible(desiredType, inputType, input, out exactMatch) &&
			       (desiredType.IsGenericType && desiredType.GetGenericTypeDefinition() == typeof (Nullable<>));
		}
	}
}