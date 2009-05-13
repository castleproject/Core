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
// limitations under the License.using System;

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using Castle.MicroKernel.SubSystems.Conversion;

	/// <summary>
	/// Looks for a <see cref="ConvertibleAttribute"/> on the type to be converted. 
	/// If found, the TypeConverter defined by the attribute is used to perform the conversion.
	/// </summary>
	public class AttributeAwareConverter : AbstractTypeConverter
	{
		#region ITypeConverter Member

		public override bool CanHandleType(Type type)
		{
			ITypeConverter converter = TryGetConverterInstance(type);

			if (converter != null)
			{
				return converter.CanHandleType(type);
			}
			else
			{
				return false;
			}
		}

		public override object PerformConversion(string value, Type targetType)
		{
			ITypeConverter converter = GetConverterInstance(targetType);
			return converter.PerformConversion(value, targetType);
		}

		public override object PerformConversion(Castle.Core.Configuration.IConfiguration configuration, Type targetType)
		{
			ITypeConverter converter = GetConverterInstance(targetType);
			return converter.PerformConversion(configuration, targetType);
		}

		#endregion

		private ITypeConverter TryGetConverterInstance(Type type)
		{
			ITypeConverter converter = null;

			ConvertibleAttribute attr = (ConvertibleAttribute) 
				Attribute.GetCustomAttribute(type, typeof(ConvertibleAttribute));
			
			if (attr != null)
			{
				converter = (ITypeConverter) Activator.CreateInstance(attr.ConverterType);
				converter.Context = Context;
			}

			return converter;
		}

		private ITypeConverter GetConverterInstance(Type type)
		{
			ITypeConverter converter = TryGetConverterInstance(type);
			
			if (converter == null)
			{
				throw new InvalidOperationException("Type " + type.Name + " does not have a Convertible attribute.");
			}

			return converter;
		}
	}
}