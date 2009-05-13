// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT

namespace Castle.Windsor.Installer
{
	using Castle.Core.Resource;
	using Castle.Windsor.Configuration.Interpreters;
	
	public static class Configuration
	{
		/// <summary>
		/// Installs all the components from the App.Config file.
		/// </summary>
		/// <returns></returns>
		public static ConfigurationInstaller FromAppConfig()
		{
			return new ConfigurationInstaller(new XmlInterpreter());
		}
		
		/// <summary>
		/// Installs all the component from the xml configuration file.
		/// </summary>
		/// <param name="file">The xml configuration file.</param>
		/// <returns></returns>
		public static ConfigurationInstaller FromXmlFile(string file)
		{
			return new ConfigurationInstaller(new XmlInterpreter(file));
		}

		/// <summary>
		/// Installs all the component from the xml configuration.
		/// </summary>
		/// <param name="resource">The xml configuration resource.</param>
		/// <returns></returns>
		public static ConfigurationInstaller FromXml(IResource resource)
		{
			return new ConfigurationInstaller(new XmlInterpreter(resource));
		}
	}
}

#endif
