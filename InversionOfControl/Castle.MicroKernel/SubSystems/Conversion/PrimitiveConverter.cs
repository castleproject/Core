 // Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using System.Collections;

	/// <summary>
	/// Implements all standard conversions.
	/// </summary>
	public class PrimitiveConverter : ITypeConverter
	{
		private Type[] _types;

		public PrimitiveConverter()
		{
			_types = new Type[]
				{
					typeof (Char),
					typeof (DateTime),
					typeof (Decimal),
					typeof (Boolean),
					typeof (Int16),
					typeof (Int32),
					typeof (Int64),
					typeof (UInt16),
					typeof (UInt32),
					typeof (UInt64),
					typeof (Byte),
					typeof (SByte),
					typeof (Single),
					typeof (Double),
					typeof (String)
				};
		}

		public virtual bool CanHandleType(Type type)
		{
			return Array.IndexOf(_types, type) != -1;
		}

		public virtual object PerformConversion(String value, Type targetType)
		{
			if (targetType == typeof(String)) return value;

			try
			{
				return Convert.ChangeType(value, targetType);
			}
			catch(Exception ex)
			{
				String message = String.Format(
					"Could not convert from '{0}' to {1}", 
					value, targetType.FullName);
				
				throw new ConverterException(message, ex);
			}
		}
	}
}