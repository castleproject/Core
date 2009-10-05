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


namespace Castle.Components.Binder
{
	using System;
	using System.ComponentModel;

	internal class TypeConverterAdapter:TypeConverterBase
	{
		public override object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			conversionSucceeded = true;

			if (inputType == null)
			{
				inputType = (input != null ? input.GetType() : typeof(String));
			}

			TypeConverter conv = TypeDescriptor.GetConverter(desiredType);

			if (conv != null && conv.CanConvertFrom(inputType))
			{
				try
				{
					return conv.ConvertFrom(input);
				}
				catch (Exception ex)
				{
					String message = String.Format("Conversion error: " +
												   "Could not convert parameter with value '{0}' to {1}", input, desiredType);

					throw new BindingException(message, ex);
				}
			}
			else
			{
				String message = String.Format("Conversion error: " +
											   "Could not convert parameter with value '{0}' to {1}", input, desiredType);

				throw new BindingException(message);
			}
		}

		public override bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			var convertible = false;
			if (input != null && !desiredType.IsInstanceOfType(input))
			{
				TypeConverter conv = TypeDescriptor.GetConverter(desiredType);
				convertible = (conv != null && conv.CanConvertFrom(input.GetType()));
			}
			return IsTypeConvertible(desiredType, inputType, input, out exactMatch) && convertible;
		}
	}
}
