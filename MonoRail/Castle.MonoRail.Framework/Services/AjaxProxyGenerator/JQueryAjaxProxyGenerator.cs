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

namespace Castle.MonoRail.Framework.Services.AjaxProxyGenerator
{
	using System;
	using System.Text;

	/// <summary>
	/// Provides a service which generates a <em>JavaScript</em> block, that
	/// can be used to call Ajax actions on the controller. This JavaScript will
	/// use the <em>JQuery</em> syntax.
	/// </summary>
	public class JQueryAjaxProxyGenerator : AbstractAjaxProxyGenerator
	{
		/// <summary>
		/// Generates the javascript proxy function.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="httpRequestMethod">The HTTP request method.</param>
		/// <param name="scriptFunctionParameters">The script function parameters.</param>
		/// <returns></returns>
		protected override string GenerateJavascriptFunction(string url, string functionName, string httpRequestMethod,
		                                                     ScriptFunctionParameter[] scriptFunctionParameters)
		{
			StringBuilder function = new StringBuilder();
			function.AppendFormat("\t{0}: function(", functionName);

			StringBuilder ajaxCallParameters = new StringBuilder("_=");

			for(int i = 0; i < scriptFunctionParameters.Length; i++)
			{
				ScriptFunctionParameter scriptFunctionParameter = scriptFunctionParameters[i];
				string clientSideName = scriptFunctionParameter.ClientSideParameterName;
				string serverSideName = scriptFunctionParameter.ServerSideParameterName;

				// append to the function parameters
				function.AppendFormat("{0}{1}", clientSideName, i < scriptFunctionParameters.Length - 1 ? ", " : string.Empty);

				// append to the ajax call parameters
				ajaxCallParameters.Append("&")
					.Append(serverSideName)
					.Append("='+")
					// I don't know the equivalent of Object.toJSON in JQuery, so if you need it.. this is the place to insert it :)
					//.Append(scriptFunctionParameter.NeedsJSONEncoding ? "Object.toJSON(" + clientSideName + ")" : clientSideName)
					.Append(clientSideName)
					.Append("+'");

				// close the last parameter with an extra "," for the callback parameter
				function.Append(i == scriptFunctionParameters.Length - 1 ? ", " : string.Empty);
			}

			function.Append("callback)").Append(Environment.NewLine).Append("\t{").Append(Environment.NewLine);

			function.AppendFormat("\t\t" +
			                      "var r=$.ajax({{type: '{0}'," +
			                      "url: '{1}'," +
			                      "data: '{2}'," +
			                      "async: !!callback," +
			                      "complete: callback}});" +
			                      Environment.NewLine + "\t\t" +
			                      "if(!callback) return r.responseText;",
			                      httpRequestMethod, url, ajaxCallParameters);
			function.Append("\r\n\t}\r\n");

			return function.ToString();
		}
	}
}