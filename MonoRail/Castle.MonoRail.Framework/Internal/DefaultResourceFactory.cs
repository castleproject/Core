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
	using System;
	using System.Reflection;
	using System.Resources;

	using Castle.MonoRail.Framework.Attributes;

	/// <summary>
	/// Standard implementation of <see cref="IResourceFactory" />
	/// </summary>
	public class DefaultResourceFactory : IResourceFactory
	{
		#region IResourceFactory Members

		public IResource Create( IResourceDefinition definition, Assembly appAssembly )
		{
			if ( definition is ResourceAttribute )
				return Create( definition as ResourceAttribute, appAssembly );

			throw new ArgumentException( "Can't create resource of type " + definition.ToString() );
		}

		public IResource Create( ResourceAttribute attribute, Assembly appAssembly )
		{
			Assembly resAssembly	= attribute.Assembly;
			ResourceManager rm		= new ResourceManager( attribute.ResourceName, 
															resAssembly == null ? appAssembly : resAssembly, 
															attribute.ResourceType );
            
			return new ResourceFacade( rm.GetResourceSet( attribute.Culture, true, true ) );
		}

		public void Release( IResource resource )
		{
		}

		#endregion
	}
}
