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
	public class GenericListConverter : AbstractTypeConverter
	{
		public GenericListConverter()
		{
		}

		public override bool CanHandleType(Type type)
		{
			return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(IList<>) 
					|| type.GetGenericTypeDefinition() == typeof(ICollection<>)
					|| type.GetGenericTypeDefinition() == typeof(List<>)
					|| type.GetGenericTypeDefinition() == typeof(IEnumerable<>));
		}

		public override object PerformConversion(String value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			System.Diagnostics.Debug.Assert( CanHandleType(targetType) );


			String itemType = configuration.Attributes["type"];
			Type convertTo = typeof(String);

			if (itemType != null)
			{
				convertTo = (Type) Context.Composition.PerformConversion( itemType, typeof(Type) );
			}

			IGenericCollectionConverterHelper converterHelper = (IGenericCollectionConverterHelper)
			                                                    Activator.CreateInstance(
			                                                    	typeof(ListHelper<>).MakeGenericType(convertTo), 
			                                                        this);
			return converterHelper.ConvertConfigurationToCollection(configuration);
		}

		private class ListHelper<T> : IGenericCollectionConverterHelper
		{
			GenericListConverter parent;

			public ListHelper(GenericListConverter parent)
			{
				this.parent = parent;
			}

			public object ConvertConfigurationToCollection(IConfiguration configuration)
			{
				List<T> list = new List<T>();
				foreach (IConfiguration itemConfig in configuration.Children)
				{
					T item = (T)this.parent.Context.Composition.PerformConversion(itemConfig.Value, typeof(T));
					list.Add(item);
				}

				return list;
			}
		}
	}
}
#endif