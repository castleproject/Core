// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Collections;
	using System.IO;

	/// <summary>
	/// Renders the contents of the block component into the $childContent context
	/// variable, and then renders the components view file.
	/// </summary>
	/// <example>
	/// Controller view:
	/// <code>
	/// #blockcomponent(ChildContentComponent)
	///		This will be rendered inside a div tag.
	/// #end
	/// </code>
	/// 
	/// ViewComponent view:
	/// <code>
	/// &lt;div&gt;$componentChildContent&lt;/&gt;
	/// </code>
	/// </example>
	public class ChildContentComponent : ViewComponent
	{
		private readonly String EntryKey = "componentChildContent";
		
		protected virtual void ObtainChildContent()
		{
			StringWriter writer = new StringWriter();
			
			Context.RenderBody(writer);

			Context.ContextVars[EntryKey] = writer.ToString();
		}

		protected void PopulateContext()
		{
			foreach(DictionaryEntry value in ComponentParams)
			{
				Context.ContextVars[value.Key] = value.Value;
			}
		}

		public override void Render()
		{
			RenderView("default");
		}

		public new void RenderView(string name)
		{
			PopulateContext();
			ObtainChildContent();

			base.RenderView(name);
		}

		public new void RenderView(string component, string name)
		{
			PopulateContext();
			ObtainChildContent();

			base.RenderView(component, name);
		}
	}
}