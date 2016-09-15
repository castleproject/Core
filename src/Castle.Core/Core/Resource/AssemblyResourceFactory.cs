// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

	public class AssemblyResourceFactory : IResourceFactory
	{
		public bool Accept(CustomUri uri)
		{
			return "assembly".Equals(uri.Scheme);
		}

		public IResource Create(CustomUri uri)
		{
			return Create(uri, null);
		}

		public IResource Create(CustomUri uri, String basePath)
		{
			if (basePath == null)
			{
				return new AssemblyResource(uri);
			}

			return new AssemblyResource(uri, basePath);
		}
	}
}