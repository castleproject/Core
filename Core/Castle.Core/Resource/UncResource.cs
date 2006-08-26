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

namespace Castle.Model.Resource
{
	using System;
	using System.IO;

	/// <summary>
	/// Enable access to files on network shares
	/// </summary>
	public class UncResource : AbstractStreamResource
	{
		private readonly Stream stream;
		private String basePath;
		private string filePath;

		public UncResource(CustomUri resource)
		{
			stream = CreateStreamFromUri(resource, DefaultBasePath);
		}

		public UncResource(CustomUri resource, String basePath)
		{
			stream = CreateStreamFromUri(resource, basePath);
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

		public override IResource CreateRelative(String resourceName)
		{
			return new UncResource( Path.Combine(basePath, resourceName) );
		}

		public override string ToString()
		{
			return String.Format("UncResource: [{0}] [{1}]", filePath, basePath);
		}

		protected override Stream Stream
		{
			get { return stream; }
		}

		private Stream CreateStreamFromUri(CustomUri resource, String basePath)
		{
			if (resource == null) 
				throw new ArgumentNullException("resource");
			if (!resource.IsUnc) 
				throw new ArgumentException("Resource must be an Unc", "resource");
			if (!resource.IsFile) 
				throw new ArgumentException("The specified resource is not a file", "resource");

			String filePath = resource.Path;

			if (!File.Exists(filePath) && basePath != null)
			{
				filePath = Path.Combine( basePath, filePath );
			}

			this.filePath = Path.GetFileName(filePath);
			this.basePath = Path.GetDirectoryName(filePath);
			
			CheckFileExists(filePath);

			return File.OpenRead(filePath);
		}

		private void CheckFileExists(String path)
		{
			if (!File.Exists(path))
			{
				String message = String.Format("File {0} could not be found", path);
				throw new ResourceException(message);
			}
		}
	}
}
