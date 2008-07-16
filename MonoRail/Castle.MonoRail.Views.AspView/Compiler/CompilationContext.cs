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

namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.IO;

	public class CompilationContext : ICompilationContext
	{
		readonly DirectoryInfo binFolder;
		readonly DirectoryInfo siteRoot;
		readonly DirectoryInfo temporarySourceFilesDirectory;
		readonly DirectoryInfo viewRootDir;

		public CompilationContext(DirectoryInfo binFolder, DirectoryInfo siteRoot, DirectoryInfo viewRootDir, DirectoryInfo temporarySourceFilesDirectory)
		{
			this.binFolder = binFolder;
			this.siteRoot = siteRoot;
			this.temporarySourceFilesDirectory = temporarySourceFilesDirectory;
			this.viewRootDir = viewRootDir;
		}

		public DirectoryInfo BinFolder
		{
			get { return binFolder; }
		}

		public DirectoryInfo ViewRootDir
		{
			get { return viewRootDir; }
		}

		public DirectoryInfo SiteRoot
		{
			get { return siteRoot; }
		}
		
		public DirectoryInfo TemporarySourceFilesDirectory
		{
			get { return temporarySourceFilesDirectory; }
		}

	}
}
