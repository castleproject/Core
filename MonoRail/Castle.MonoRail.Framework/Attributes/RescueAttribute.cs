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

namespace Castle.MonoRail.Framework
{
	using System;

	/// <summary>
	/// Associates a rescue template with a <see cref="Controller"/> or an action 
	/// (method). The rescue is invoked in response to some exception during the 
	/// action processing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=true), Serializable]
	public class RescueAttribute : Attribute
	{
		private String _viewName;
		private Type _exceptionType;
		
		/// <summary>
		/// Constructs a RescueAttribute with the template name.
		/// </summary>
		/// <param name="viewName"></param>
		public RescueAttribute( String viewName ) : this( viewName, typeof(Exception) )
		{			
		}

		public RescueAttribute( String viewName, Type exceptionType )
		{
			if ( viewName == null || viewName.Length == 0 )
			{
				throw new ArgumentNullException( "viewName" );
			}
			
			if ( exceptionType != null && !typeof( Exception ).IsAssignableFrom( exceptionType ) )
			{
				throw new ArgumentException( "exceptionType must be a type assignable from Exception" );
			}
			
			this._viewName = viewName;
			this._exceptionType = exceptionType;
		}

		public String ViewName
		{
			get { return _viewName; }
		}
		
		public Type ExceptionType
		{
			get { return _exceptionType; }
		}		
	}

	/// <summary>
	/// Declares that for the specific method (action) no rescue should be performed.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple=false, Inherited=true), Serializable]
	public class SkipRescueAttribute : Attribute
	{
	}
}
