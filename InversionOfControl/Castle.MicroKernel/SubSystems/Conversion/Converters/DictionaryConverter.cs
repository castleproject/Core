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

	using Castle.Model.Configuration;


	public class DictionaryConverter : AbstractTypeConverter
	{
		public DictionaryConverter()
		{
		}

		public override bool CanHandleType(Type type)
		{
			return (type == typeof(IDictionary) || type == typeof(Hashtable));
		}

		public override object PerformConversion(String value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			System.Diagnostics.Debug.Assert( targetType == typeof(IDictionary) || targetType == typeof(Hashtable) );

			Hashtable dict = new Hashtable();

			String keyType = configuration.Attributes["keyType"];
			Type convertKeyTo = typeof(String);
			
			String valueType = configuration.Attributes["valueType"];
			Type convertValueTo = typeof(String);

			if (keyType != null)
			{
				convertKeyTo = (Type) Context.Composition.PerformConversion( keyType, typeof(Type) );
			}
			if (valueType != null)
			{
				convertValueTo = (Type) Context.Composition.PerformConversion( valueType, typeof(Type) );
			}

			foreach(IConfiguration itemConfig in configuration.Children)
			{
				String keyValue = itemConfig.Attributes["key"];

				if (keyValue == null)
				{
					throw new ConverterException("You must provide a key for the dictionary entry");
				}

				object key = Context.Composition.PerformConversion(keyValue, convertKeyTo);
				object value = Context.Composition.PerformConversion(itemConfig.Value, convertValueTo);

				dict.Add( key, value );
			}

			return dict;
		}
	}
}
