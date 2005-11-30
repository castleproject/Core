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
		private bool _create, _require;
		
		public ARFetchAttribute(String requestParameterName, bool create, bool require) : base()
		{
			this._requestParameterName = requestParameterName;
			this._create = create;
			this._require = require;
		}
		
		public ARFetchAttribute() : this(null, true, true)
		{
		}
		
		public ARFetchAttribute(String requestParameterName) : this(requestParameterName, true, true)
		{
		}
		
		public ARFetchAttribute(bool create, bool require) : this(null, create, require)
		{
		}
		
		public String RequestParameterName
		{
			get { return _requestParameterName; }
			set { _requestParameterName = value; }
		}

		public bool Create
		{
			get { return _create; }
			set { _create = value; }
		}
		
		public bool Require
		{
			get { return _require; }
			set { _require = value; }
		}
	}
}
