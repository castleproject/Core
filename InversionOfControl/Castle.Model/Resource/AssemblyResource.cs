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
	using System.Reflection;

	public class AssemblyResource : AbstractStreamResource
	{
		private readonly Stream stream;
		private String basePath;

		public AssemblyResource(CustomUri resource)
		{
			stream = CreateResourceFromUri(resource, null);
		}

		public AssemblyResource(CustomUri resource, String basePath)
		{
			stream = CreateResourceFromUri(resource, basePath);
		}

		public AssemblyResource(String resource)
		{
			stream = CreateResourceFromPath(resource, basePath);
		}

		protected override Stream Stream
		{
			get { return stream; }
		}

		public override IResource CreateRelative(String resourceName)
		{
			throw new NotImplementedException();
		}

		private Stream CreateResourceFromPath(String resource, String path)
		{
			if (!resource.StartsWith("assembly" + CustomUri.SchemeDelimiter))
			{
				resource = "assembly" + CustomUri.SchemeDelimiter + resource;
			}

			return CreateResourceFromUri(new CustomUri(resource), path);
		}

		private Stream CreateResourceFromUri(CustomUri resourcex, String basePath)
		{
			if (resourcex == null) throw new ArgumentNullException("resourcex");

			String assemblyName = resourcex.Host;
			String resourcePath = ConvertToResourceName(assemblyName, resourcex.Path, basePath);

			Assembly assembly = ObtainAssembly(assemblyName);

			String[] names = assembly.GetManifestResourceNames();

			String nameFound = null;

			foreach(String name in names)
			{
				if (String.Compare(resourcePath, name, true) == 0)
				{
					nameFound = name; break;
				}
			}

			if (nameFound == null)
			{
				String message = String.Format("The assembly resource {0} could not be located", resourcePath);
				throw new ResourceException(message);
			}

			this.basePath = ConvertToPath(resourcePath);

			return assembly.GetManifestResourceStream(nameFound);
		}

		private string ConvertToResourceName(String assemblyName, String resource, String basePath)
		{
			// TODO: use basePath for relative name construction
			return String.Format("{0}{1}", assemblyName, resource.Replace('/', '.'));
		}

		private string ConvertToPath(String resource)
		{
			return resource.Replace('.', '/');
		}

		private Assembly ObtainAssembly(String assemblyName)
		{
			try
			{
				return Assembly.Load( assemblyName );
			}
			catch(Exception ex)
			{
				String message = String.Format("The assembly {0} could not be loaded", assemblyName);
				throw new ResourceException(message, ex);
			}
		}
	}
}
