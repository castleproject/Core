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
	/// Represents a layout configuration
	/// </summary>
	public class LayoutDescriptor
	{
		private readonly String layoutName;

		/// <summary>
		/// Initializes a new instance of the <see cref="LayoutDescriptor"/> class.
		/// </summary>
		/// <param name="layoutName">Name of the layout.</param>
		public LayoutDescriptor(String layoutName)
		{
			this.layoutName = layoutName;
		}

		/// <summary>
		/// Gets the name of the layout.
		/// </summary>
		/// <value>The name of the layout.</value>
		public String LayoutName
		{
			get { return layoutName; }
		}
	}
}
