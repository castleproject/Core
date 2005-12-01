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
	/// Declares that the Controller should enable a DefaultAction method 
	/// for request processing if no action can be found with the supplied name
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false), Serializable]
	public class DefaultActionAttribute : Attribute
	{
		private static readonly String DEFAULT_ACTION = "DefaultAction";

		private String _defaultAction;

		public DefaultActionAttribute()
		{
			_defaultAction = DEFAULT_ACTION;
		}

		public DefaultActionAttribute(String action)
		{
			_defaultAction = action;
		}

		public String DefaultAction
		{
			get { return _defaultAction; }
		}
	}
}
