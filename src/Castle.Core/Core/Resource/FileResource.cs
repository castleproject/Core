// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

namespace Castle.Core.Resource
{
	using System;
	using System.Globalization;
	using System.IO;

	/// <summary>
	/// 
	/// </summary>
	public class FileResource : AbstractStreamResource
	{
		private string filePath;
		private string basePath;

		public FileResource(CustomUri resource)
		{
			CreateStream = delegate
			{
				return CreateStreamFromUri(resource, DefaultBasePath);
			};
		}

		public FileResource(CustomUri resource, string basePath)
		{
			CreateStream = delegate
			{
				return CreateStreamFromUri(resource, basePath);
			};
		}

		public FileResource(string resourceName)
		{
			CreateStream = delegate
			{
				return CreateStreamFromPath(resourceName, DefaultBasePath);
			};
		}

		public FileResource(string resourceName, string basePath)
		{
			CreateStream = delegate
			{
				return CreateStreamFromPath(resourceName, basePath);
			};
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "FileResource: [{0}] [{1}]", filePath, basePath);
		}

		public override string FileBasePath
		{
			get { return basePath; }
		}

		public override IResource CreateRelative(string relativePath)
		{
			return new FileResource(relativePath, basePath);
		}

		private Stream CreateStreamFromUri(CustomUri resource, string rootPath)
		{
			if (resource == null) throw new ArgumentNullException(nameof(resource));
			if (rootPath == null) throw new ArgumentNullException(nameof(rootPath));

			if (!resource.IsFile)
				throw new ArgumentException("The specified resource is not a file", nameof(resource));

			return CreateStreamFromPath(resource.Path, rootPath);
		}

		private Stream CreateStreamFromPath(string resourcePath, string rootPath)
		{
			if (resourcePath == null)
				throw new ArgumentNullException(nameof(resourcePath));
			if (rootPath == null)
				throw new ArgumentNullException(nameof(rootPath));

			if (!Path.IsPathRooted(resourcePath) || !File.Exists(resourcePath))
			{
				// For a relative path, we use the basePath to
				// resolve the full path

				resourcePath = Path.Combine(rootPath, resourcePath);
			}

			CheckFileExists(resourcePath);

			this.filePath = Path.GetFileName(resourcePath);
			this.basePath = Path.GetDirectoryName(resourcePath);

			return File.OpenRead(resourcePath);
		}

		private static void CheckFileExists(string path)
		{
			if (!File.Exists(path))
			{
				string message = string.Format(CultureInfo.InvariantCulture, "File {0} could not be found", new FileInfo(path).FullName);
				throw new ResourceException(message);
			}
		}
	}
}