// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Specialized;
	using System.Text;

	/// <summary>
	/// JQuery Helper, a helper class that builds Html elements (i.e links) that contain Javascript that make Ajax calls using JQuery syntax
	/// </summary>
	///<remarks>
	///   Examples, you can make use of the DictHelper.CreateDict() to create an IDictionary of arguments that get passed to jQuery.Ajax(). 
	///   
	///</remarks>
	///<para/>
	///<para/>
	///$JQuery.LinkToRemote("FAQ",$UrlHelper.For($DictHelper.CreateDict("controller=faq", "action=list")),$DictHelper.CreateDict("type='POST'","update=#bodycontent"))  
	///<para/>
	///<para/>
	///Results in html of : <a onclick="jQuery.ajax({url:'/faq/list.rails', success: function(data){ jQuery('#bodycontent').html(data);}, type:'POST'}); return false;" href="javascript:void(0);">FAQ</a>
	///<para/>
	///$JQuery.LinkToRemote("Mileage ",$UrlHelper.For($DictHelper.CreateDict("controller=Report", "action=DetailMileageByDateRange")),$DictHelper.CreateDict("with='id=$RunWorkOutTypeId'","type='GET'","dataType='script'"))
	///<para/>
	///<para/>
	///Results in html of : <a onclick="jQuery.ajax({url:'/Report/DetailMileageByDateRange.rails', type:'GET', dataType:'script', data:'id=a9d72871-e691-45a1-8955-2a22bdbbb10b'}); return false;" href="javascript:void(0);">Mileage </a>
	///<para/>
	///$JQuery.LinkToRemote("FAQ",$UrlHelper.For($DictHelper.CreateDict("controller=faq", "action=list")),$DictHelper.CreateDict("type='POST'","success='function(d2){ processData(d2); }'"))  
	///<para/>
	///<para/>
	///Results in html of : <a onclick="jQuery.ajax({url:'/faq/list.rails', success: function(d2){ processData(d2);}, type:'POST'}); return false;" href="javascript:void(0);">FAQ</a>
	///<para/>
	public class JQueryHelper : AjaxHelper
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AjaxHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		public JQueryHelper(IEngineContext engineContext) : base(engineContext)
		{
		}

		/// <summary>
		/// Build Ajax Options
		/// </summary>
		/// <param name="jsOptions"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		protected override string BuildAjaxOptions(IDictionary jsOptions, IDictionary options)
		{
			BuildCallbacks(jsOptions, options);

			if (options.Contains("type"))
			{
				jsOptions["type"] = options["type"];
			}

			if (options.Contains("async"))
			{
				jsOptions["async"] = options["async"];
			}

			if (options.Contains("dataType"))
			{
				jsOptions["dataType"] = options["dataType"];
			}


			if (options.Contains("method"))
			{
				jsOptions["method"] = options["method"];
			}


			if (options.Contains("position"))
			{
				jsOptions["insertion"] = String.Format("Insertion.{0}", options["position"]);
			}

			if (!options.Contains("with") && options.Contains("form"))
			{
				jsOptions["data"] = "jQuery('form').serialize();";
			}


			else if (options.Contains("with"))
			{
				jsOptions["data"] = ProcessWith(options["with"]);
			}

			return JQueryJavascriptOptions(jsOptions);
		}

		/// <summary>
		/// Remote Function
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public override String RemoteFunction(IDictionary options)
		{
			IDictionary jsOptions = new HybridDictionary();

			String javascriptOptionsString = BuildAjaxOptions(jsOptions, options);

			var contents = new StringBuilder();


			bool isRequestOnly = !options.Contains("update") &&
			                     !options.Contains("success") && !options.Contains("failure");

			if (!options.Contains("url"))
			{
				throw new ArgumentException("url is required");
			}
			String urlOptions = GetUrlOptionNoQuotes(options);

			if (isRequestOnly)
			{
				contents.Append("jQuery.ajax({");
				contents.AppendFormat("url:'{0}'", urlOptions);
			}
			else
			{
				if (options.Contains("update"))
				{
					contents.Append("jQuery.ajax({");

					//callback function implemented via anonymous function 
					String anonymousFunction = string.Format("function(data){{ jQuery('{0}').html(data);}}", options["update"]);

					if (!options.Contains("success"))
					{
						contents.AppendFormat("url:'{0}', success: {1}", urlOptions, anonymousFunction);
						options.Remove("update");
					}
					else
					{
						contents.AppendFormat("url:'{0}', success: {1}", urlOptions, options["success"]);
						options.Remove("update");
						options.Remove("success");
					}
				}
				else
				{
					contents.Append("jQuery.ajax({");

					if (options.Contains("success"))
					{
						contents.AppendFormat("url:'{0}', success: {1}", urlOptions, options["success"]);

						options.Remove("success");
					}
				}
			}

			if (javascriptOptionsString != string.Empty)
			{
				contents.Append(", " + javascriptOptionsString + "})");
			}
			else
			{
				contents.Append("})");
			}


			if (options.Contains("condition"))
			{
				String old = contents.ToString();

				contents = new StringBuilder(
					String.Format("if ( {0} ) {{ {1}; }}", options["condition"], old));

				options.Remove("condition");
			}

			return contents.ToString();
		}


		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		public override String InstallScripts()
		{
			return RenderScriptBlockToSource("/MonoRail/Files/JQueryScripts");
		}

		/// <summary>
		/// Get Url Option
		/// </summary>
		/// <param name="options">Dictionary of options</param>
		/// <returns>string</returns>
		protected String GetUrlOptionNoQuotes(IDictionary options)
		{
			var url = (String) options["url"];

			if (url.StartsWith("<") && url.EndsWith(">"))
			{
				return url.Substring(1, url.Length - 2);
			}

			return url;
		}

		/// <summary>
		/// Builds a JS associative array based on the specified dictionary instance.
		/// <para>
		/// For example: <c>name: value, other: 'another'</c>
		/// </para>
		/// </summary>
		/// <param name="jsOptions">The js options.</param>
		/// <returns>An associative array in javascript</returns>
		public static string JQueryJavascriptOptions(IDictionary jsOptions)
		{
			if (jsOptions == null || jsOptions.Count == 0)
			{
				// return "{}";
				return string.Empty;
			}

			var sb = new StringBuilder(jsOptions.Count * 10);

			bool comma = false;

			foreach(DictionaryEntry entry in jsOptions)
			{
				if (!comma)
				{
					comma = true;
				}
				else
				{
					sb.Append(", ");
				}

				sb.Append(string.Format("{0}:{1}", entry.Key, entry.Value));
			}


			return sb.ToString();
		}
	}
}