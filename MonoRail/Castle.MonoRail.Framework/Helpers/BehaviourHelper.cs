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

namespace Castle.MonoRail.Framework.Helpers
{
	using System;

	/// <summary>
	/// Exposes the functionality available on the Behaviour js library
	/// which Uses css selectors to bind javascript code to DOM elements 
	/// </summary>
	/// <remarks>
	/// Before using it, you must install the scripts. See <see cref="BehaviourHelper.InstallScripts"/>
	/// </remarks>
	public class BehaviourHelper : AbstractHelper
	{
		private bool behaviourCommaNeeded;

		#region Scripts

		/// <summary>
		/// Renders a script tag refering the Behaviour library code.
		/// </summary>
		/// <returns></returns>
		public String InstallScripts()
		{
			return RenderScriptBlockToSource("/MonoRail/Files/BehaviourScripts");
		}

		#endregion

		/// <summary>
		/// Renders a script block invoking <c>Behaviour.apply()</c>
		/// </summary>
		public String ReApply()
		{
			return ScriptBlock("Behaviour.apply();");
		}

		/// <summary>
		/// Renders a script block invoking <c>Behaviour.addLoadEvent(loadFunctionName);</c>
		/// </summary>
		/// <param name="loadFunctionName">The name of the js function to be invoked when the body is loaded</param>
		public String AddLoadEvent(String loadFunctionName)
		{
			return ScriptBlock("Behaviour.addLoadEvent(" + loadFunctionName + ");");
		}

		/// <summary>
		/// Renders a script block starting the association of events to selector rules
		/// <seealso cref="Register"/>
		/// <seealso cref="EndBehaviourRegister"/>
		/// </summary>
		public String StartBehaviourRegister()
		{
			behaviourCommaNeeded = false;

			return "<script type=\"text/javascript\">" + Environment.NewLine +
				"Behaviour.register({" + Environment.NewLine;
		}

		/// <summary>
		/// Adds a entry to a registration array. Invoking it 
		/// with <c>#form</c>, <c>onsubmit</c> and <c>validate</c> will produce
		/// <c>'#form' : function(e){ e.onsubmit = validate; },</c>
		/// <seealso cref="StartBehaviourRegister"/>
		/// <seealso cref="EndBehaviourRegister"/>
		/// </summary>
		/// <param name="selector">The css selector rule</param>
		/// <param name="eventName">The name of the event on the element</param>
		/// <param name="jsFunctionName">The function to be invoked in response to the event</param>
		public String Register(String selector, String eventName, String jsFunctionName)
		{
			String val = behaviourCommaNeeded ? "," : String.Empty +
					"\t'" + selector + "' : function(e){ e." + eventName + " = " + jsFunctionName + "; }" +
				   Environment.NewLine;

			if (!behaviourCommaNeeded)
			{
				behaviourCommaNeeded = true;
			}

			return val;
		}

		/// <summary>
		/// Renders the end of a script block that associated events to selector rules
		/// <seealso cref="StartBehaviourRegister"/>
		/// <seealso cref="Register"/>
		/// </summary>
		public String EndBehaviourRegister()
		{
			return Environment.NewLine + "});" + Environment.NewLine + "</script>";
		}
	}
}
