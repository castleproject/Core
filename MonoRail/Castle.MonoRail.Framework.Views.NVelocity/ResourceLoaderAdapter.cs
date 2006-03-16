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

using Resource = NVelocity.Runtime.Resource.Resource;
using ResourceLoader = NVelocity.Runtime.Resource.Loader.ResourceLoader;
using ResourceNotFoundException = NVelocity.Exception.ResourceNotFoundException;

namespace Castle.MonoRail.Framework.Views.NVelocity
{
	using System;
	using System.IO;

	using Commons.Collections;

	public class ResourceLoaderAdapter : ResourceLoader
	{
		private readonly IViewSourceLoader sourceLoader;

		public ResourceLoaderAdapter(IViewSourceLoader sourceLoader)
		{
			this.sourceLoader = sourceLoader;
		}

		internal IViewSource GetViewSource(String templateName)
		{
			if (sourceLoader.HasTemplate(templateName))
			{
				return sourceLoader.GetViewSource(templateName);
			}

			return null;
		}

		public override void Init(ExtendedProperties configuration)
		{
		}

		public override Stream GetResourceStream(string source)
		{
			IViewSource viewSource = GetViewSource(source);

			if (viewSource == null)
			{
				throw new ResourceNotFoundException(String.Format("Resource could not be located {0}", source));
			}

			return viewSource.OpenViewStream();
		}

		public override bool IsSourceModified(Resource resource)
		{
			CustomTemplate template = resource as CustomTemplate;

			if (template != null)
			{
				return template.IsModified;
			}
			
			return false;
		}

		public override long GetLastModified(Resource resource)
		{
			return resource.LastModified;
		}
	}
}
