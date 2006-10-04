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
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using System.Xml;
	
	using Castle.MonoRail.Framework.Internal;

	public class ViewEngineConfig : ISerializedConfig
	{
		private String viewPathRoot;
		private Type customEngine;
		private AssemblySourceInfo[] sources = new AssemblySourceInfo[0];
		private bool enableXhtmlRendering;
		
		#region ISerializedConfig implementation

		public void Deserialize(XmlNode section)
		{
			section = section.SelectSingleNode("viewEngine");
			
			if (section == null)
			{
				String message = "The 'viewEngine' node is not optional";
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}
			
			XmlAttribute viewPath = section.Attributes["viewPathRoot"];

			if (viewPath == null)
			{
				String message = "The 'viewEngine' node must include a " + 
					"'viewPathRoot' attribute indicating the root folder that contains the views";
#if DOTNET2
				throw new ConfigurationErrorsException(message);
#else
				throw new ConfigurationException(message);
#endif
			}

			viewPathRoot = viewPath.Value;

			if (!Path.IsPathRooted(viewPathRoot))
			{
				viewPathRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, viewPathRoot);
			}

			XmlAttribute xhtmlRendering = section.Attributes["xhtmlRendering"];

			if (xhtmlRendering != null)
			{
				try
				{
					enableXhtmlRendering = bool.Parse(xhtmlRendering.Value);
				}
				catch (FormatException ex)
				{
					String message = "The xhtmlRendering attribute of the views node must be a boolean value.";
#if DOTNET2
					throw new ConfigurationErrorsException(message,ex);
#else
					throw new ConfigurationException(message,ex);
#endif
				}
			}

			XmlAttribute customEngineAtt = section.Attributes["customEngine"];

			if (customEngineAtt != null)
			{
				customEngine = TypeLoadUtil.GetType(customEngineAtt.Value);
			}

			ArrayList items = new ArrayList();
			
			foreach(XmlElement assemblyNode in section.SelectNodes("additionalSources/assembly"))
			{
				String assemblyName = assemblyNode.GetAttribute("name");
				String ns = assemblyNode.GetAttribute("namespace");

				items.Add(new AssemblySourceInfo(assemblyName, ns));
			}
			
			sources = (AssemblySourceInfo[]) items.ToArray(typeof(AssemblySourceInfo));
		}
		
		#endregion

		public String ViewPathRoot
		{
			get { return viewPathRoot; }
			set { viewPathRoot = value; }
		}

		public Type CustomEngine
		{
			get { return customEngine; }
			set { customEngine = value; }
		}

		public bool EnableXHtmlRendering
		{
			get { return enableXhtmlRendering; }
			set { enableXhtmlRendering = value; }
		}

		public AssemblySourceInfo[] Sources
		{
			get { return sources; }
			set { sources = value; }
		}
	}
}
