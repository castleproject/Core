// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.Resources;

	/// <summary>
	/// Simple facade that provides the IResource interface to a
	/// ResourceSet instance.
	/// </summary>
	public class ResourceFacade : IResource
	{
		private ResourceSet resourceSet;

		public ResourceFacade( ResourceSet resourceSet )
		{
			this.resourceSet = resourceSet;
		}

		#region IResource Members

		public object this[ string key ]
		{
			get
			{
				return resourceSet.GetString( key, true );
			}
		}

		public string GetString( string key )
		{
			return resourceSet.GetString( key, true );
		}

		public object GetObject(string key)
		{
			return resourceSet.GetObject( key, true );
		}

		#endregion
	}
}
