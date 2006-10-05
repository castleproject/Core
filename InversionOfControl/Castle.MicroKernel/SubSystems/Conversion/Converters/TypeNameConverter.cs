// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Configuration;

	/// <summary>
	/// Convert a type name to a Type instance.
	/// </summary>
	[Serializable]
	public class TypeNameConverter : AbstractTypeConverter
	{
		public override bool CanHandleType(Type type)
		{
			return type == typeof(Type);
		}

		public override object PerformConversion(String value, Type targetType)
		{
			try
			{
				Type type = Type.GetType(value, true, false);

				if (type == null)
				{
					String message = String.Format(
						"Could not convert from '{0}' to {1} - Maybe type could not be found", 
						value, targetType.FullName);
				
					throw new ConverterException(message);
				}

				return type;
			}
			catch(ConverterException)
			{
				throw;
			}
			catch(Exception ex)
			{
				String message = String.Format(
					"Could not convert from '{0}' to {1}.", 
					value, targetType.FullName);

				throw new ConverterException(message, ex);
			}
		}
		
		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			return PerformConversion(configuration.Value, targetType);
		}
	}
}
