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
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using System.Xml;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Represents the view engines configuration
	/// </summary>
	public class ViewEngineConfig : ISerializedConfig
	{
		private String viewPathRoot;
		private AssemblySourceInfo[] sources = new AssemblySourceInfo[0];
		private ViewEngineInfo[] viewEngines = new ViewEngineInfo[0];

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified section.
		/// </summary>
		/// <param name="section">The section.</param>
		public void Deserialize(XmlNode section)
		{
			XmlElement engines = (XmlElement) section.SelectSingleNode("viewEngines");

			if (engines != null)
			{
				ConfigureMultipleViewEngines(engines);
			}
			else
			{
				// Backward compatibility
				
				ConfigureSingleViewEngine(section);
			}

			LoadAdditionalSources(section);
			ResolveViewPath();
		}

		#endregion

		/// <summary>
		/// Gets or sets the view path root.
		/// </summary>
		/// <value>The view path root.</value>
		public String ViewPathRoot
		{
			get { return viewPathRoot; }
			set { viewPathRoot = value; }
		}

		/// <summary>
		/// Gets the view engines.
		/// </summary>
		/// <value>The view engines.</value>
		public ViewEngineInfo[] ViewEngines
		{
			get { return viewEngines; }
		}

		/// <summary>
		/// Gets or sets the additional assembly sources.
		/// </summary>
		/// <value>The sources.</value>
		public AssemblySourceInfo[] Sources
		{
			get { return sources; }
			set { sources = value; }
		}

		private void ConfigureMultipleViewEngines(XmlElement engines)
		{
			viewPathRoot = engines.GetAttribute("viewPathRoot");

			if (viewPathRoot == null || viewPathRoot == String.Empty)
			{
				viewPathRoot = "views";
			}

			ArrayList viewEnginesList = new ArrayList();

			foreach (XmlElement addNode in engines.SelectNodes("add"))
			{
				string typeName = addNode.GetAttribute("type");
				string xhtmlVal = addNode.GetAttribute("xhtml");

				if (typeName == null || typeName.Length == 0)
				{
					String message = "The attribute 'type' is required for the element 'add' under 'viewEngines'";
					throw new ConfigurationErrorsException(message);
				}

				Type engine = TypeLoadUtil.GetType(typeName, true);

				if (engine == null)
				{
					String message = "The type '" + typeName + "' could not be loaded";
					throw new ConfigurationErrorsException(message);
				}

				viewEnginesList.Add(new ViewEngineInfo(engine, xhtmlVal == "true"));
			}

			if (viewEnginesList.Count == 0)
			{
				ConfigureDefaultViewEngine();
			}
			else
			{
				viewEngines = new ViewEngineInfo[viewEnginesList.Count];
				viewEnginesList.CopyTo(viewEngines);
			}
		}

		private void ResolveViewPath()
		{
			if (!Path.IsPathRooted(viewPathRoot))
			{
				viewPathRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, viewPathRoot);
			}
		}

		/// <summary>
		/// Configures the default view engine.
		/// </summary>
		private void ConfigureDefaultViewEngine()
		{
			viewPathRoot = "views";

			Type engineType = typeof(Castle.MonoRail.Framework.Views.Aspx.WebFormsViewEngine);

			viewEngines = new ViewEngineInfo[] {new ViewEngineInfo(engineType, false)};
		}

		private void ConfigureSingleViewEngine(XmlNode section)
		{
			section = section.SelectSingleNode("viewEngine");

			if (section == null)
			{
				ConfigureDefaultViewEngine();

				return;
			}

			XmlAttribute viewPath = section.Attributes["viewPathRoot"];

			if (viewPath == null)
			{
				viewPathRoot = "views";
			}
			else
			{
				viewPathRoot = viewPath.Value;
			}

			XmlAttribute xhtmlRendering = section.Attributes["xhtmlRendering"];

			bool enableXhtmlRendering = false;

			if (xhtmlRendering != null)
			{
				try
				{
					enableXhtmlRendering = xhtmlRendering.Value.ToLowerInvariant() == "true";
				}
				catch(FormatException ex)
				{
					String message = "The xhtmlRendering attribute of the views node must be a boolean value.";
					throw new ConfigurationErrorsException(message, ex);
				}
			}

			XmlAttribute customEngineAtt = section.Attributes["customEngine"];

			Type engineType = typeof(Castle.MonoRail.Framework.Views.Aspx.WebFormsViewEngine);

			if (customEngineAtt != null)
			{
				engineType = TypeLoadUtil.GetType(customEngineAtt.Value);
			}

			viewEngines = new ViewEngineInfo[] {new ViewEngineInfo(engineType, enableXhtmlRendering)};

		}

		private void LoadAdditionalSources(XmlNode section)
		{
			ArrayList items = new ArrayList();

			foreach(XmlElement assemblyNode in section.SelectNodes("/monorail/*/additionalSources/assembly"))
			{
				String assemblyName = assemblyNode.GetAttribute("name");
				String ns = assemblyNode.GetAttribute("namespace");

				items.Add(new AssemblySourceInfo(assemblyName, ns));
			}

			sources = (AssemblySourceInfo[]) items.ToArray(typeof(AssemblySourceInfo));
		}
	}
}
