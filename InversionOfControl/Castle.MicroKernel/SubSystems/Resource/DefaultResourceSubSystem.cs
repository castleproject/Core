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

namespace Castle.MicroKernel.SubSystems.Resource
{
	using System;
	using System.Collections;

	using Castle.Core.Resource;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultResourceSubSystem : AbstractSubSystem, IResourceSubSystem
	{
		private readonly IList resourceFactories = new ArrayList();

		public DefaultResourceSubSystem()
		{
			InitDefaultResourceFactories();
		}

		protected virtual void InitDefaultResourceFactories()
		{
			RegisterResourceFactory( new AssemblyResourceFactory() );
			RegisterResourceFactory( new FileResourceFactory() );
			RegisterResourceFactory( new UncResourceFactory() );
			RegisterResourceFactory( new ConfigResourceFactory() );
		}

		public void RegisterResourceFactory(IResourceFactory resourceFactory)
		{
			if (resourceFactory == null) throw new ArgumentNullException("resourceFactory");

			resourceFactories.Add( resourceFactory );
		}

		public IResource CreateResource(String resource)
		{
			if (resource == null) throw new ArgumentNullException("resource");

			return CreateResource(new CustomUri(resource));
		}

		public IResource CreateResource(String resource, String basePath)
		{
			if (resource == null) throw new ArgumentNullException("resource");

			return CreateResource(new CustomUri(resource), basePath);
		}

		public IResource CreateResource(CustomUri uri)
		{
			if (uri == null) throw new ArgumentNullException("uri");

			foreach(IResourceFactory resFactory in resourceFactories)
			{
				if (resFactory.Accept(uri))
				{
					return resFactory.Create(uri);
				}
			}

			throw new KernelException("No Resource factory was able to " + 
				"deal with Uri " + uri.ToString());
		}

		public IResource CreateResource(CustomUri uri, String basePath)
		{
			if (uri == null) throw new ArgumentNullException("uri");
			if (basePath == null) throw new ArgumentNullException("basePath");

			foreach(IResourceFactory resFactory in resourceFactories)
			{
				if (resFactory.Accept(uri))
				{
					return resFactory.Create(uri, basePath);
				}
			}

			throw new KernelException("No Resource factory was able to " + 
				"deal with Uri " + uri.ToString());
		}
	}
}
