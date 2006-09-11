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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Reflection;

	internal class TypeLoadUtil
	{
		private static Assembly thisAssembly = typeof(TypeLoadUtil).Assembly;
		
		public static Type GetType(String typeName)
		{
			return GetType(typeName, false);
		}

		public static Type GetType(String typeName, bool ignoreError)
		{
			Type loadedType = Type.GetType(typeName, false, false);

			if (loadedType == null && !ignoreError)
			{
				throw new ConfigurationException(String.Format("The type {0} could not be found", typeName));
			}
			
			return loadedType;
		}
		
		/// <summary>
		/// Hack to allow MR to work when the main assemblies are on 
		/// GAC. This method returns the complete name.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public static String GetEffectiveTypeName(string typeName)
		{
			String assemblyName = thisAssembly.GetName().Name;
			String assemblyFullName = thisAssembly.GetName().FullName;
			
			return String.Format("{0}{1}", typeName, assemblyFullName.Substring(assemblyName.Length));
		}
	}
}