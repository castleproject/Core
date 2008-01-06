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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;

	/// <summary>
	/// MonoRail Helper that delivers Zebda validation capabilities.
	/// </summary>
	public class ZebdaHelper : AbstractHelper
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ZebdaHelper"/> class.
		/// </summary>
		public ZebdaHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="ZebdaHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public ZebdaHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

		#region Scripts

		/// <summary>
		/// Renders a zebda library inside a script tag.
		/// </summary>
		public String InstallScripts()
		{
			return RenderScriptBlockToSource("/MonoRail/Files/ZebdaScripts");
		}

		#endregion
	}
}
