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
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// MonoRail Helper that delivers AJAX capabilities.
	/// </summary>
	/// <remarks>
	/// This version provides less overloads but makes intensive use of 
	/// <see cref="IDictionary"/> to pass on options and attributes.
	/// </remarks>
	public class AjaxHelper : AbstractHelper
	{
		public AjaxHelper()
		{
		}

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public String GetJavascriptFunctions()
		{
			return String.Format("<script type=\"text/javascript\" src=\"{0}.{1}\"></script>", 
				Controller.Context.ApplicationPath + "/MonoRail/Files/AjaxScripts", 
				Controller.Context.UrlInfo.Extension);
		}

		/// <summary>
		/// Renders a script tag refering the Behaviour library code.
		/// </summary>
		/// <returns></returns>
		public String GetBehaviourFunctions()
		{
			return String.Format("<script type=\"text/javascript\" src=\"{0}.{1}\"></script>", 
				Controller.Context.ApplicationPath + "/MonoRail/Files/BehaviourScripts", 
				Controller.Context.UrlInfo.Extension);
		}
		
		/// <summary>
		/// Returns a link that'll trigger a javascript +function+ using the 
		/// onclick handler and return false after the fact.
		/// <code>
		/// &lt;a href="#" onclick="functionCodeOrName; return false"&gt;innerContent&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <param name="attributes">Attributes to be applied to the html element</param>
		/// <returns></returns>
		public String LinkToFunction(String innerContent, String functionCodeOrName, IDictionary attributes)
		{
			String htmlAtt = GetAttributes(attributes);

			return String.Format("<a href=\"#\" {2} onclick=\"{0}; return false;\" >{1}</a>", functionCodeOrName, innerContent, htmlAtt );
		}

		/// <summary>
		/// Returns a link that'll trigger a javascript +function+ using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <returns></returns>
		public String LinkToFunction(String innerContent, String functionCodeOrName)
		{
			return LinkToFunction(innerContent, functionCodeOrName, null);
		}

		/// <summary>
		/// Returns a button that'll trigger a javascript +function+ using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <param name="attributes">Attributes to be applied to the html element</param>
		/// <returns></returns>
		public String ButtonToFunction(String innerContent, String functionCodeOrName, IDictionary attributes) 
		{
			String htmlAtt = GetAttributes(attributes);

			return String.Format("<input type=\"button\" {2} onclick=\"{0}; return false;\" value=\"{1}\" />",
				functionCodeOrName, innerContent, htmlAtt);
		}

		/// <summary>
		/// Returns a button that'll trigger a javascript +function+ using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <returns></returns>
		public String ButtonToFunction(String innerContent, String functionCodeOrName) 
		{
			return ButtonToFunction(innerContent, functionCodeOrName, null);
		}

		/// <summary>
		/// Creates a button that if clicked will fire an Ajax invocation. 
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <returns>The handcrafted input</returns>
		public String ButtonToRemote(String innerContent, String url, IDictionary options)
		{
			return ButtonToFunction(innerContent, BuildRemoteFunction(url, options));
		}

		/// <summary>
		/// Returns a link to a remote action defined by <tt>options[:url]</tt> 
		/// (using the url_for format) that's called in the background using 
		/// XMLHttpRequest. The result of that request can then be inserted into a
		/// DOM object whose id can be specified with <tt>options[:update]</tt>. 
		/// Usually, the result would be a partial prepared by the controller with
		/// either render_partial or render_partial_collection. 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="url"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public String LinkToRemote(String name, String url, IDictionary options)
		{
			return LinkToFunction(name, BuildRemoteFunction(url, options));
		}

		public String LinkToRemote(String name, String url, IDictionary options, IDictionary htmloptions)
		{
			return LinkToFunction(name, BuildRemoteFunction(url, options), htmloptions);
		}

		/// <summary>
		/// Returns a form tag that will submit using XMLHttpRequest 
		/// in the background instead of the regular 
		///	reloading POST arrangement. Even though it's 
		///	using Javascript to serialize the form elements, the form submission 
		///	will work just like a regular submission as viewed by the 
		///	receiving side (all elements available in @params).
		///	The options for specifying the target with :url and defining 
		///	callbacks is the same as link_to_remote.
		/// </summary>
		/// <returns></returns>
		public String BuildFormRemoteTag(String url, IDictionary options)
		{
			return BuildFormRemoteTag( GetOptions(url, options) );
		}

		/// <summary>
		/// Returns a form tag that will submit using XMLHttpRequest 
		/// in the background instead of the regular 
		///	reloading POST arrangement. Even though it's 
		///	using Javascript to serialize the form elements, the form submission 
		///	will work just like a regular submission as viewed by the 
		///	receiving side (all elements available in @params).
		///	The options for specifying the target with :url and defining 
		///	callbacks is the same as link_to_remote.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public String BuildFormRemoteTag(IDictionary options)
		{
			options["form"] = true;

			String remoteFunc = RemoteFunction(options);

			String formId = options.Contains("formId") ? ("id=\"" + (String)options["formId"] + "\"") : String.Empty;

			return String.Format("<form {1} onsubmit=\"{0}; return false;\" enctype=\"multipart/form-data\">", remoteFunc, formId);
		}

		public IDictionary GetOptions(String url, IDictionary options)
		{
			if (options == null)
			{
				options = new HybridDictionary();
			}
			
			options["form"] = true;
			options["url"] = url;
			
			return options;
		}

		public IDictionary GetOptions(String url, String idOfElementToBeUpdated, String with, String loading, String loaded, String complete, String interactive)
		{
			IDictionary options = new HybridDictionary();
	
			options["form"] = true;
			options["url"] = url;
			//	options["method"] = method;
	
			if (with != null && with.Length > 0) options["with"] = with;
			if (idOfElementToBeUpdated != null && idOfElementToBeUpdated.Length > 0) options["update"] = idOfElementToBeUpdated;
			if (loading != null && loading.Length > 0) options["Loading"] = loading;
			if (loaded != null && loaded.Length > 0) options["Loaded"] = loaded;
			if (complete != null && complete.Length > 0) options["Complete"] = complete;
			if (interactive != null && interactive.Length > 0) options["Interactive"] = interactive;

			return options;
		}

		/// <summary>
		/// Observes the field with the DOM ID specified by +field_id+ and makes
		/// an Ajax when its contents have changed.
		/// 
		/// Required +options+ are:
		/// 
		/// <tt>:frequency</tt>:: The frequency (in seconds) at which changes to
		///                       this field will be detected.
		/// <tt>:url</tt>::       +url_for+-style options for the action to call
		///                       when the field has changed.
		/// 
		/// Additional options are:
		/// <tt>:update</tt>::    Specifies the DOM ID of the element whose 
		///                       innerHTML should be updated with the
		///                       XMLHttpRequest response text.
		/// <tt>:with</tt>::      A Javascript expression specifying the
		///                       parameters for the XMLHttpRequest. This defaults
		///                       to 'value', which in the evaluated context 
		///                       refers to the new field value.
		///
		/// Additionally, you may specify any of the options documented in
		/// LinkToRemote
		/// </summary>
		/// <returns></returns>
		public String ObserveField(String fieldId, int frequency, String url, String idOfElementToBeUpdated, String with)
		{
			IDictionary options = new HybridDictionary();
			options["frequency"] = frequency;
			options["url"] = url;
			
			if (idOfElementToBeUpdated != null) options["update"] = idOfElementToBeUpdated;
			if (with != null) options["with"] = with;

			return BuildObserver("Form.Element.Observer", fieldId, options);
		}

		/// <summary>
		/// Observes the field with the DOM ID specified by +field_id+ and makes
		/// an Ajax when its contents have changed.
		/// 
		/// Required +options+ are:
		/// 
		/// <tt>:frequency</tt>:: The frequency (in seconds) at which changes to
		///                       this field will be detected.
		/// <tt>:url</tt>::       +url_for+-style options for the action to call
		///                       when the field has changed.
		/// 
		/// Additional options are:
		/// <tt>:update</tt>::    Specifies the DOM ID of the element whose 
		///                       innerHTML should be updated with the
		///                       XMLHttpRequest response text.
		/// <tt>:with</tt>::      A Javascript expression specifying the
		///                       parameters for the XMLHttpRequest. This defaults
		///                       to 'value', which in the evaluated context 
		///                       refers to the new field value.
		///
		/// Additionally, you may specify any of the options documented in
		/// LinkToRemote
		/// </summary>
		/// <param name="fieldId"></param>
		/// <param name="frequency"></param>
		/// <param name="url"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public String ObserveField(String fieldId, int frequency, String url, IDictionary options)
		{
			options["url"] = url;
			options["frequency"] = frequency;
			return BuildObserver("Form.Element.Observer", fieldId, options);
		}

		/// <summary>
		/// Like <see cref="ObserveField"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField"/>, except 
		/// the default value of the <tt>:with</tt> option evaluates to the
		/// serialized (request String) value of the form.
		/// </summary>
		/// <param name="formId"></param>
		/// <param name="frequency"></param>
		/// <param name="idOfElementToBeUpdated"></param>
		/// <param name="url"></param>
		/// <param name="with"></param>
		/// <returns></returns>
		public String ObserveForm(String formId, int frequency, String url, String idOfElementToBeUpdated, String with)
		{
			IDictionary options = new HybridDictionary();
			options["frequency"] = frequency;
			options["url"] = url;

			if (idOfElementToBeUpdated != null && idOfElementToBeUpdated.Length > 0) options["update"] = idOfElementToBeUpdated;
			if (with != null && with.Length > 0) options["with"] = with;

			return ObserveForm(formId, options);
		}

		/// <summary>
		/// Like <see cref="ObserveField"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField"/>, except 
		/// the default value of the <tt>:with</tt> option evaluates to the
		/// serialized (request String) value of the form.
		/// </summary>
		/// <param name="formId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public String ObserveForm(String formId, IDictionary options)
		{
			return BuildObserver("Form.Observer", formId, options);
		}

		/// <summary>
		/// Periodically invokes the specified url. You can use the options to 
		/// override the default frequency (defaults to 10 seconds)
		/// </summary>
		/// <param name="url">url to be invoked periodically</param>
		/// <param name="options"></param>
		/// <returns></returns>
		public String PeriodicallyCallRemote(String url, IDictionary options)
		{
			if (options == null)
			{
				options = new HybridDictionary();
			}
				
			if (!options.Contains("frequency"))
			{
				options["frequency"] = "10";
			}

			String code = String.Format("new PeriodicalExecuter(function() {{ {0}, {1} }})", 
				BuildRemoteFunction(url, options), options["frequency"]);

			return String.Format( "<script>{0}</script>", code );
		}

		/// <summary>
		/// Rendes a input field with Google style autocomplete enabled.
		/// The specified url is used to gather the contents for the auto complete panel, so 
		/// and your action should return filtered and sorted results.
		/// <seealso cref="AutoCompleteInputText"/>
		/// </summary>
		/// <param name="inputName">input element id</param>
		/// <param name="url">url used to gather results</param>
		/// <param name="tagAttributes">attributes for the input element</param>
		/// <param name="completionOptions">options for the autocomplete</param>
		/// <returns></returns>
		public String InputTextWithAutoCompletion(String inputName, String url, IDictionary tagAttributes, IDictionary completionOptions)
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat( "<input type=\"text\" autocomplete=\"off\" name=\"{0}\" id=\"{0}\" {1}/>",
				inputName, GetAttributes(tagAttributes) );

			sb.AppendFormat( "<div id=\"{0}\" class=\"auto_complete\"></div>", inputName + "autocomplete" );

			sb.Append( AutoCompleteInputText( inputName, url, completionOptions ) );

			return sb.ToString();
		}

		/// <summary>
		/// Generates an javascript block enabling 
		/// auto completion for the specified input text id (<c>elementId</c>). 
		/// You can specify the element to be updated using the options
		/// dictionary (<c>key 'update'</c>), if you don't we assume 
		/// <c>elementId + "autocomplete"</c>.
		/// </summary>
		/// <remarks>
		/// It's assumed that the url invoked returns a unordered list.
		/// </remarks>
		/// <param name="elementId">The element id (input type=text)</param>
		/// <param name="url">The url to be invoked returning results</param>
		/// <param name="options">Custom options</param>
		/// <returns></returns>
		public String AutoCompleteInputText( String elementId, String url, IDictionary options )
		{
			if (options == null)
			{
				options = new HybridDictionary();
			}

			StringBuilder sb = new StringBuilder();
			
			String update = (String) options["update"];

			if (update == null)
			{
				update = elementId + "autocomplete";
			}

			sb.Append("<script type=\"text/javascript\">");

			sb.AppendFormat( "new Ajax.Autocompleter('{0}', '{1}', '{2}'", elementId, update, url );

			if (options.Contains("tokens"))
			{
				String[] tokens = options["tokens"].ToString().Split('|');

				if (tokens.Length == 0)
				{
					options.Remove("tokens");
				}
				else if (tokens.Length == 1)
				{
					options["tokens"] = tokens[0];
				}
				else
				{
					StringBuilder content = new StringBuilder("new Array(");
					
					foreach(String tok in tokens)
					{
						content.Append('\'').Append(tok).Append("\',");
					}
					
					if(tokens.Length > 0)
						content.Remove( content.Length - 1, 1); // removing extra comma
					
					content.Append(')');
					
					options["tokens"] = content.ToString();
				}
			}

			if (options.Contains("with"))
			{
				options["callback"] = String.Format( "function(element, value) { return {0} }", options["with"] );
			}

			if (options.Contains("indicator"))
			{
				options["indicator"] = String.Format( "'{0}'", options["indicator"] );
			}

			sb.Append( "," );

			sb.Append( JavascriptOptions(options) );

			sb.Append( ")" );

			sb.Append("</script>");

			return sb.ToString();
		}

		#region Supporting methods

		public String BuildRemoteFunction(String url, IDictionary options)
		{
			if (options == null)
			{
				options = new HybridDictionary();
			}

			options["url"] = url;

			return RemoteFunction(options);
		}

		public String RemoteFunction(IDictionary options)
		{
			IDictionary jsOptions = new HybridDictionary();

			String javascriptOptionsString = BuildAjaxOptions(jsOptions, options);

			StringBuilder contents = new StringBuilder();

			bool isRequestOnly = !options.Contains("update") && 
				!options.Contains("success") && !options.Contains("failure");

			if (isRequestOnly)
			{
				contents.Append( "new Ajax.Request(" );
			}
			else
			{
				contents.Append( "new Ajax.Updater(" );

				if (options.Contains("update"))
				{
					contents.AppendFormat( "'{0}', ", options["update"] );
				}
				else
				{
					contents.Append("{");

					bool commaFirst = false;

					if (options.Contains("success"))
					{
						contents.AppendFormat( "success:'{0}'", options["success"] );
						commaFirst = true;
					}
					if (options.Contains("failure"))
					{
						if (commaFirst) contents.Append(",");
						contents.AppendFormat( "failure:'{0}'", options["failure"] );
					}

					contents.Append("}, ");
				}
			}

			if (!options.Contains("url")) throw new ArgumentException("url is required");

			contents.Append( GetUrlOption(options) );
			contents.Append( ", " + javascriptOptionsString + ")" );

			if (options.Contains("before"))
			{
				contents = new StringBuilder( String.Format("{0}; {1}", 
					options["before"].ToString(), contents.ToString()) );
			}

			if (options.Contains("after"))
			{
				contents = new StringBuilder( String.Format("{1}; {0}", 
					options["after"].ToString(), contents.ToString()) );
			}

			if (options.Contains("condition"))
			{
				String old = contents.ToString();
				contents = new StringBuilder( 
					String.Format("if ( {0} ) {{ {1}; }}", options["condition"], old) );
			}

			return contents.ToString();
		}

		private String GetUrlOption(IDictionary options)
		{
			String url = (String) options["url"];

			if (url.StartsWith("<") && url.EndsWith(">"))
			{
				return url.Substring(1, url.Length - 2);
			}

			return "'" + url + "'";
		}

		protected String BuildAjaxOptions(IDictionary jsOptions, IDictionary options)
		{
			BuildCallbacks(jsOptions, options);

			jsOptions["asynchronous"] = (!options.Contains("type")).ToString().ToLower();

			if (options.Contains("method"))
			{
				jsOptions["method"] = options["method"];
			}

			if (options.Contains("evalScripts"))
			{
				jsOptions["evalScripts"] = options["evalScripts"];
			}
			else
			{
				jsOptions["evalScripts"] = "true";
			}

			if (options.Contains("position"))
			{
				options["insertion"] = String.Format("Insertion.{0}", options["position"]);
			}

			if (options.Contains("form"))
			{
				jsOptions["parameters"] = "Form.serialize(this)";
			}
			else if (options.Contains("with"))
			{
				jsOptions["parameters"] = options["with"];
			}

			return JavascriptOptions(jsOptions);
		}

		private String JavascriptOptions(IDictionary jsOptions)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("{");
			bool comma = false;
	
			foreach(DictionaryEntry entry in jsOptions)
			{
				if (!comma)
					comma = true;
				else
					sb.Append(", ");

				sb.Append( String.Format("{0}:{1}", entry.Key, entry.Value) );
			}
	
			sb.Append("}");
			return sb.ToString();
		}

		private void BuildCallbacks(IDictionary jsOptions, IDictionary options)
		{
			String[] names = CallbackEnum.GetNames( typeof(CallbackEnum) );
			
			foreach(String name in names)
			{
				if (!options.Contains(name.ToLower())) continue;

				String callbackFunctionName;

				String function = BuildCallbackFunction( 
					(CallbackEnum) Enum.Parse( typeof(CallbackEnum), name, true), 
					options[name.ToLower()] as String, out callbackFunctionName );

				if (function == String.Empty) return;

				jsOptions[callbackFunctionName] = function;
			}
		}

		protected String BuildCallbackFunction(CallbackEnum callback, String code, out String name)
		{
			name = String.Empty;

			if (callback == CallbackEnum.Uninitialized) return String.Empty;

			name = "on" + callback.ToString();

			return String.Format("function(request) {{ {0} }} ", code);
		}

		protected String BuildObserver(String clazz, String name, IDictionary options)
		{
			if (options.Contains("update") && !options.Contains("with"))
			{
				options["with"] = "value";
			}

			String call = RemoteFunction(options);

			StringBuilder js = new StringBuilder();

			js.Append( "<script type=\"text/javascript\">" );
			js.Append( String.Format("new {0}('{1}', {2}, function(element, value) {{ {3} }})", 
				clazz, name, options["frequency"], call) );
			js.Append( "</script>" ); 

			return js.ToString();
		}

		#endregion
	}
}
