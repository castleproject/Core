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

	/// <summary>
	/// Summary description for TypeNameConverter.
	/// </summary>
	public class TypeNameConverter : ITypeConverter
	{
		public TypeNameConverter()
		{
		}

		#region ITypeConverter Members

		public bool CanHandleType(Type type)
		{
			return type == typeof(Type);
		}

		public object PerformConversion(String value, Type targetType)
		{
			Type type = Type.GetType(value, false, false);

			if (type == null)
			{
				String message = String.Format(
					"Could not convert from '{0}' to {1} - Maybe type could not be found", 
					value, targetType.FullName);
				
				throw new ConverterException(message);
			}

			return type;
		}

		#endregion
	}
}
