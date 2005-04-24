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

namespace Castle.CastleOnRails.Framework.Helpers
{
	using System;
	using System.Reflection;
	using System.Resources;

	/// <summary>
	/// Just apply the CSS class "fade" to any element and it will fade from yellow
	/// to its background color or white if none is specified.
	/// 
	/// Key Features
	/// * Fade an infinite number of elements.
	/// * No inline JavaScript. Simply give an element a class of "fade", the script does the rest "automagically".
	/// * Background color aware. FAT will do better than simply fade to white if the element (or it's parents) have a CSS background color, it will fade to that background color instead.
	/// * Fade from any color. For example, if you wanted a list of error messages to fade out from red you would simply give the list a class of "fade-FF0000".
	/// * Super smooth fading. By default, elements will fade at 30 frames per second (the same rate as a television) over 3 seconds. You can adjust this to any framerate and any duration. You could easily fade elements at 60 frames per second over 4, 5, 10 seconds!
	/// </summary>
	/// <remarks>
	/// NOTE: All elements to be faded must have an id tag!
	/// NOTE: it adds itself to the window.onload event in a way that may
	/// screw other things up still and is still beta code.
	/// BASIC USE: <p id="paragraph1" class="fade">Watch me fade</p>
	/// ADVANCED USE: Change the default fade from color:
	/// <p id="paragraph1" class="fade-0066FF">Watch me fade from Blue (#0066FF)</p>
	/// SEE MORE HERE: http://www.axentric.com/posts/default/7
	/// </remarks>
	public class EffectsFatHelper 
	{
		private static ResourceManager _resourceManager;

		protected ResourceManager ResourceManager
		{
			get
			{
				if (_resourceManager == null)
				{
					Assembly assembly = Assembly.GetAssembly( typeof(EffectsFatHelper) );
					_resourceManager = new ResourceManager("Castle.CastleOnRails.Framework.Javascripts", assembly);
				}

				return _resourceManager;
			}
		}

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public String GetJavascriptFunctions()
		{
			return ResourceManager.GetString("fatfunctions");
		}
	}
}
