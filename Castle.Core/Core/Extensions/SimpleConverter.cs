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

namespace Castle.Core.Extensions
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Globalization;
#if SILVERLIGHT

	public class SimpleConverter : TypeConverter
	{
		public void Register()
		{
			foreach (var key in converters.Keys)
			{
				TypeDescriptor.RegisterConverter(key, this);
			}
		}

		private static readonly Dictionary<Type, Func<string, object>> converters = new Dictionary<Type, Func<string, object>>
		                                                                   	{
		                                                                   		{typeof (int), i => int.Parse(i)},
		                                                                   		{typeof (short), s => short.Parse(s)},
		                                                                   		{typeof (long), l => long.Parse(l)},
#if !SL3 // no Guid.Parse in SL3
		                                                                   		{typeof (Guid), g => Guid.Parse(g)},
#endif
		                                                                   		{typeof (TimeSpan), s => TimeSpan.Parse(s)},
		                                                                   		{typeof (DateTime), t => DateTime.Parse(t)}
		                                                                   	};

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return converters.ContainsKey(destinationType);
		}

		private object Convert(string sourceString, Type propertyType)
		{
			if (sourceString == null)
			{
				return null;
			}
			Func<string, object> converter;
			if (converters.TryGetValue(propertyType, out converter) == false)
			{
				return null;
			}
			return converter.Invoke(sourceString);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
		                                 Type destinationType)
		{
			return Convert(value as string, destinationType);
		}
	}
#endif
}