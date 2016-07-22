// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	/// Enable access to files on network shares
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Unc")]
	public class UncResource : AbstractStreamResource
	{
		private string basePath;
		private string filePath;

		public UncResource(CustomUri resource)
		{
			CreateStream = delegate
			{
				return CreateStreamFromUri(resource, DefaultBasePath);
			};
		}

		public UncResource(CustomUri resource, String basePath)
		{
			CreateStream = delegate
			{
				return CreateStreamFromUri(resource, basePath);
			};
		}

		public UncResource(String resourceName) : this(new CustomUri(resourceName))
		{
		}

		public UncResource(String resourceName, String basePath) : this(new CustomUri(resourceName), basePath)
		{
		}

		public override String FileBasePath
		{
			get { return basePath; }
		}

		public override IResource CreateRelative(String relativePath)
		{
			return new UncResource(Path.Combine(basePath, relativePath));
		}

		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "UncResource: [{0}] [{1}]", filePath, basePath);
		}

		private Stream CreateStreamFromUri(CustomUri resource, String rootPath)
		{
			if (resource == null)
				throw new ArgumentNullException("resource");
			if (!resource.IsUnc)
				throw new ArgumentException("Resource must be an Unc", "resource");
			if (!resource.IsFile)
				throw new ArgumentException("The specified resource is not a file", "resource");

			String resourcePath = resource.Path;

			if (!File.Exists(resourcePath) && rootPath != null)
			{
				resourcePath = Path.Combine(rootPath, resourcePath);
			}

			filePath = Path.GetFileName(resourcePath);
			basePath = Path.GetDirectoryName(resourcePath);

			CheckFileExists(resourcePath);

			return File.OpenRead(resourcePath);
		}

		private static void CheckFileExists(String path)
		{
			if (!File.Exists(path))
			{
				String message = String.Format(CultureInfo.InvariantCulture, "File {0} could not be found", path);
				throw new ResourceException(message);
			}
		}
	}
}