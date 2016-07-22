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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.ComponentModel;
	using System.Globalization;

	public class PhoneConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context,
		                                    Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}

		public override object ConvertFrom(ITypeDescriptorContext context,
		                                   CultureInfo culture, object value)
		{
			if (value is string)
			{
				string[] fields = ((string)value).Split(new char[] { ',' });
				return new Phone(fields[0], fields[1]);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(ITypeDescriptorContext context,
		                                 CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(string))
			{
				return ((Phone)value).Number + "," + ((Phone)value).Extension;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}