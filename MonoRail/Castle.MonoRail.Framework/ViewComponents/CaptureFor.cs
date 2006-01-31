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
	using System.IO;
	using System.Text;

	/// <summary>
	/// Renders the inner content and stores it in the IViewEngineContext
	/// <code>
	/// #blockcomponent(CaptureFor with "id=someId" ["append=before"])
	///		content to be captured
	/// #end
	///
	/// ${someId}
	/// </code>
	/// id - the key to be used to retrieve the captured contents
	/// append - when present will append component content into the current
	///			 content, if append = "before" will append before the current content
	/// </summary>
	public class CaptureFor : ViewComponent
	{
		/// <summary>
		/// Render component's content and stores it in the view engine ContextVars
		/// so it can be reference and included in other places
		/// </summary>
		public override void Render()
		{
			String id = Context.ComponentParameters["id"] as string;

			if( id == null || id.Trim().Length == 0 )
			{
				throw new RailsException("CaptureFor requires an id attribute use #blockcomponent(CaptureFor with \"id=someid\")...#end");
			}

			StringWriter buffer = new StringWriter();

			Context.RenderBody(buffer);

			String currentContent = Context.ContextVars[id] as string;
			StringBuilder sb = buffer.GetStringBuilder();
			String appendAtt = Context.ComponentParameters["append"] as string;

			if( appendAtt != null )
			{				
				if( appendAtt == "before" )
				{
					sb.Append(currentContent);
				}
				else
				{
					sb.Insert(0, currentContent);
				}				
			}

			Context.ContextVars[id] = sb.ToString();	
		}
	}
}