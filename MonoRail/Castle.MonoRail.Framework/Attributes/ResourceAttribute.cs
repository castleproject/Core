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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Declares that for the specified class or method, the given resource file should be 
	/// loaded and set available in the PropertyBag with the specified name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true, Inherited=true), Serializable]
	public class ResourceAttribute : Attribute, IResourceDefinition
	{
		private String _name, _resourceName, _cultureName, _assemblyName;
		private Type _resourceType;
		
		/// <summary>
		/// Constructs a resource attribute, with the specified name, based
		/// on the resource in a satellite assembly.
		/// </summary>
		/// <param name="name">Name the resource will be available as in the PropertyBag</param>
		/// <param name="resourceName">Fully qualified name of the resource in the sattelite assembly</param>
		public ResourceAttribute( String name, String resourceName )
		{
			_name = name;
			_resourceName	= resourceName;

			if ( resourceName.IndexOf( ',' ) > 0 )
			{
				String[] pair	= resourceName.Split( ',' );
				_resourceName	= pair[0].Trim();
				_assemblyName	= pair[1].Trim();
			}
		}

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public String ResourceName
		{
			get { return _resourceName; }
			set { _resourceName = value; }
		}

		public String CultureName
		{
			get { return _cultureName; }
			set { _cultureName = value; }
		}

		public String AssemblyName
		{
			get { return _assemblyName; }
			set { _assemblyName = value; }
		}

		public Type ResourceType
		{
			get { return _resourceType; }
			set { _resourceType = value; }
		}
	}
}
