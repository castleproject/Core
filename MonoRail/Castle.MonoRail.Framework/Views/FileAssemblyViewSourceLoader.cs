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
	using System.IO;

	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Views;

	/// <summary>
	/// Default <see cref="IViewSourceLoader"/> implementation
	/// that uses the file system and assembly source as source of view templates
	/// </summary>
	public class FileAssemblyViewSourceLoader : IViewSourceLoader, IServiceEnabledComponent
	{
		private readonly IList additionalSources = ArrayList.Synchronized(new ArrayList());
		private String viewRootDir;
		private bool enableCache = true;
		private FileSystemWatcher viewFolderWatcher;

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public FileAssemblyViewSourceLoader()
		{
		}


		///<summary>
		/// Creates a new instance with the viewRootDir 
		///</summary>
		public FileAssemblyViewSourceLoader(string viewRootDir)
		{
			this.viewRootDir = viewRootDir;
		}

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public void Service(IServiceProvider provider)
		{
			MonoRailConfiguration config = (MonoRailConfiguration)provider.GetService(typeof(MonoRailConfiguration));

			if (config != null)
			{
				viewRootDir = config.ViewEngineConfig.ViewPathRoot;
				
				foreach(AssemblySourceInfo sourceInfo in config.ViewEngineConfig.Sources)
				{
					AddAssemblySource(sourceInfo);
				}
			}
		}

		#endregion

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName">The template name</param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(String templateName)
		{
			if (HasTemplateOnFileSystem(templateName))
			{
				return true;
			}

			return HasTemplateOnAssemblies(templateName);
		}

		/// <summary>
		/// Builds and returns a representation of a view template
		/// </summary>
		/// <param name="templateName">The template name</param>
		/// <returns></returns>
		public IViewSource GetViewSource(String templateName)
		{
			FileInfo fileInfo = CreateFileInfo(templateName);

			if (fileInfo.Exists)
			{
				return new FileViewSource(fileInfo, enableCache);
			}
			else
			{
				return GetStreamFromAdditionalSources(templateName);
			}
		}

		/// <summary>
		/// Gets a list of views on the specified directory
		/// </summary>
		/// <param name="dirName">Directory name</param>
		/// <returns></returns>
		public String[] ListViews(String dirName)
		{
			ArrayList views = new ArrayList();

			CollectViewsOnFileSystem(dirName, views);
			CollectViewsOnAssemblies(dirName, views);

			return (String[])views.ToArray(typeof(String));
		}

		/// <summary>
		/// Gets/sets the root directory of views, 
		/// obtained from the configuration.
		/// </summary>
		public String ViewRootDir
		{
			get { return viewRootDir; }
			set { viewRootDir = value; }
		}

		/// <summary>
		/// Gets or sets whether the instance should use cache
		/// </summary>
		/// <value></value>
		public bool EnableCache
		{
			get { return enableCache; }
			set { enableCache = value; }
		}

		/// <summary>
		/// Gets a list of assembly sources
		/// </summary>
		/// <value></value>
		public IList AssemblySources
		{
			get { return additionalSources; }
		}

		/// <summary>
		/// Adds the assembly source.
		/// </summary>
		/// <param name="assemblySourceInfo">The assembly source info.</param>
		public void AddAssemblySource(AssemblySourceInfo assemblySourceInfo)
		{
			additionalSources.Add(assemblySourceInfo);
		}

		#region Handle File System Changes To Views

		/// <summary>
		/// Raised when the view is changed.
		/// </summary>
		public event FileSystemEventHandler ViewChanged
		{
			add
			{
				//avoid concurrency problems with creating/removing the watcher
				//in two threads in parallel. Unlikely, but better to be safe.
				lock (this)
				{
					//create the watcher if it doesn't exists
					if (viewFolderWatcher == null)
					{
						InitViewFolderWatch();
					}
					ViewChangedImpl += value;
				}
			}
			remove
			{
				//avoid concurrency problems with creating/removing the watcher
				//in two threads in parallel. Unlikely, but better to be safe.
				lock (this)
				{
					ViewChangedImpl -= value;
					if (ViewChangedImpl == null)//no more subscribers.
					{
						DisposeViewFolderWatch();
					}
				}
			}
		}

		private event FileSystemEventHandler ViewChangedImpl = delegate { };

		private void DisposeViewFolderWatch()
		{
			ViewChangedImpl -= new FileSystemEventHandler(viewFolderWatcher_Changed);
			if (viewFolderWatcher != null)
			{
				viewFolderWatcher.Dispose();
			}
		}

		private void InitViewFolderWatch()
		{
			if (Directory.Exists(ViewRootDir))
			{
				viewFolderWatcher = new FileSystemWatcher(ViewRootDir);
				viewFolderWatcher.IncludeSubdirectories = true;
				viewFolderWatcher.Changed += new FileSystemEventHandler(viewFolderWatcher_Changed);
				viewFolderWatcher.Created += new FileSystemEventHandler(viewFolderWatcher_Changed);
				viewFolderWatcher.Deleted += new FileSystemEventHandler(viewFolderWatcher_Changed);
				viewFolderWatcher.Renamed += new RenamedEventHandler(viewFolderWatcher_Renamed);
				viewFolderWatcher.EnableRaisingEvents = true;
			}
		}

		private void viewFolderWatcher_Renamed(object sender, RenamedEventArgs e)
		{
			ViewChangedImpl(this, e);
		}

		private void viewFolderWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			ViewChangedImpl(this, e);
		}

		#endregion

		private bool HasTemplateOnFileSystem(string templateName)
		{
			return CreateFileInfo(templateName).Exists;
		}

		private FileInfo CreateFileInfo(string templateName)
		{
			if (Path.IsPathRooted(templateName))
			{
				templateName = templateName.Substring(Path.GetPathRoot(templateName).Length);
			}

			return new FileInfo(Path.Combine(viewRootDir, templateName));
		}

		private bool HasTemplateOnAssemblies(string templateName)
		{
			foreach(AssemblySourceInfo sourceInfo in additionalSources)
			{
				if (sourceInfo.HasTemplate(templateName))
				{
					return true;
				}
			}

			return false;
		}

		private IViewSource GetStreamFromAdditionalSources(string templateName)
		{
			foreach(AssemblySourceInfo sourceInfo in additionalSources)
			{
				if (sourceInfo.HasTemplate(templateName))
				{
					return new EmbeddedResourceViewSource(templateName, sourceInfo);
				}
			}

			return null;
		}

		private void CollectViewsOnFileSystem(string dirName, ArrayList views)
		{
			DirectoryInfo dir = new DirectoryInfo(Path.Combine(ViewRootDir, dirName));

			if (dir.Exists)
			{
				foreach(FileInfo file in dir.GetFiles("*.*"))
				{
					views.Add(Path.Combine(dirName, file.Name));
				}
			}
		}

		private void CollectViewsOnAssemblies(string dirName, ArrayList views)
		{
			foreach(AssemblySourceInfo sourceInfo in additionalSources)
			{
				sourceInfo.CollectViews(dirName, views);
			}
		}
	}
}
