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

using NVelocity;
using NVelocity.App;
using NVelocity.Context;
using NVelocity.Runtime;

namespace Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine
{
	using System;
	using System.Collections;
	using System.IO;
	using System.ComponentModel;

	using Castle.Components.Common.TemplateEngine;

	using Commons.Collections;

	/// <summary>
	/// Implementation of <see cref="ITemplateEngine"/> 
	/// that uses NVelocity
	/// </summary>
	public class NVelocityTemplateEngine : ITemplateEngine, ISupportInitialize
	{
		private readonly VelocityEngine vengine = new VelocityEngine();
		
		private String assemblyName;
		private String templateDir = ".";
		private bool enableCache = true;

		/// <summary>
		/// Constructs a NVelocityTemplateEngine instance
		/// assuming the default values
		/// </summary>
		public NVelocityTemplateEngine()
		{
		}

		/// <summary>
		/// Constructs a NVelocityTemplateEngine instance
		/// specifing the template directory
		/// </summary>
		/// <param name="templateDir"></param>
		public NVelocityTemplateEngine(String templateDir)
		{
			this.templateDir = templateDir;
		}

		/// <summary>
		/// Gets or sets the assembly name. This
		/// forces NVelocityTemplateEngine to use an assembly resource loader
		/// instead of File resource loader (which is the default) 
		/// </summary>
		public string AssemblyName
		{
			get { return assemblyName; }
			set { assemblyName = value; }
		}

		/// <summary>
		/// Gets or sets the template directory
		/// </summary>
		public string TemplateDir
		{
			get { return templateDir; }
			set { templateDir = value; }
		}

		/// <summary>
		/// Enable/Disable caching. Default is <c>true</c>
		/// </summary>
		public bool EnableCache
		{
			get { return enableCache; }
			set { enableCache = value; }
		}

		/// <summary>
		/// Starts/configure NVelocity based on the properties.
		/// </summary>
		public void BeginInit()
		{
			ExtendedProperties props = new ExtendedProperties();

			if (assemblyName != null)
			{
				props.SetProperty(RuntimeConstants_Fields.RESOURCE_LOADER, "assembly");
				props.SetProperty("assembly.resource.loader.class", "NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader;NVelocity");
				props.SetProperty("assembly.resource.loader.cache", EnableCache.ToString().ToLower() );
				props.SetProperty("assembly.resource.loader.assembly", assemblyName);
			}
			else
			{
				props.SetProperty(RuntimeConstants_Fields.RESOURCE_LOADER, "file");
				props.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_PATH, templateDir);
				props.SetProperty(RuntimeConstants_Fields.FILE_RESOURCE_LOADER_CACHE, EnableCache.ToString().ToLower() );
			}

			vengine.Init(props);
		}

		public void EndInit()
		{
		}

		/// <summary>
		/// Returns <c>true</c> only if the 
		/// specified template exists and can be used
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns></returns>
		public bool HasTemplate(String templateName)
		{
			try
			{
				vengine.GetTemplate(templateName);
				
				return true;
			}
			catch(Exception)
			{
				return false;
			}
		}

		/// <summary>
		/// Implementors should process the template with
		/// data from the context.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="templateName"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		public bool Process(IDictionary context, String templateName, TextWriter output)
		{
			Template template = vengine.GetTemplate(templateName);

			template.Merge(CreateContext(context), output);

			return true;
		}

		private IContext CreateContext(IDictionary context)
		{
			return new VelocityContext( new Hashtable(context) );
		}
	}
}
