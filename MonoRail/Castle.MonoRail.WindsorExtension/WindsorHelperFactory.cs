// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.WindsorExtension
{
	using System;
	using System.Collections.Specialized;
	using Castle.MonoRail.Framework.Services;
	using Framework;
	using MicroKernel;

	public class WindsorHelperFactory : DefaultHelperFactory
	{
		private readonly IKernel kernel;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorHelperFactory"/> class.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		public WindsorHelperFactory(IKernel kernel)
		{
			this.kernel = kernel;
		}

		/// <summary>
		/// Creates the helper.
		/// </summary>
		/// <param name="helperType">Type of the helper.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="initialized">if set to <c>true</c> the helper is already initialized.</param>
		/// <returns></returns>
		public override object Create(Type helperType, IEngineContext engineContext, out bool initialized)
		{
			initialized = false;

			if (kernel.HasComponent(helperType))
			{
				HybridDictionary dict = new HybridDictionary(true);
				dict["engineContext"] = engineContext;
				dict["serverUtility"] = engineContext.Server;
				return kernel.Resolve(helperType);
				// return kernel.Resolve(helperType, new { engineContext = engineContext, serverUtility = engineContext.Server });
			}
			else
			{
				return base.Create(helperType, engineContext, out initialized);
			}
		}
	}
}
