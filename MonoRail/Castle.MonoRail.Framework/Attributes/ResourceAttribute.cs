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

namespace Castle.MonoRail.Framework.Attributes
{
	using System;
	using System.Globalization;
	using System.Reflection;

	/// <summary>
	/// Declares that for the specified class or method, the given resource
	/// file should be loaded and set available in the PropertyBag with the 
	/// specified name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true), Serializable]
	public class ResourceAttribute : AbstractResourceAttribute
	{
		private string _ResourceName, _CultureName, _AssemblyName;
		
		private Assembly _Assembly;
		private CultureInfo _Culture;
		private Type _ResourceType;
		
		/// <summary>
		/// Constructs a resource attribute, with the specified name, based
		/// on the resource in a sattelite assembly.
		/// </summary>
		/// <param name="name">Name the resource will be available as in the PropertyBag</param>
		/// <param name="resourceName">Fully qualified name of the resource in the sattelite assembly</param>
		public ResourceAttribute( string name, string resourceName ) : base( name )
		{
			_ResourceName	= resourceName;

			if ( resourceName.IndexOf( ',' ) > 0 )
			{
				string[] pair	= resourceName.Split( ',' );
				_ResourceName	= pair[0].Trim();
				_AssemblyName	= pair[1].Trim();
			}
		}

		#region Properties

		public string ResourceName
		{
			get { return _ResourceName; }
			set { _ResourceName = value; }
		}

		public string CultureName
		{
			get { return Culture.DisplayName; }
			set { _CultureName = value; }
		}

		public CultureInfo Culture
		{
			get
			{
				if ( _Culture != null )
					return _Culture;
				else if ( _CultureName != null )
					return CultureInfo.CreateSpecificCulture( _CultureName );
				
				return CultureInfo.CurrentCulture;
			}
			set { _Culture = value; }
		}

		public string AssemblyName
		{
			get { return Assembly.FullName; }
			set { _AssemblyName = value; }
		}

		public Assembly Assembly
		{
			get 
			{
				if ( _Assembly != null ) 
					return _Assembly;
				else if ( _AssemblyName != null ) 
					return Assembly.Load( _AssemblyName );

				return null;
			}
			set { _Assembly = value; }
		}

		public Type ResourceType
		{
			get { return _ResourceType; }
			set { _ResourceType = value; }
		}

		#endregion
	}
}
