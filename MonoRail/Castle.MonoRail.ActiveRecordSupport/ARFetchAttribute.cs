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

namespace Castle.MonoRail.ActiveRecordSupport
{
	using System;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// This attribute tells <see cref="ARSmartDispatchController" />
	/// to fetches the ActiveRecord based on its Primary Key.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter), Serializable]
	public class ARFetchAttribute : Attribute
	{
		private String _requestParameterName;
		private bool _autoCreateNew, _throwIfNotFound;
		
		public ARFetchAttribute(String requestParameterName, bool autoCreateNew, bool throwIfNotFound) : base()
		{
			this._requestParameterName = requestParameterName;
			this._autoCreateNew = autoCreateNew;
			this._throwIfNotFound = throwIfNotFound;
		}
		
		public ARFetchAttribute() : this(null, true, true)
		{
		}
		
		public ARFetchAttribute(String requestParameterName) : this(requestParameterName, true, true)
		{
		}
		
		public ARFetchAttribute(bool autoCreateNew, bool throwIfNotFound) : this(null, autoCreateNew, throwIfNotFound)
		{
		}
		
		public String RequestParameterName
		{
			get { return _requestParameterName; }
			set { _requestParameterName = value; }
		}

		public bool AutoCreateNew
		{
			get { return _autoCreateNew; }
			set { _autoCreateNew = value; }
		}
		
		public bool ThrowIfNotFound
		{
			get { return _throwIfNotFound; }
			set { _throwIfNotFound = value; }
		}
	}
}
