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

namespace Castle.MonoRail.Framework.Views
{
	using System;
	using System.IO;

	/// <summary>
	/// Pendent
	/// </summary>
	public class FileViewSource : IViewSource
	{
		private readonly FileInfo fileInfo;
		private readonly bool enableCache;
		private long lastUpdated;

		public FileViewSource(FileInfo fileInfo, bool enableCache)
		{
			this.fileInfo = fileInfo;
			this.enableCache = enableCache;

			lastUpdated = LastModified;
		}

		public Stream OpenViewStream()
		{
			lastUpdated = LastModified;

			return fileInfo.OpenRead();
		}

		public bool EnableCache
		{
			get { return enableCache; }
		}

		public long LastUpdated
		{
			get { return lastUpdated; }
			set { lastUpdated = value; }
		}

		public long LastModified
		{
			get { return File.GetLastWriteTime(fileInfo.FullName).Ticks; }
		}
	}
}
