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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Views;

	/// <summary>
	/// Default <see cref="IViewSourceLoader"/> implementation
	/// that uses the file system and assembly source as source of view templates
	/// </summary>
	public class FileAssemblyViewSourceLoader : IViewSourceLoader, IMRServiceEnabled
	{
		private readonly IList additionalAssemblySources = ArrayList.Synchronized(new ArrayList());
		private readonly IList additionalPathSources = ArrayList.Synchronized(new ArrayList());
		private bool enableCache = true;
		private string viewRootDir;
		private string virtualViewDir;
		private readonly List<FileSystemWatcher> viewFolderWatchers = new List<FileSystemWatcher>();

		private object locker = new object();

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

		#region IMRServiceEnabled implementation

		/// <summary>
		/// Services the specified provider.
		/// </summary>
		/// <param name="provider">The provider.</param>
		public virtual void Service(IMonoRailServices provider)
		{
			IMonoRailConfiguration config = (IMonoRailConfiguration) provider.GetService(typeof(IMonoRailConfiguration));

			if (config != null)
			{
				viewRootDir = config.ViewEngineConfig.ViewPathRoot;
				virtualViewDir = config.ViewEngineConfig.VirtualPathRoot;

				foreach(AssemblySourceInfo sourceInfo in config.ViewEngineConfig.AssemblySources)
				{
					AddAssemblySource(sourceInfo);
				}

				foreach(string pathSource in config.ViewEngineConfig.PathSources)
				{
					AddPathSource(pathSource);
				}
			}
		}

		#endregion

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="sourceName">The template name</param>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasSource(String sourceName)
		{
			if (HasTemplateOnFileSystem(sourceName))
			{
				return true;
			}

			return HasTemplateOnAssemblies(sourceName);
		}

		/// <summary>
		/// Builds and returns a representation of a view template
		/// </summary>
		/// <param name="templateName">The template name</param>
		/// <returns></returns>
		public IViewSource GetViewSource(String templateName)
		{
			FileInfo fileInfo = CreateFileInfo(templateName);

			if (fileInfo != null && fileInfo.Exists)
			{
				return new FileViewSource(fileInfo, enableCache);
			}
			
			return GetStreamFromAdditionalSources(templateName);
		}

		/// <summary>
		/// Gets a list of views on the specified directory
		/// </summary>
		/// <param name="dirName">Directory name</param>
		/// <param name="fileExtensionsToInclude">Optional fileExtensions to include in listing.</param>
		/// <returns></returns>
		public String[] ListViews(String dirName,params string[] fileExtensionsToInclude)
		{
			ArrayList views = new ArrayList();

			CollectViewsOnFileSystem(dirName, views,fileExtensionsToInclude);
			CollectViewsOnAssemblies(dirName, views);

			return (String[]) views.ToArray(typeof(String));
		}

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		/// <value></value>
		public string VirtualViewDir
		{
			get { return virtualViewDir; }
			set { virtualViewDir = value; }
		}

		/// <summary>
		/// Gets/sets the root directory of views, 
		/// obtained from the configuration.
		/// </summary>
		public string ViewRootDir
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
			get { return additionalAssemblySources; }
		}

		/// <summary>
		/// Adds the path source.
		/// </summary>
		/// <param name="pathSource">The path source.</param>
		public void AddPathSource(string pathSource) {
			additionalPathSources.Add(pathSource);
		}
		
		/// <summary>
		/// Adds the assembly source.
		/// </summary>
		/// <param name="assemblySourceInfo">The assembly source info.</param>
		public void AddAssemblySource(AssemblySourceInfo assemblySourceInfo)
		{
			additionalAssemblySources.Add(assemblySourceInfo);
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
				lock (locker)
				{
					//create the watcher if it doesn't exists
					if (viewFolderWatchers.Count == 0)
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
				lock (locker)
				{
					ViewChangedImpl -= value;
					if (ViewChangedImpl == null) //no more subscribers.
					{
						DisposeViewFolderWatch();
					}
				}
			}
		}

		private event FileSystemEventHandler ViewChangedImpl = delegate { };

		private void DisposeViewFolderWatch()
		{
			ViewChangedImpl -= (viewFolderWatcher_Changed);
			foreach(FileSystemWatcher watcher in viewFolderWatchers)
			{
				watcher.Dispose();
			}
			viewFolderWatchers.Clear();
		}

		private void InitViewFolderWatch()
		{
			if (Directory.Exists(ViewRootDir))
			{
				FileSystemWatcher viewFolderWatcher = GetViewFolderWatcher(ViewRootDir);
				viewFolderWatchers.Add(viewFolderWatcher);
			}

			foreach(string path in additionalPathSources)
			{
				if (Directory.Exists(path))
				{
					FileSystemWatcher viewFolderWatcher = GetViewFolderWatcher(path);
					viewFolderWatchers.Add(viewFolderWatcher);
				}
			}
		}

		private FileSystemWatcher GetViewFolderWatcher(string path)
		{
			FileSystemWatcher viewFolderWatcher = new FileSystemWatcher(path);
			viewFolderWatcher.IncludeSubdirectories = true;
			viewFolderWatcher.Changed += (viewFolderWatcher_Changed);
			viewFolderWatcher.Created += (viewFolderWatcher_Changed);
			viewFolderWatcher.Deleted += (viewFolderWatcher_Changed);
			viewFolderWatcher.Renamed += (viewFolderWatcher_Renamed);
			viewFolderWatcher.EnableRaisingEvents = true;
			return viewFolderWatcher;
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
			FileInfo fileInfo = CreateFileInfo(viewRootDir, templateName);

			if (fileInfo.Exists)
				return true;

			foreach (string pathSource in additionalPathSources) {
				fileInfo = CreateFileInfo(pathSource, templateName);
				if (fileInfo.Exists)
					return true;
			}

			return false;
		}

		private FileInfo CreateFileInfo(string templateName) {

			FileInfo fileInfo = CreateFileInfo(viewRootDir, templateName);

			if(fileInfo.Exists)
				return fileInfo;

			foreach (string pathSource in additionalPathSources) {
				fileInfo = CreateFileInfo(pathSource, templateName);
				if (fileInfo.Exists)
					return fileInfo;
			}
			
			return null;
		}

		private static FileInfo CreateFileInfo(string viewRoot,string templateName)
		{
			if (Path.IsPathRooted(templateName))
			{
				templateName = templateName.Substring(Path.GetPathRoot(templateName).Length);
			}

			return new FileInfo(Path.Combine(viewRoot, templateName));
		}

		private bool HasTemplateOnAssemblies(string templateName)
		{
			foreach(AssemblySourceInfo sourceInfo in additionalAssemblySources)
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
			foreach(AssemblySourceInfo sourceInfo in additionalAssemblySources)
			{
				if (sourceInfo.HasTemplate(templateName))
				{
					return new EmbeddedResourceViewSource(templateName, sourceInfo);
				}
			}

			return null;
		}

		private void CollectViewsOnFileSystem(string dirName, ArrayList views,params string[] fileExtensionsToInclude)
		{
			DirectoryInfo dir = new DirectoryInfo(Path.Combine(ViewRootDir, dirName));
			if(!dir.Exists)
			{
				return; //early return
			}

			
			if(fileExtensionsToInclude ==null || fileExtensionsToInclude.Length==0)
			{
				fileExtensionsToInclude = new string[] {".*"};
			}
			
			foreach (string ext in fileExtensionsToInclude)
			{
				foreach (FileInfo file in dir.GetFiles("*" + ext))
				{
					views.Add(Path.Combine(dirName, file.Name));
				}
			}
		}

		private void CollectViewsOnAssemblies(string dirName, ArrayList views)
		{
			foreach(AssemblySourceInfo sourceInfo in additionalAssemblySources)
			{
				sourceInfo.CollectViews(dirName, views);
			}
		}
	}
}
