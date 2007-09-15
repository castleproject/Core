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

namespace Castle.MonoRail.Framework.Views
{
	using System.IO;

	/// <summary>
	/// Represents a view template source on the file system.
	/// </summary>
	public class FileViewSource : IViewSource
	{
		private readonly FileInfo fileInfo;
		private readonly bool enableCache;
		private long lastUpdated;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileViewSource"/> class.
		/// </summary>
		/// <param name="fileInfo">The file info.</param>
		/// <param name="enableCache">if set to <c>true</c> [enable cache].</param>
		public FileViewSource(FileInfo fileInfo, bool enableCache)
		{
			this.fileInfo = fileInfo;
			this.enableCache = enableCache;

			lastUpdated = LastModified;
		}

		/// <summary>
		/// Opens the view stream.
		/// </summary>
		/// <returns></returns>
		public Stream OpenViewStream()
		{
			lastUpdated = LastModified;

			return fileInfo.OpenRead();
		}

		/// <summary>
		/// Gets a value indicating whether cache is enabled for it.
		/// </summary>
		/// <value><c>true</c> if cache is enabled for it; otherwise, <c>false</c>.</value>
		public bool EnableCache
		{
			get { return enableCache; }
		}

		/// <summary>
		/// Gets or sets the last updated.
		/// </summary>
		/// <value>The last updated.</value>
		public long LastUpdated
		{
			get { return lastUpdated; }
			set { lastUpdated = value; }
		}

		/// <summary>
		/// Gets the last modified.
		/// </summary>
		/// <value>The last modified.</value>
		public long LastModified
		{
			get { return File.GetLastWriteTime(fileInfo.FullName).Ticks; }
		}
	}
}
