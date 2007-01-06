// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

	/// <summary>
	/// Represents a view engine configuration
	/// </summary>
	public class ViewEngineInfo
	{
		private Type engine;
		private bool xhtmlRendering;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewEngineInfo"/> class.
		/// </summary>
		/// <param name="engine">The engine.</param>
		/// <param name="xhtmlRendering">if set to <c>true</c> [XHTML rendering].</param>
		public ViewEngineInfo(Type engine, bool xhtmlRendering)
		{
			this.engine = engine;
			this.xhtmlRendering = xhtmlRendering;
		}

		/// <summary>
		/// Gets or sets the View Engine type.
		/// </summary>
		/// <value>The View Engine type.</value>
		public Type Engine
		{
			get { return engine; }
			set { engine = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether 
		/// the view engine should send the content using the xhtml mime type. 
		/// </summary>
		/// <value><c>true</c> if xhtml mime type should be used; otherwise, <c>false</c>.</value>
		public bool XhtmlRendering
		{
			get { return xhtmlRendering; }
			set { xhtmlRendering = value; }
		}
	}
}
