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

namespace Castle.MonoRail.Views.AspView
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Reflection;
	using System.Xml;

	using Compiler;

	public class AspViewConfigurationSection : IConfigurationSectionHandler
	{
		#region IConfigurationSectionHandler Members

		public object Create(object parent, object configContext, XmlNode section)
		{
			if (section == null)
				throw new AspViewException("AspView config section is missing or not found");

			IEnumerable<ReferencedAssembly> references = GetReferencesFrom(section);

			IDictionary<Type, Type> providers = GetProviders(section);

			AspViewCompilerOptions compilerOptions = GetCompilerOptionsFrom(section, references, providers);

			AspViewEngineOptions options = new AspViewEngineOptions(compilerOptions);

			return options;
		}

		#endregion

		private static IEnumerable<ReferencedAssembly> GetReferencesFrom(XmlNode section)
		{
			List<ReferencedAssembly> references = new List<ReferencedAssembly>();
			XmlNodeList referenceNodes = section.SelectNodes("reference");
			if (referenceNodes == null || referenceNodes.Count == 0)
				return references;

			foreach (XmlNode reference in referenceNodes)
			{
				ReferencedAssembly.AssemblySource source;
				string name = null;
				bool isFromGac = false;
				foreach (XmlAttribute attribute in reference.Attributes)
				{
					switch (attribute.Name.ToLower())
					{
						case "assembly":
							name = attribute.Value;
							break;
						case "isfromgac":
							isFromGac = bool.Parse(attribute.Value);
							break;
						default:
							throw new ConfigurationErrorsException(string.Format("Config error: Unknown attribute [{0}] on reference node, in aspview config section", attribute.Name));
					}
				}

				if (string.IsNullOrEmpty(name))
					throw new ConfigurationErrorsException("Config error: reference must have an assembly name");

				if (isFromGac)
					source = ReferencedAssembly.AssemblySource.GlobalAssemblyCache;
				else
					source = ReferencedAssembly.AssemblySource.BinDirectory;

				references.Add(new ReferencedAssembly(name, source));
			}
			return references;
		}

		private static AspViewCompilerOptions GetCompilerOptionsFrom(XmlNode section, IEnumerable<ReferencedAssembly> references, IDictionary<Type, Type> providers)
		{
			bool? debug = null;
			bool? autoRecompilation = null;
			bool? allowPartiallyTrustedCallers = null;
			string temporarySourceFilesDirectory = null;
			bool? saveFiles = null;

			foreach (XmlAttribute attribute in section.Attributes)
			{
				switch (attribute.Name.ToLower())
				{
					case "debug":
						debug = bool.Parse(attribute.Value);
						break;
					case "autorecompilation":
						autoRecompilation = bool.Parse(attribute.Value);
						break;
					case "allowpartiallytrustedcallers":
						allowPartiallyTrustedCallers = bool.Parse(attribute.Value);
						break;
					case "temporarysourcefilesdirectory":
						temporarySourceFilesDirectory = attribute.Value;
						break;
					case "savefiles":
						saveFiles = bool.Parse(attribute.Value);
						break;
					default:
						throw new ConfigurationErrorsException(string.Format("Config error: Unknown attribute [{0}] in aspview config section", attribute.Name));
				}
			}

			return new AspViewCompilerOptions(
				debug, autoRecompilation, allowPartiallyTrustedCallers, temporarySourceFilesDirectory, saveFiles, references, providers);

		}

		private static IDictionary<Type, Type> GetProviders(XmlNode section)
		{
			XmlNode providersNode = section.SelectSingleNode("providers");

			if (providersNode == null)
				return null;

			IDictionary<Type, Type> providers = new Dictionary<Type, Type>();

			XmlNodeList providerNodes = providersNode.SelectNodes("provider");

			if (providerNodes == null || providerNodes.Count == 0)
				return providers;

			const string serviceAssemblyName = "Castle.MonoRail.Views.AspView";

			foreach (XmlNode providerNode in providerNodes)
			{
				string serviceName = null;
				string typeName = null;
				string implementationAssemblyName = null;
				foreach (XmlAttribute attribute in providerNode.Attributes)
				{
					switch (attribute.Name.ToLower())
					{
						case "name":
							serviceName = attribute.Value;
							break;
						case "type":
							string[] typeParts = attribute.Value.Split(',');
							typeName = typeParts[0].Trim();
							if (typeParts.Length == 2)
								implementationAssemblyName = typeParts[1].Trim();
							break;
						default:
							throw new ConfigurationErrorsException(string.Format("Config error: Unknown attribute [{0}] in aspview config section, provider node.\r\nExpected attributes: [name, type]", attribute.Name));
					}
				}
				Assembly serviceAssembly;
				Assembly implementationAssembly;
				bool isServiceAssemblyLoaded = false;
				try
				{
					serviceAssembly = Assembly.Load(serviceAssemblyName);
					isServiceAssemblyLoaded = true;
					implementationAssembly = Assembly.Load(implementationAssemblyName);
				}
				catch (Exception ex)
				{
					string unloadedAssembly = isServiceAssemblyLoaded ?
						implementationAssemblyName :
						serviceAssemblyName;
					throw new ConfigurationErrorsException(string.Format("Could not load assembly [{0}]", unloadedAssembly), ex);
				}

				Type service = serviceAssembly.GetType(serviceName, false);

				if (service == null)
					throw new AspViewException("Cannot find service [{0}] in assembly [{1}]", serviceName, serviceAssemblyName);

				if (!service.IsInterface)
					throw new AspViewException("Type [{0}] is not an interface", serviceName);

				Type implementation = implementationAssembly.GetType(typeName, false);

				if (implementation == null)
					throw new AspViewException("Cannot find service implementation [{0}] in assembly [{1}]", typeName, implementationAssemblyName);

				if (!service.IsAssignableFrom(implementation))
					throw new AspViewException("Type [{0}] does not implement service interface [{1}]", typeName, serviceName);

				providers.Add(service, implementation);

			}
			return providers;
		}
	}
}
