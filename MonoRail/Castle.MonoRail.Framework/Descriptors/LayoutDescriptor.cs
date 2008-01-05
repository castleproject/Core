// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Descriptors
{
	using System;

	/// <summary>
	/// Represents a layout configuration
	/// </summary>
	public class LayoutDescriptor
	{
		private readonly string[] layoutNames;

		/// <summary>
		/// Initializes a new instance of the <see cref="LayoutDescriptor"/> class.
		/// </summary>
		/// <param name="layoutNames">The layout names.</param>
		public LayoutDescriptor(params String[] layoutNames)
		{
			this.layoutNames = layoutNames;
		}

		/// <summary>
		/// Gets the name of the layout.
		/// </summary>
		/// <value>The name of the layout.</value>
		public string[] LayoutNames
		{
			get { return layoutNames; }
		}

		/// <summary>
		/// Gets a value indicating whether this attribute represents a single layout.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance has single; otherwise, <c>false</c>.
		/// </value>
		public bool HasSingle
		{
			get { return layoutNames.Length == 0; }
		}
	}
}
