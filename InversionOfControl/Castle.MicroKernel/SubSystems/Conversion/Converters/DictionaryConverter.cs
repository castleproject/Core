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
	using System.Collections;

	using Castle.Model.Configuration;


	[Serializable]
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

			String keyTypeName = configuration.Attributes["keyType"];
			Type defaultKeyType = typeof(String);
			
			String valueTypeName = configuration.Attributes["valueType"];
			Type defaultValueType = typeof(String);

			if (keyTypeName != null)
			{
				defaultKeyType = (Type) Context.Composition.PerformConversion( keyTypeName, typeof(Type) );
			}
			if (valueTypeName != null)
			{
				defaultValueType = (Type) Context.Composition.PerformConversion( valueTypeName, typeof(Type) );
			}

			foreach(IConfiguration itemConfig in configuration.Children)
			{
				// Preparing the key

				String keyValue = itemConfig.Attributes["key"];

				if (keyValue == null)
				{
					throw new ConverterException("You must provide a key for the dictionary entry");
				}

				Type convertKeyTo = defaultKeyType;

				if (itemConfig.Attributes["keyType"] != null)
				{
					convertKeyTo = (Type) Context.Composition.PerformConversion( itemConfig.Attributes["keyType"], typeof(Type) );
				}

				object key = Context.Composition.PerformConversion(keyValue, convertKeyTo);

				// Preparing the value

				Type convertValueTo = defaultValueType;

				if (itemConfig.Attributes["valueType"] != null)
				{
					convertValueTo = (Type) Context.Composition.PerformConversion( itemConfig.Attributes["valueType"], typeof(Type) );
				}
				object value = Context.Composition.PerformConversion(itemConfig.Value, convertValueTo);

				dict.Add( key, value );
			}

			return dict;
		}
	}
}
