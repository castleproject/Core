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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using System.Reflection;

	/// <summary>
	/// 
	/// </summary>
	public class DefaultHelperFactory : IHelperFactory
	{
		/// <summary>
		/// Creates the helper.
		/// </summary>
		/// <param name="helperType">Type of the helper.</param>
		/// <param name="engineContext">The engine context.</param>
		/// <param name="initialized">if set to <c>true</c> the helper is already initialized.</param>
		/// <returns></returns>
		public virtual object Create(Type helperType, IEngineContext engineContext, out bool initialized)
		{
			if (helperType == null)
			{
				throw new ArgumentNullException("helperType");
			}
			if (engineContext == null)
			{
				throw new ArgumentNullException("engineContext");
			}

			initialized = false;

			object helper;

			ConstructorInfo constructorInfo = 
				helperType.GetConstructor(BindingFlags.Public, null, new Type[] {typeof(IEngineContext)}, null);

			try
			{
				if (constructorInfo != null)
				{
					helper = Activator.CreateInstance(helperType, new object[] { engineContext });
				}
				else
				{
					helper = Activator.CreateInstance(helperType);
				}
			}
			catch(Exception ex)
			{
				throw new MonoRailException("Could not instantiate helper " + helperType.FullName, ex);
			}

			return helper;
		}
	}
}
