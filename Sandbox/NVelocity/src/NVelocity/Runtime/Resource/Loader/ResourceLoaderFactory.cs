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

namespace NVelocity.Runtime.Resource.Loader
{
	using System;
	using NVelocity.Util;


	/// <summary>
	/// Factory to grab a template loader.
	/// </summary>
	/// <author><a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	public class ResourceLoaderFactory
	{
		/// <summary>
		/// Gets the loader specified in the configuration file.
		/// </summary>
		/// <returns>TemplateLoader</returns>
		public static ResourceLoader getLoader(RuntimeServices rs, String loaderClassName)
		{
			ResourceLoader loader = null;

			try
			{
				// since properties are parsed into arrays with commas, 
				// something else needed to be used
				loaderClassName = loaderClassName.Replace(';', ',');
				Type loaderType = Type.GetType(loaderClassName);
				Object o = Activator.CreateInstance(loaderType);
				loader = (ResourceLoader) o;

				rs.info("Resource Loader Instantiated: " + loader.GetType().FullName);

				return loader;
			}
			catch (Exception e)
			{
				rs.error("Problem instantiating the template loader.\n" + "Look at your properties file and make sure the\n" + "name of the template loader is correct. Here is the\n" + "error: " + StringUtils.stackTrace(e));
				throw new Exception("Problem initializing template loader: " + loaderClassName + "\nError is: " + StringUtils.stackTrace(e));
			}
		}

	}
}