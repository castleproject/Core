// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	public class ListConverter : AbstractTypeConverter
	{
		public ListConverter()
		{
		}

		public override bool CanHandleType(Type type)
		{
			return (type == typeof(IList) || type == typeof(ArrayList));
		}

		public override object PerformConversion(String value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			System.Diagnostics.Debug.Assert( targetType == typeof(IList) || targetType == typeof(ArrayList) );

			ArrayList list = new ArrayList();

			String itemType = configuration.Attributes["type"];
			Type convertTo = typeof(String);

			if (itemType != null)
			{
				convertTo = (Type) Context.Composition.PerformConversion( itemType, typeof(Type) );
			}

			foreach(IConfiguration itemConfig in configuration.Children)
			{
				list.Add( Context.Composition.PerformConversion(itemConfig.Value, convertTo) );
			}

			return list;
		}
	}
}
