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
		private static readonly Dictionary<Type, Func<string, object>> converters = new Dictionary<Type, Func<string, object>>
		                                                                            	{
		                                                                            		{typeof (int), i => int.Parse(i)},
		                                                                            		{typeof (short), s => short.Parse(s)},
		                                                                            		{typeof (long), l => long.Parse(l)},
		                                                                            		{typeof (float), f => float.Parse(f)},
		                                                                            		{typeof (double), d => double.Parse(d)},
		                                                                            		{typeof (decimal), d => decimal.Parse(d)},
		                                                                            		{typeof (Guid), g => new Guid(g)},
		                                                                            		{
		                                                                            			typeof (TimeSpan),
		                                                                            			s => TimeSpan.Parse(s)
		                                                                            			},
		                                                                            		{
		                                                                            			typeof (DateTime),
		                                                                            			t => DateTime.Parse(t)
		                                                                            			}
		                                                                            	};

		private readonly Func<string, object> conversionFunction;
		private readonly Type type;

		public SimpleConverter(Type type, Func<string, object> conversionFunction)
		{
			this.type = type;
			this.conversionFunction = conversionFunction;
		}

		public static void Register()
		{
			foreach (var key in converters)
			{
				var converter = new SimpleConverter(key.Key, key.Value);
				TypeDescriptor.RegisterConverter(key.Key, converter);
				TypeDescriptor.RegisterConverter(typeof (Nullable<>).MakeGenericType(key.Key), converter);
			}
		}

		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType == typeof (string);
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType.IsAssignableFrom(type);
		}

		private object Convert(string sourceString)
		{
			if (sourceString == null)
			{
				return null;
			}
			return conversionFunction.Invoke(sourceString);
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return Convert(value as string);
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value,
		                                 Type destinationType)
		{
			return Convert(value as string);
		}
	}
#endif
}