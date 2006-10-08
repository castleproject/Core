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
	/// Associates a provider that can add dynamic actions 
	/// to a controller
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=true), Serializable]
	public class DynamicActionProviderAttribute : Attribute
	{
		private readonly Type providerType;

		/// <summary>
		/// Constructs a <see cref="DynamicActionProviderAttribute"/>
		/// associating the supplied type as the action provider.
		/// </summary>
		/// <param name="providerType"></param>
		public DynamicActionProviderAttribute( Type providerType )
		{
			if (!typeof(IDynamicActionProvider).IsAssignableFrom( providerType ))
			{
				throw new ArgumentException( "The specified provider does not implement IDynamicActionProvider" );
			}

			this.providerType = providerType;
		}

		/// <summary>
		/// Gets the provider type
		/// </summary>
		public Type ProviderType
		{
			get { return providerType; }
		}
	}
}
