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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	public class ResourceDescriptor
	{
		private readonly Type resourceType;
		private readonly string name;
		private readonly string resourceName;
		private readonly string cultureName;
		private readonly string assemblyName;

		public ResourceDescriptor(Type resourceType, string name, string resourceName, 
		                          string cultureName, string assemblyName)
		{
			this.resourceType = resourceType;
			this.name = name;
			this.resourceName = resourceName;
			this.cultureName = cultureName;
			this.assemblyName = assemblyName;
		}

		public Type ResourceType
		{
			get { return resourceType; }
		}

		public string Name
		{
			get { return name; }
		}

		public string ResourceName
		{
			get { return resourceName; }
		}

		public string CultureName
		{
			get { return cultureName; }
		}

		public string AssemblyName
		{
			get { return assemblyName; }
		}
	}
}
