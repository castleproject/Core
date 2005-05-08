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
	/// Associates a rescue template with a <see cref="Controller"/>
	/// or an action (method). The rescue is invoked in
	/// response to some exception during the action processing.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Method, AllowMultiple=false)]
	public class RescueAttribute : Attribute
	{
		private String _viewName;
		
		/// <summary>
		/// Constructs a RescueAttribute with the template name.
		/// </summary>
		/// <param name="viewName"></param>
		public RescueAttribute(String viewName)
		{
			if (viewName == null || viewName.Length == 0)
			{
				throw new ArgumentNullException("viewName");
			}

			_viewName = viewName;
		}

		public String ViewName
		{
			get { return _viewName; }
		}
	}
}
