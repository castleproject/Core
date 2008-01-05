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

namespace Castle.MonoRail.Framework.JSGeneration.Prototype
{
	using System;
	using System.Collections;
	using Helpers;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ScriptaculousExtension
	{
		private readonly IJSCodeGenerator jsCodeGenerator;

		/// <summary>
		/// Initializes a new instance of the <see cref="ScriptaculousExtension"/> class.
		/// </summary>
		/// <param name="jsCodeGenerator">The js code generator.</param>
		public ScriptaculousExtension(IJSCodeGenerator jsCodeGenerator)
		{
			this.jsCodeGenerator = jsCodeGenerator;
		}

		/// <summary>
		/// Generates a call to a scriptaculous' visual effect. 
		/// </summary>
		/// 
		/// <seealso cref="ScriptaculousHelper"/>
		/// 
		/// <example>
		/// The following example uses nvelocity syntax:
		/// 
		/// <code>
		///   $page.VisualEffect('ToggleSlide', 'myelement')
		/// </code>
		/// 
		/// <para>
		/// This is especially useful to show which elements 
		/// where updated in an ajax call.
		/// </para>
		/// 
		/// <code>
		///	  $page.ReplaceHtml('mydiv', "Hey, I've changed")
		///   $page.VisualEffect('Highlight', 'mydiv')
		/// </code>
		/// 
		/// </example>
		/// 
		/// <param name="name">The effect name.</param>
		/// <param name="element">The target element.</param>
		/// <param name="options">The optional options.</param>
		[DynamicOperation]
		public void VisualEffect(String name, String element, IDictionary options)
		{
			jsCodeGenerator.Write(new ScriptaculousHelper().VisualEffect(name, element, options));
			jsCodeGenerator.Write(Environment.NewLine);
		}

		/// <summary>
		/// Generates a call to a scriptaculous' drop out visual effect. 
		/// </summary>
		/// 
		/// <seealso cref="ScriptaculousHelper"/>
		/// 
		/// <param name="element">The target element.</param>
		/// <param name="options">The optional options.</param>
		[DynamicOperation]
		void VisualEffectDropOut(String element, IDictionary options)
		{
			jsCodeGenerator.Write(new ScriptaculousHelper().VisualEffectDropOut(element, options));
			jsCodeGenerator.Write(Environment.NewLine);
		}
	}
}
