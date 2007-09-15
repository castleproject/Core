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

namespace Castle.MonoRail.Framework.ViewComponents
{
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Renders a javascript tag block with an inline javascript 
	/// that changes the page 
	/// elements using a special dsl-like language.
	/// </summary>
	/// 
	/// <seealso cref="PrototypeHelper"/>
	/// 
	/// <example>
	/// The following illustrates its use.
	/// <code>
	/// #blockcomponent(UpdatePageTag)
	///   $page.ReplaceHtml('myotherdiv', 'new content')
	///   $page.Highlight('mydivid')
	/// #end
	/// </code>
	/// </example>
	/// <remarks>
	/// The current implementation is dependent on 
	/// prototype.js and scriptaculous.js
	/// </remarks>
	public class UpdatePageTag : UpdatePage
	{
		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			Context.Writer.WriteLine(AbstractHelper.ScriptBlock(GenerateJS()));
		}
	}
}
