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
	using System.Text;
	using System.Reflection;
	using System.Resources;
	using System.Collections;

	public enum CallbackEnum
	{
		Uninitialized,
		/// <summary>
		/// Called when the remote document is being 
		/// loaded with data by the browser.
		/// </summary>
		Loading, 
		/// <summary>
		/// Called when the browser has finished loading
		/// the remote document.
		/// </summary>
		Loaded, 
		/// <summary>
		/// Called when the user can interact with the 
		/// remote document, even though it has not 
		/// finished loading.
		/// </summary>
		Interactive, 
		/// <summary>
		/// Called when the XMLHttpRequest has completed.
		/// </summary>
		Complete
	}


	public class AjaxHelper
	{
		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public String GetJavascriptFunctions()
		{
			ResourceManager rs = new ResourceManager(
				"Castle.CastleOnRails.Framework.Javascripts", 
				  Assembly.GetAssembly( typeof(AjaxHelper) ) );

			return rs.GetString("jsfunctions");
		}

		/// <summary>
		/// Returns a link that'll trigger a javascript +function+ using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="functionCodeOrName"></param>
		/// <param name="styleClass"></param>
		/// <returns></returns>
		public String LinkToFunction(String name, String functionCodeOrName, String styleClass)
		{
			return String.Format("<a href=\"#\" class=\"{2}\" onclick=\"{0}; return false;\" >{1}</a>", functionCodeOrName, name, styleClass );
		}

		/// <summary>
		/// Returns a link that'll trigger a javascript +function+ using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="functionCodeOrName"></param>
		/// <returns></returns>
		public String LinkToFunction(String name, String functionCodeOrName)
		{
			return String.Format("<a href=\"#\" onclick=\"{0}; return false;\" >{1}</a>", functionCodeOrName, name );
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
		/// <param name="options"></param>
		/// <returns></returns>
		public String LinkToRemote(String name, String url, IDictionary options)
		{
			options["url"] = url;
			return LinkToFunction(name, RemoteFunction(options) );
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
		public String BuildFormRemoteTag(String url, String idOfElementToBeUpdated, String with)
		{
			IDictionary options = new Hashtable();

			options["form"] = true;
			options["url"] = url;
//			options["method"] = method;

			if (idOfElementToBeUpdated != null) options["update"] = idOfElementToBeUpdated;
			if (with != null) options["with"] = with;

			return BuildFormRemoteTag(options);
		}

		public String BuildFormRemoteTag(String url, String idOfElementToBeUpdated, 
			String with, String loading, String loaded, String interactive, String complete)
		{
			IDictionary options = new Hashtable();

			options["form"] = true;
			options["url"] = url;
			//	options["method"] = method;

			if (idOfElementToBeUpdated != null) options["update"] = idOfElementToBeUpdated;
			if (with != null) options["with"] = with;
			if (loading != null) options["Loading"] = loading;
			if (loaded != null) options["Loaded"] = loaded;
			if (complete != null) options["Complete"] = complete;
			if (interactive != null) options["Interactive"] = interactive;

			return BuildFormRemoteTag(options);
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

			return String.Format("<form onsubmit=\"{0}; return false;\">", remoteFunc);
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
			IDictionary options = new Hashtable();
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
			return BuildObserver("Form.Element.Observer", fieldId, options);
		}

		/// <summary>
		/// Like <see cref="ObserveField"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField"/>, except 
		/// the default value of the <tt>:with</tt> option evaluates to the
		/// serialized (request string) value of the form.
		/// </summary>
		/// <param name="formId"></param>
		/// <returns></returns>
		public String ObserveForm(String formId, int frequency, String url, String idOfElementToBeUpdated, String with)
		{
			IDictionary options = new Hashtable();
			options["frequency"] = frequency;
			options["url"] = url;

			if (idOfElementToBeUpdated != null) options["update"] = idOfElementToBeUpdated;
			if (with != null) options["with"] = with;

			return ObserveForm(formId, options);
		}

		/// <summary>
		/// Like <see cref="ObserveField"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField"/>, except 
		/// the default value of the <tt>:with</tt> option evaluates to the
		/// serialized (request string) value of the form.
		/// </summary>
		/// <param name="formId"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public String ObserveForm(String formId, IDictionary options)
		{
			return BuildObserver("Form.Observer", formId, options);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		protected String RemoteFunction(IDictionary options)
		{
			IDictionary jsOptions = new Hashtable();

			String javascriptOptionsString = BuildAjaxOptions(jsOptions, options);

			StringBuilder contents = new StringBuilder();

			contents.Append( options.Contains("update") ? 
				"new Ajax.Updater('" + options["update"].ToString() + "', " : 
				"new Ajax.Request(" );

			if (!options.Contains("url")) throw new ArgumentException("url is required");

			contents.Append( "'" + options["url"] + "'" );
			contents.Append( ", " + javascriptOptionsString + ")" );

			if (options.Contains("before"))
			{
				contents = new StringBuilder( String.Format("{0}; {1}", options["before"], contents.ToString()) );
			}
			if (options.Contains("after"))
			{
				contents = new StringBuilder( String.Format("{1}; {0}", options["after"], contents.ToString()) );
			}
			if (options.Contains("condition"))
			{
				contents = new StringBuilder( String.Format("if ({0}) { {1}; }", options["condition"], contents.ToString()) );
			}

			return contents.ToString();
		}

		protected String BuildAjaxOptions(IDictionary jsOptions, IDictionary options)
		{
			BuildCallbacks(jsOptions, options);

			jsOptions["asynchronous"] = (!options.Contains("type")).ToString().ToLower();

			if (options.Contains("method"))
			{
				jsOptions["method"] = options["method"];
			}
			
			if (options.Contains("position"))
			{
				// "Insertion.#{options[:position].to_s.camelize}" if options[:position]
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

			// '{' + js_options.map {|k, v| "#{k}:#{v}"}.join(', ') + '}'

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
				if (!options.Contains(name)) continue;

				String callbackFunctionName;

				String function = BuildCallbackFunction( 
					(CallbackEnum) Enum.Parse( typeof(CallbackEnum), name), 
					options[name] as String, out callbackFunctionName );

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
	}
}
