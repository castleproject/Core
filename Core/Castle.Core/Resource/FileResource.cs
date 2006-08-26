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

namespace Castle.Core.Resource
{
	using System;
	using System.IO;

	/// <summary>
	/// 
	/// </summary>
	public class FileResource : AbstractStreamResource
	{
		private readonly Stream stream;
		private string filePath;
		private String basePath;

		public FileResource(CustomUri resource)
		{
			stream = CreateStreamFromUri(resource, DefaultBasePath);
		}

		public FileResource(CustomUri resource, String basePath)
		{
			stream = CreateStreamFromUri(resource, basePath);
		}

		public FileResource(String resourceName)
		{
			stream = CreateStreamFromPath(resourceName, DefaultBasePath);
		}

		public FileResource(String resourceName, String basePath)
		{
			stream = CreateStreamFromPath(resourceName, basePath);
		}

		public override string ToString()
		{
			return String.Format("FileResource: [{0}] [{1}]", filePath, basePath);
		}

		public override String FileBasePath
		{
			get { return basePath; }
		}

		public override IResource CreateRelative(String resourceName)
		{
			return new FileResource(resourceName, basePath);
		}

		protected override Stream Stream
		{
			get { return stream; }
		}

		private Stream CreateStreamFromUri(CustomUri resource, String basePath)
		{
			if (resource == null) throw new ArgumentNullException("resource");
			if (basePath == null) throw new ArgumentNullException("basePath");

			if (!resource.IsFile) 
				throw new ArgumentException("The specified resource is not a file", "resource");

			return CreateStreamFromPath(resource.Path, basePath);
		}

		private Stream CreateStreamFromPath(String filePath, String basePath)
		{
			if (filePath == null) 
				throw new ArgumentNullException("filePath");
			if (basePath == null) 
				throw new ArgumentNullException("basePath");

			if (!Path.IsPathRooted(filePath) || !File.Exists(filePath))
			{
				// For a relative path, we use the basePath to
				// resolve the full path

				filePath = Path.Combine( basePath, filePath );
			}

			CheckFileExists(filePath);

			this.filePath = Path.GetFileName(filePath);
			this.basePath = Path.GetDirectoryName(filePath);

			return File.OpenRead(filePath);
		}

		private void CheckFileExists(String path)
		{
			if (!File.Exists(path))
			{
				String message = String.Format("File {0} could not be found", new FileInfo(path).FullName);
				throw new ResourceException(message);
			}
		}
	}
}
