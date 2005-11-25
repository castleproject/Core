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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;

	/// <summary>
	/// Exposes the effect script from Thomas Fuchs 
	/// (http://script.aculo.us, http://mir.aculo.us)
	/// </summary>
	public class Effects2Helper : AbstractHelper
	{
		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public String GetJavascriptFunctions()
		{
			return String.Format("<script type=\"text/javascript\" src=\"{0}.{1}\"></script>", 
				Controller.Context.ApplicationPath + "/MonoRail/Files/Effects2", 
				Controller.Context.UrlInfo.Extension);
		}

		/// <summary>
		/// Make an element appear. If the element was previously set to display:none;  
		/// inside the style attribute of the element, the effect will automatically 
		/// show the element.
		/// </summary>
		/// <remarks>
		/// Microsoft Internet Explorer can only set opacity on elements that have a 
		/// 'layout'. To let an element have a layout, you must set some CSS 
		/// positional properties, like 'width' or 'height'.
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String Appear(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.Appear('{0}');", elementId ) );
		}

		/// <summary>
		/// Makes an element fade away and takes it out of the document flow 
		/// at the end of the effect by setting the CSS display property to false.
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI).
		/// Microsoft Internet Explorer can only set opacity on elements that 
		/// have a 'layout'. To let an element have a layout, you must set some 
		/// CSS positional properties, like 'width' or 'height'.
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String Fade(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.Fade('{0}');", elementId ) );
		}

		/// <summary>
		/// Gives the illusion of the element puffing away (like a in a cloud of smoke).
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI).
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String Puff(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.Puff('{0}');", elementId ) );
		}

		/// <summary>
		///  Makes the element drop and fade out at the same time.
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI).
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String DropOut(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.DropOut('{0}');", elementId ) );
		}

		/// <summary>
		/// Moves the element slightly to the left, then to the right, repeatedly.
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI).
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String Shake(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.Shake('{0}');", elementId ) );
		}

		/// <summary>
		/// Gives the illusion of a TV-style switch off.
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI).
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String SwitchOff(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.SwitchOff('{0}');", elementId ) );
		}

		/// <summary>
		/// This pair of effects simulates a window blind, where the 
		/// contents of the affected elements stay in place.
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI), 
		/// except table rows, table bodies and table heads.
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String BlindUp(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.BlindUp('{0}');", elementId ) );
		}

		/// <summary>
		/// This pair of effects simulates a window blind, where the 
		/// contents of the affected elements stay in place.
		/// </summary>
		/// <remarks>
		/// Works safely with most HTML block elements (like DIV and LI), 
		/// except table rows, table bodies and table heads.
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String BlindDown(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.BlindDown('{0}');", elementId ) );
		}

		/// <summary>
		/// This pair of effects simulates a window blind, where the contents of 
		/// the affected elements scroll up and down accordingly.
		/// </summary>
		/// <remarks>
		///  You must include a second DIV element, 
		///  wrapping the contents of the outer DIV. 
		///  So, if you call new Effect.SlideDown('x'), your element must look like this:
		/// <code>
		///  &lt;div id="x"&gt;&lt;div&gt;contents&lt;/div&gt;&lt;/div&gt;
		/// </code>
		/// Because of a bug in Internet Explorer 6 (overflow not correctly hidden), 
		/// an additional wrapper div is needed if you want to use these effects on 
		/// absolutely positionend elements (wrapper is the absolutely positioned element, 
		/// x has position:relative; set; ):
		/// <code>
		/// &lt;div id="wrapper"&gt;
		///  &lt;div id="x"&gt;&lt;div&gt;contents&lt;/div&gt;&lt;/div&gt;
		/// &lt;/div&gt;
		/// </code>
		/// Works only on block elements. 
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String SlideUp(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.SlideUp('{0}');", elementId ) );
		}

		/// <summary>
		/// This pair of effects simulates a window blind, where the contents of 
		/// the affected elements scroll up and down accordingly.
		/// </summary>
		/// <remarks>
		///  You must include a second DIV element, 
		///  wrapping the contents of the outer DIV. 
		///  So, if you call new Effect.SlideDown('x'), your element must look like this:
		/// <code>
		///  &lt;div id="x"&gt;&lt;div&gt;contents&lt;/div&gt;&lt;/div&gt;
		/// </code>
		/// Because of a bug in Internet Explorer 6 (overflow not correctly hidden), 
		/// an additional wrapper div is needed if you want to use these effects on 
		/// absolutely positionend elements (wrapper is the absolutely positioned element, 
		/// x has position:relative; set; ):
		/// <code>
		/// &lt;div id="wrapper"&gt;
		///  &lt;div id="x"&gt;&lt;div&gt;contents&lt;/div&gt;&lt;/div&gt;
		/// &lt;/div&gt;
		/// </code>
		/// Works only on block elements. 
		/// </remarks>
		/// <param name="elementId"></param>
		/// <returns></returns>
		public String SlideDown(String elementId)
		{
			return ScriptBlock( String.Format( "new Effect.SlideDown('{0}');", elementId ) );
		}
	}
}