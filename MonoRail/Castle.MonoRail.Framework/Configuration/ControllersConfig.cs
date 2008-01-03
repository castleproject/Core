// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;
	using System.Xml;

	/// <summary>
	/// Represents the controller node configuration
	/// </summary>
	public class ControllersConfig : ISerializedConfig
	{
		private readonly List<string> assemblies = new List<string>();
		private Type customControllerFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllersConfig"/> class.
		/// </summary>
		public ControllersConfig()
		{
		}

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlNode customFactoryNode = section.SelectSingleNode("customControllerFactory");
			
			if (customFactoryNode != null)
			{
				XmlAttribute typeAtt = customFactoryNode.Attributes["type"];
				
				if (typeAtt == null || typeAtt.Value == String.Empty)
				{
					String message = "If the node customControllerFactory is " + 
						"present, you must specify the 'type' attribute";
					throw new ConfigurationErrorsException(message);
				}
				
				String typeName = typeAtt.Value;
				
				customControllerFactory = TypeLoadUtil.GetType(typeName);
			}
			
			XmlNodeList nodeList = section.SelectNodes("controllers/assembly");
			
			foreach(XmlNode node in nodeList)
			{
				assemblies.Add(node.ChildNodes[0].Value);
			}
		}
		
		#endregion

		/// <summary>
		/// Adds an assembly that should be source of controllers.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		public void AddAssembly(Assembly assembly)
		{
			if (assembly == null) throw new ArgumentNullException("assembly");

			assemblies.Add(assembly.FullName);
		}

		/// <summary>
		/// Adds an assembly that should be source of controllers.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		public void AddAssembly(string assemblyName)
		{
			if (assemblyName == null) throw new ArgumentNullException("assemblyName");

			assemblies.Add(assemblyName);
		}

		/// <summary>
		/// Gets or sets the assemblies.
		/// </summary>
		/// <value>The assemblies.</value>
		public List<string> Assemblies
		{
			get { return assemblies; }
		}

		/// <summary>
		/// Gets or sets the custom controller factory.
		/// </summary>
		/// <seealso cref="IControllerFactory"/>
		/// <value>The custom controller factory.</value>
		public Type CustomControllerFactory
		{
			get { return customControllerFactory; }
			set { customControllerFactory = value; }
		}
	}
}
