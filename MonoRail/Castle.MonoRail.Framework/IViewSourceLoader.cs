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
	using System.IO;

	/// <summary>
	/// Defines a contract that abstracts view template locations. 
	/// </summary>
	public interface IViewSourceLoader
	{
		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="sourceName">The template name</param>
		/// <returns><c>true</c> if it exists</returns>
		bool HasSource(String sourceName);

		/// <summary>
		/// Builds and returns a representation of a view template
		/// </summary>
		/// <param name="templateName">The template name</param>
		/// <returns></returns>
		IViewSource GetViewSource(String templateName);

		/// <summary>
		/// Gets a list of views on the specified directory
		/// </summary>
		/// <param name="dirName">Directory name</param>
		/// <returns></returns>
		/// <param name="fileExtensionsToInclude">Optional fileExtensions to include in listing.</param>
        String[] ListViews(String dirName, params string[] fileExtensionsToInclude);

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		string VirtualViewDir { get; set; }

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		string ViewRootDir { get; set; }

		/// <summary>
		/// Gets or sets whether the instance should use cache
		/// </summary>
		bool EnableCache { get; set; }

		/// <summary>
		/// Gets a list of assembly sources
		/// </summary>
		IList AssemblySources { get; }

		/// <summary>
		/// Gets a list of assembly sources
		/// </summary>
		IList PathSources { get; }

		/// <summary>
		/// Adds the assembly source.
		/// </summary>
		/// <param name="assemblySourceInfo">The assembly source info.</param>
		void AddAssemblySource(AssemblySourceInfo assemblySourceInfo);

		/// <summary>
		/// Adds the path source.
		/// </summary>
		/// <param name="pathSource">The path source.</param>
		void AddPathSource(string pathSource);

		/// <summary>
		/// Raised when the view is changed.
		/// </summary>
		event FileSystemEventHandler ViewChanged;
	}
}
