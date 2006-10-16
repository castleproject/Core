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



#if DOTNET2

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Castle.Core.Configuration;


	[Serializable]
	public class GenericDictionaryConverter : AbstractTypeConverter
	{
		public GenericDictionaryConverter() {}

		public override bool CanHandleType(Type type)
		{
			return type.IsGenericType  && (type.GetGenericTypeDefinition() == typeof (IDictionary<,>) || type.GetGenericTypeDefinition() == typeof (Dictionary<,>));
		}

		public override object PerformConversion(String value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			System.Diagnostics.Debug.Assert(CanHandleType(targetType), "Got a type we can't handle!");


			String keyTypeName = configuration.Attributes["keyType"];
			Type defaultKeyType = typeof (String);

			String valueTypeName = configuration.Attributes["valueType"];
			Type defaultValueType = typeof (String);

			if (keyTypeName != null)
			{
				defaultKeyType = (Type) Context.Composition.PerformConversion(keyTypeName, typeof (Type));
			}
			if (valueTypeName != null)
			{
				defaultValueType = (Type) Context.Composition.PerformConversion(valueTypeName, typeof (Type));
			}
			IGenericCollectionConverterHelper collectionConverterHelper = (IGenericCollectionConverterHelper) Activator.CreateInstance(typeof (DictionaryHelper<,>).MakeGenericType(defaultKeyType, defaultValueType), this);
			return collectionConverterHelper.ConvertConfigurationToCollection(configuration);
		}

		private class DictionaryHelper<TKey, TValue> : IGenericCollectionConverterHelper
		{
			GenericDictionaryConverter parent;

			public DictionaryHelper(GenericDictionaryConverter parent)
			{
				this.parent = parent;
			}

			public object ConvertConfigurationToCollection(IConfiguration configuration)
			{
				Dictionary<TKey, TValue> dict = new Dictionary<TKey, TValue>();

				foreach (IConfiguration itemConfig in configuration.Children)
				{
					// Preparing the key

					String keyValue = itemConfig.Attributes["key"];

					if (keyValue == null)
					{
						throw new ConverterException("You must provide a key for the dictionary entry");
					}

					Type convertKeyTo = typeof (TKey);

					if (itemConfig.Attributes["keyType"] != null)
					{
						convertKeyTo = (Type) parent.Context.Composition.PerformConversion(itemConfig.Attributes["keyType"], typeof (Type));
					}

					if (!typeof (TKey).IsAssignableFrom(convertKeyTo))
					{
						throw new ArgumentException(string.Format("Could not create dictionary<{0},{1}> because {2} is not assignmable to key type {0}", typeof (TKey), typeof (TValue), convertKeyTo));
					}

					TKey key = (TKey) parent.Context.Composition.PerformConversion(keyValue, convertKeyTo);

					// Preparing the value

					Type convertValueTo = typeof (TValue);

					if (itemConfig.Attributes["valueType"] != null)
					{
						convertValueTo = (Type) parent.Context.Composition.PerformConversion(itemConfig.Attributes["valueType"], typeof (Type));
					}

					if (!typeof (TValue).IsAssignableFrom(convertValueTo))
					{
						throw new ArgumentException(string.Format("Could not create dictionary<{0},{1}> because {2} is not assignmable to value type {1}", typeof (TKey), typeof (TValue), convertValueTo));
					}
					TValue value = (TValue) parent.Context.Composition.PerformConversion(itemConfig.Value, convertValueTo);

					dict.Add(key, value);
				}
				return dict;
			}
		}
	}
}

#endif