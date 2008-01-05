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
	using System.Collections;

	// TODO: Add support for Sortable, Draggable, and DropReceiving 

	/// <summary>
	/// Exposes the effect script from Thomas Fuchs 
	/// (http://script.aculo.us, http://mir.aculo.us)
	/// </summary>
	/// <remarks>
	/// Before using it, you must install the scripts. 
	/// See <see cref="ScriptaculousHelper.InstallScripts"/>
	/// </remarks>
	public class ScriptaculousHelper : AbstractHelper
	{
		#region Scripts

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public String InstallScripts()
		{
			return RenderScriptBlockToSource("/MonoRail/Files/Effects2");
		}

		/// <summary>
		/// Gets the javascript functions.
		/// </summary>
		/// <returns></returns>
		[Obsolete("Please use the preferred InstallScripts function.")]
		public String GetJavascriptFunctions()
		{
			return InstallScripts();
		}

		#endregion

		/// <summary>
		/// Generates a JS snippet invoking the specified effect. 
		/// <para>Examples:
		/// <code>
		/// VisualEffect('ToggleSlide', 'elementid')
		/// VisualEffect('ToggleBlind', 'elementid')
		/// VisualEffect('ToggleAppear', 'elementid')
		/// VisualEffect('Highlight', 'elementid')
		/// VisualEffect('Fade', 'elementid')
		/// VisualEffect('Shake', 'elementid')
		/// VisualEffect('DropOut', 'elementid')
		/// </code>
		/// </para>
		/// </summary>
		/// <param name="name">The effect name.</param>
		/// <param name="elementId">The element id to act upon.</param>
		/// <returns>A JS snippet</returns>
		public string VisualEffect(string name, string elementId)
		{
			return VisualEffect(name, elementId, null);
		}

		/// <summary>
		/// Generates a JS snippet invoking the specified effect. 
		/// <para>Examples:
		/// <code>
		/// VisualEffect('ToggleSlide', 'elementid')
		/// VisualEffect('ToggleBlind', 'elementid')
		/// VisualEffect('ToggleAppear', 'elementid')
		/// VisualEffect('Highlight', 'elementid')
		/// VisualEffect('Fade', 'elementid')
		/// VisualEffect('Shake', 'elementid')
		/// VisualEffect('DropOut', 'elementid')
		/// </code>
		/// </para>
		/// <para>
		/// Common options includes <c>duration</c>, 
		/// <c>transition</c>, <c>fps</c>, <c>sync</c>, 
		/// <c>from</c>, <c>to</c>, <c>delay</c>, <c>queue</c>, 
		/// <c>startcolor</c>, <c>endcolor</c>.
		/// </para>
		/// <para>
		/// Callbacks:
		/// <c>beforeStart</c>, <c>beforeUpdate</c>, <c>afterUpdate</c>, <c>afterFinish</c>
		/// </para>
		/// </summary>
		/// <remarks>
		/// If you want to use the DropOut effect, please refer to <see cref="VisualEffectDropOut"/>
		/// </remarks>
		/// <param name="name">The effect name.</param>
		/// <param name="elementId">The element id to act upon.</param>
		/// <param name="options">A dictionary used to specify options to the effect behavior</param>
		/// <returns>A JS snippet</returns>
		public string VisualEffect(string name, string elementId, IDictionary options)
		{
			if (name == null) throw new ArgumentNullException("name", "Visual effect name is required");

			if (elementId == null) elementId = "element";

			if (name.ToLowerInvariant().StartsWith("toggle"))
			{
				return "Effect.toggle(" + SQuote(elementId) + ", " + 
					SQuote(name.Substring("toggle".Length).ToLowerInvariant()) + 
					", " + JavascriptOptions(options) + ");";
			}
			else
			{
				return "new Effect." + name + "(" + SQuote(elementId) + ", " + JavascriptOptions(options) + ");";
			}
		}

		/// <summary>
		/// Generates a JS snippet invoking the DropOut effect
		/// <para>Examples:
		/// <code>
		/// VisualEffectDropOut('elementid', {position:'end', scope='test', limit=2})
		/// </code>
		/// </para>
		/// </summary>
		/// <param name="elementId">The element id to act upon.</param>
		/// <param name="queue">A dictionary used to specify options to the DropOut behavior</param>
		/// <returns>A JS snippet</returns>
		public string VisualEffectDropOut(string elementId, IDictionary queue)
		{
			if (elementId == null) throw new ArgumentNullException("elementId", "Visual effect's elementId is required");

			if (queue == null || queue.Count == 0)
			{
				return "new Effect.DropOut(" + SQuote(elementId) + ", {queue:'end'});";
			}
			else
			{
				return "new Effect.DropOut(" + SQuote(elementId) + ", " + JavascriptOptions(queue) + ");";
			}
		}
	}
}
