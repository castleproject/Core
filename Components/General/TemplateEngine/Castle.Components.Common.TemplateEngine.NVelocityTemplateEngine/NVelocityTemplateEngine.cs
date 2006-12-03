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

namespace Castle.Components.Common.TemplateEngine.NVelocityTemplateEngine
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Web;

	using Castle.Components.Common.TemplateEngine;
	using Castle.Core.Logging;

	using Commons.Collections;

	using NVelocity;
	using NVelocity.App;
	using NVelocity.Context;
	using NVelocity.Runtime;

	/// <summary>
	/// Implementation of <see cref="ITemplateEngine"/> 
	/// that uses NVelocity
	/// </summary>
	public class NVelocityTemplateEngine : ITemplateEngine, ISupportInitialize
	{
		private VelocityEngine vengine;
		private ILogger log = NullLogger.Instance;
		
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
			this.TemplateDir = templateDir;
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
			set
			{
				if (vengine != null)
					throw new InvalidOperationException("Could not change the TemplateDir after Template Engine initialization.");
				
				templateDir = value;
			}
		}

		/// <summary>
		/// Enable/Disable caching. Default is <c>true</c>
		/// </summary>
		public bool EnableCache
		{
			get { return enableCache; }
			set { enableCache = value; }
		}

		public ILogger Log
		{
			get { return log; }
			set { log = value; }
		}

		/// <summary>
		/// Starts/configure NVelocity based on the properties.
		/// </summary>
		public void BeginInit()
		{
			vengine = new VelocityEngine();
			
			ExtendedProperties props = new ExtendedProperties();

			if (assemblyName != null)
			{
				log.InfoFormat("Initializing NVelocityTemplateEngine component using Assembly: {0}", assemblyName);
				
				props.SetProperty(RuntimeConstants.RESOURCE_LOADER, "assembly");
				props.SetProperty("assembly.resource.loader.class", "NVelocity.Runtime.Resource.Loader.AssemblyResourceLoader;NVelocity");
				props.SetProperty("assembly.resource.loader.cache", EnableCache.ToString().ToLower() );
				props.SetProperty("assembly.resource.loader.assembly", assemblyName);
			}
			else
			{
				String expandedTemplateDir = ExpandTemplateDir(templateDir);
				log.InfoFormat("Initializing NVelocityTemplateEngine component using template directory: {0}", expandedTemplateDir);
				
				FileInfo propertiesFile = new FileInfo(Path.Combine(expandedTemplateDir, "nvelocity.properties"));
				if (propertiesFile.Exists)
				{
					log.Info("Found 'nvelocity.properties' on template dir, loading as base configuration");
					using(Stream stream = propertiesFile.OpenRead())
					{
						props.Load(stream);
					}
				}
				
				props.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");
				props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH, expandedTemplateDir);
				props.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_CACHE, EnableCache.ToString().ToLower() );
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
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");
				
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
		/// Process the template with data from the context.
		/// </summary>
		public bool Process(IDictionary context, String templateName, TextWriter output)
		{
			if (vengine == null)
				throw new InvalidOperationException("Template Engine not yet initialized.");

			Template template = vengine.GetTemplate(templateName);

			template.Merge(CreateContext(context), output);

			return true;
		}

		private IContext CreateContext(IDictionary context)
		{
			return new VelocityContext( new Hashtable(context) );
		}

		private String ExpandTemplateDir(String templateDir)
		{
			log.DebugFormat("Template directory before expansion: {0}", templateDir);
			
			// if nothing to expand, then exit
			if (templateDir == null)
				templateDir = String.Empty;
			
			// expand web application root
			if (templateDir.StartsWith("~/"))
			{
				HttpContext webContext = HttpContext.Current;
				if (webContext != null && webContext.Request != null)
					templateDir = webContext.Server.MapPath(templateDir);
			}
			
			// normalizes the path (including ".." notation, for parent directories)
			templateDir = new DirectoryInfo(templateDir).FullName;

			log.DebugFormat("Template directory after expansion: {0}", templateDir);
			return templateDir;
		}
	}
}
