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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections;
	using System.IO;
	
	using Castle.Core;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Views;

	/// <summary>
	/// Pendent
	/// </summary>
	public class FileAssemblyViewSourceLoader : IViewSourceLoader, IServiceEnabledComponent
	{
		private readonly IList additionalSources = new ArrayList();
		private String viewRootDir;
		private bool enableCache = true;
		private FileSystemWatcher viewFolderWatcher;
		
		#region IServiceEnabledComponent implementation

		public void Service(IServiceProvider provider)
		{
			MonoRailConfiguration config = (MonoRailConfiguration) provider.GetService(typeof(MonoRailConfiguration));
			
			if (config != null)
			{
				viewRootDir = config.ViewEngineConfig.ViewPathRoot;
			}
		}

		#endregion

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(String templateName)
		{
			if (HasTemplateOnFileSystem(templateName))
			{
				return true;
			}

			return HasTemplateOnAssemblies(templateName);
		}

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
		/// 
		/// </summary>
		/// <param name="dirName"></param>
		/// <returns></returns>
		public String[] ListViews(String dirName)
		{
			ArrayList views = new ArrayList();

			CollectViewsOnFileSystem(dirName, views);
			CollectViewsOnAssemblies(dirName, views);

			return (String[]) views.ToArray(typeof(String));
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
		/// 
		/// </summary>
		public bool EnableCache
		{
			get { return enableCache; }
			set { enableCache = value; }
		}

		public IList AdditionalSources
		{
			get { return additionalSources; }
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
				lock(this)
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
				lock(this)
				{
					ViewChangedImpl -= value;
					if (ViewChangedImpl == null)//no more subscribers.
					{
						DisposeViewFolderWatch();
					}
				}
			}
		}

		private event FileSystemEventHandler ViewChangedImpl;
		
		private void DisposeViewFolderWatch()
		{
			ViewChangedImpl -= new FileSystemEventHandler(viewFolderWatcher_Changed);
			viewFolderWatcher.Dispose();
		}

		private void InitViewFolderWatch()
		{
			
			viewFolderWatcher = new FileSystemWatcher(ViewRootDir);
			//need all of those because VS likes to play games
			//with the way it is saving files.
			viewFolderWatcher.NotifyFilter = NotifyFilters.CreationTime |
			                                 NotifyFilters.DirectoryName |
			                                 NotifyFilters.FileName |
			                                 NotifyFilters.LastWrite |
			                                 NotifyFilters.Size;
			viewFolderWatcher.IncludeSubdirectories = true;
			viewFolderWatcher.Changed += new FileSystemEventHandler(viewFolderWatcher_Changed);
			viewFolderWatcher.EnableRaisingEvents = true;
		}

		private void viewFolderWatcher_Changed(object sender, FileSystemEventArgs e)
		{
			//shouldn't ever happen, because we make sure that the watcher is only 
			//active when there are subscribers, but checking will prevent possible concrureny issues with it.
			FileSystemEventHandler temp = ViewChangedImpl;
			if (temp != null)
				temp(this, e);
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
