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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;

	/// <summary>
	/// Represents a source of views in an assembly resource.
	/// </summary>
	public class AssemblySourceInfo
	{
		private readonly String assemblyName;
		private readonly String _namespace;
		private readonly IDictionary entries = new HybridDictionary(true);
		private readonly Assembly loadedAssembly;

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblySourceInfo"/> class.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="_namespace">The _namespace.</param>
		public AssemblySourceInfo(string assemblyName, string _namespace)
		{
			this.assemblyName = assemblyName;
			this._namespace = _namespace;

			loadedAssembly = Assembly.Load(assemblyName);

			RegisterEntries();
		}

		/// <summary>
		/// Gets the name of the assembly.
		/// </summary>
		/// <value>The name of the assembly.</value>
		public string AssemblyName
		{
			get { return assemblyName; }
		}

		/// <summary>
		/// Gets the namespace.
		/// </summary>
		/// <value>The namespace.</value>
		public string Namespace
		{
			get { return _namespace; }
		}

		/// <summary>
		/// Determines whether the assembly has the specified template.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <returns>
		/// 	<c>true</c> if the specified exists on the assembly; otherwise, <c>false</c>.
		/// </returns>
		public bool HasTemplate(String templateName)
		{
			return entries.Contains(NormalizeTemplateName(templateName));
		}

		/// <summary>
		/// Gets the template stream.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <returns></returns>
		public Stream GetTemplateStream(String templateName)
		{
			String resourcePath = (String) entries[NormalizeTemplateName(templateName)];

			return loadedAssembly.GetManifestResourceStream(resourcePath);
		}

		/// <summary>
		/// Collects the views on the assembly resource.
		/// </summary>
		/// <param name="dirName">Name of the dir.</param>
		/// <param name="views">The views.</param>
		public void CollectViews(string dirName, ArrayList views)
		{
			int toStripLength = _namespace.Length;

			dirName = NormalizeTemplateName(dirName);

			String[] names = loadedAssembly.GetManifestResourceNames();
			
			for(int i=0; i < names.Length; i++)
			{
				String name = names[i].ToLower(System.Globalization.CultureInfo.InvariantCulture);

				if (name.StartsWith(_namespace.ToLower(System.Globalization.CultureInfo.InvariantCulture)))
				{
					if (name[toStripLength] == '.')
					{
						name = name.Substring(toStripLength + 1);
					}
					else
					{
						name = name.Substring(toStripLength);
					}
				}

				if (name.StartsWith(dirName.ToLower(System.Globalization.CultureInfo.InvariantCulture)))
				{
					views.Add(name);
				}
			}
		}

		/// <summary>
		/// Normalizes the name of the template.
		/// </summary>
		/// <param name="templateName">Name of the template.</param>
		/// <returns></returns>
		private static String NormalizeTemplateName(string templateName)
		{
			return templateName.Replace('/', '.').Replace('\\', '.');
		}

		/// <summary>
		/// Registers the entries.
		/// </summary>
		private void RegisterEntries()
		{
			int toStripLength = _namespace.Length;
	
			String[] names = loadedAssembly.GetManifestResourceNames();
	
			for(int i=0; i < names.Length; i++)
			{
				String name = names[i].ToLower(System.Globalization.CultureInfo.InvariantCulture);

				if (name.StartsWith(_namespace.ToLower(System.Globalization.CultureInfo.InvariantCulture)))
				{
					if (name[toStripLength] == '.')
					{
						name = name.Substring(toStripLength + 1);
					}
					else
					{
						name = name.Substring(toStripLength);
					}
				}

				// Registers the lookup name 
				// associated to the original name
				entries.Add(name, names[i]);
			}
		}
	}
}
