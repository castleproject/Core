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
	/// Composition of all available conversion managers
	/// </summary>
	public class DefaultConversionManager : IConversionManager
	{
		private IList _converters;

		public DefaultConversionManager()
		{
			_converters = new ArrayList();

			InitDefaultConverters();
		}

		protected virtual void InitDefaultConverters()
		{
			_converters.Add( new PrimitiveConverter() );
			_converters.Add( new TypeNameConverter() );
		}

		#region ISubSystem Members

		public void Init(IKernel kernel)
		{
		}

		public void Terminate()
		{
		}

		#endregion

		#region IConversionManager Members

		public void Add(ITypeConverter converter)
		{
			_converters.Add(converter);
		}

		#endregion

		#region ITypeConverter Members

		public bool CanHandleType(Type type)
		{
			foreach(ITypeConverter converter in _converters)
			{
				if (converter.CanHandleType(type)) return true;
			}

			return false;
		}

		public object PerformConversion(String value, Type targetType)
		{
			foreach(ITypeConverter converter in _converters)
			{
				if (converter.CanHandleType(targetType)) 
					return converter.PerformConversion(value, targetType);
			}

			String message = String.Format("No converter registered to handle the type {0}", 
				targetType.FullName);

			throw new ConverterException(message);
		}

		#endregion
	}
}
