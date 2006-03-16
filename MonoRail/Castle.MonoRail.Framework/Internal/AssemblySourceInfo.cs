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

namespace Castle.MonoRail.Framework.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.IO;
	using System.Reflection;

	public class AssemblySourceInfo
	{
		private readonly String assemblyName;
		private readonly String _namespace;
		private readonly IDictionary entries = new HybridDictionary(true);
		private Assembly loadedAssembly;

		public AssemblySourceInfo(string assemblyName, string _namespace)
		{
			this.assemblyName = assemblyName;
			this._namespace = _namespace;

			loadedAssembly = Assembly.Load(assemblyName);

			RegisterEntries();
		}

		public string AssemblyName
		{
			get { return assemblyName; }
		}

		public string Namespace
		{
			get { return _namespace; }
		}

		public bool HasTemplate(String templateName)
		{
			return entries.Contains(NormalizeTemplateName(templateName));
		}

		public Stream GetTemplateStream(String templateName)
		{
			String resourcePath = (String) entries[NormalizeTemplateName(templateName)];

			return loadedAssembly.GetManifestResourceStream(resourcePath);
		}

		public void CollectViews(string dirName, ArrayList views)
		{
			int toStripLength = _namespace.Length;

			dirName = NormalizeTemplateName(dirName);

			String[] names = loadedAssembly.GetManifestResourceNames();
			
			for(int i=0; i < names.Length; i++)
			{
				String name = names[i];

				if (_namespace != null && name.StartsWith(_namespace))
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

				if (name.StartsWith(dirName))
				{
					views.Add(name);
				}
			}
		}

		private String NormalizeTemplateName(string templateName)
		{
			return templateName.Replace('/', '.').Replace('\\', '.');
		}

		private void RegisterEntries()
		{
			int toStripLength = _namespace.Length;
	
			String[] names = loadedAssembly.GetManifestResourceNames();
	
			for(int i=0; i < names.Length; i++)
			{
				String name = names[i];

				if (_namespace != null && name.StartsWith(_namespace))
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