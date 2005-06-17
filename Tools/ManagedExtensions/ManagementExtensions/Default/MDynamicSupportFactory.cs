// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Default
{
	using System;
	using System.Configuration;

	/// <summary>
	/// Summary description for MDynamicSupportFactory.
	/// </summary>
	public sealed class MDynamicSupportFactory
	{
		private static InvokerStrategy invokerStrategy;

		private static Object locker = new Object();

		private MDynamicSupportFactory()
		{
		}

		/// <summary>
		/// TODO: Summary
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static MDynamicSupport Create(Object instance)
		{
			lock(locker)
			{
				if (invokerStrategy == null)
				{
					String invokerStrategyName = 
						ConfigurationSettings.AppSettings[MConstants.INVOKER_STRATEGY_CONFIG_KEY];

					if (invokerStrategyName == null || invokerStrategyName.Length == 0)
					{
						invokerStrategyName = typeof(Strategy.ReflectionInvokerStrategy).FullName;
					}

					Type invokerType = Type.GetType(invokerStrategyName);

					invokerStrategy = (InvokerStrategy) AppDomain.CurrentDomain.CreateInstanceAndUnwrap( 
						invokerType.Assembly.FullName, 
						invokerType.FullName );
				}
			}

			return invokerStrategy.Create(instance);
		}
	}
}
