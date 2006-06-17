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

	public interface IViewSourceLoader
	{
		void Init(IServiceProvider serviceProvider);

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		bool HasTemplate(String templateName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns></returns>
		IViewSource GetViewSource(String templateName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dirName"></param>
		/// <returns></returns>
		String[] ListViews(String dirName);

		/// <summary>
		/// Gets/sets the root directory of views, obtained from the configuration.
		/// </summary>
		String ViewRootDir { get; set; }

		/// <summary>
		/// 
		/// </summary>
		bool EnableCache { get; set; }

		/// <summary>
		/// 
		/// </summary>
		IList AdditionalSources { get; }

		/// <summary>
		/// Raised when the view is changed.
		/// </summary>
		event FileSystemEventHandler ViewChanged;
	}
}
