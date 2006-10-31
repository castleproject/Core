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

namespace CustomTypeConverterSample.TypeConverters
{
	using System;

	using Castle.Core.Configuration;
	using Castle.MicroKernel.SubSystems.Conversion;
	using CustomTypeConverterSample.Components;

	[Serializable]
	public class ServerConfigConverter : AbstractTypeConverter
	{
		public ServerConfigConverter()
		{
		}

		public override bool CanHandleType(Type type)
		{
			return type == typeof(ServerConfig);
		}

		public override object PerformConversion(String value, Type targetType)
		{
			throw new NotImplementedException();
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			ServerConfig config = new ServerConfig();
	
			foreach(IConfiguration childConfig in configuration.Children)
			{
				if (childConfig.Name == "host")
				{
					config.Host = (String) 
						Context.Composition.PerformConversion(childConfig, typeof(String));
				}
				else if (childConfig.Name == "port")
				{
					config.Port = (int) 
						Context.Composition.PerformConversion(childConfig, typeof(int));
				}
				else if (childConfig.Name == "accept")
				{
					config.Accept = (bool) 
						Context.Composition.PerformConversion(childConfig, typeof(bool));
				}
			}

			return config;
		}
	}
}
