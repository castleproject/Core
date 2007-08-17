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
	using System.Collections;

	/// <summary>
	/// 
	/// </summary>
	public class UrlHelper : AbstractHelper
	{
		/// <summary>
		/// Builds the URL path.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		public string For(IDictionary parameters)
		{
			SetEncodeDefault(parameters);
			return Controller.UrlBuilder.BuildUrl(Controller.Context.UrlInfo, parameters);
		}

		public string Link(string innerContent, IDictionary parameters)
		{
			SetEncodeDefault(parameters);
			return "<a href=\"" + For(parameters) + "\">" + innerContent + "</a>";
		}

		public string Link(string innerContent, IDictionary parameters, IDictionary anchorAttributes)
		{
			SetEncodeDefault(parameters);
			return "<a " + GetAttributes(anchorAttributes) + " href=\"" + For(parameters) + "\">" + innerContent + "</a>";
		}

		public string ButtonLink(string innerContent, IDictionary parameters)
		{
			SetEncodeDefault(parameters);
			return "<button type=\"button\" onclick=\"javascript:window.location.href = '" + For(parameters) + "'\">" + innerContent + "</button>";
		}

		public string ButtonLink(string innerContent, IDictionary parameters, IDictionary anchorAttributes)
		{
			SetEncodeDefault(parameters);
			return "<button type=\"button\"" + GetAttributes(anchorAttributes) + " onclick=\"javascript:window.location.href = '" + For(parameters) + "'\">" + innerContent + "</button>";
		}

		private static void SetEncodeDefault(IDictionary parameters)
		{
			if (!parameters.Contains("encode"))
			{
				parameters["encode"] = "true";
			}
		}
	}
}
