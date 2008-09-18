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
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;
	using Castle.Core;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Services.AjaxProxyGenerator;

	/// <summary>
	/// XmlHttpRequest supported events.
	/// </summary>
	public enum CallbackEnum
	{
		/// <summary>
		/// Not initialized
		/// </summary>
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
		Complete,
		/// <summary>
		/// Called when the request was successfully (Status code &lt; 500)
		/// </summary>
		OnSuccess,
		/// <summary>
		/// Called when the request was not successfully (Status code &gt;= 500)
		/// </summary>
		OnFailure
	}

	/// <summary>
	/// MonoRail Helper that delivers AJAX capabilities.
	/// </summary>
	/// <remarks>
	/// The following libraries are exposed:
	/// <list type="table">
	/// <item><term> Prototype </term>
	/// <description> Simplify ajax programming, among other goodies 
	/// </description></item>
	/// <item><term> Behaviour </term>
	/// <description> Uses css selectors to bind javascript code to DOM elements 
	/// </description></item>
	/// </list>
	/// </remarks>
	public class AjaxHelper : AbstractHelper, IServiceEnabledComponent
	{
		private IAjaxProxyGenerator ajaxProxyGenerator;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AjaxHelper"/> class.
		/// </summary>
		public AjaxHelper() { }
		/// <summary>
		/// Initializes a new instance of the <see cref="AjaxHelper"/> class.
		/// setting the Controller, Context and ControllerContext.
		/// </summary>
		/// <param name="engineContext">The engine context.</param>
		public AjaxHelper(IEngineContext engineContext) : base(engineContext) { }
		#endregion

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ajaxProxyGenerator = (IAjaxProxyGenerator) provider.GetService(typeof(IAjaxProxyGenerator));
		}

		#endregion

		#region Scripts

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		public String InstallScripts()
		{
			return RenderScriptBlockToSource("/MonoRail/Files/AjaxScripts");
		}

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		[Obsolete("Please use the preferred InstallScripts function.")]
		public String GetJavascriptFunctions()
		{
			return InstallScripts();
		}

		#endregion

		#region GenerateJSProxy overloads

		/// <summary>
		/// Generates an AJAX JavaScript proxy for the current controller.
		/// </summary>
		/// <param name="proxyName">Name of the javascript proxy object</param>
		public String GenerateJSProxy(string proxyName)
		{
			return GenerateJSProxy(proxyName, ControllerContext.AreaName, ControllerContext.Name);
		}

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// </summary>
		/// <param name="proxyName">Name of the javascript proxy object</param>
		/// <param name="controller">Controller which will be target of the proxy</param>
		public String GenerateJSProxy(string proxyName, string controller)
		{
			return GenerateJSProxy(proxyName, String.Empty, controller);
		}

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// </summary>
		/// <param name="proxyName">Name of the javascript proxy object</param>
		/// <param name="controller">Controller which will be target of the proxy</param>
		/// <param name="area">area which the controller belongs to</param>
		public String GenerateJSProxy(string proxyName, string area, string controller)
		{
			return ajaxProxyGenerator.GenerateJSProxy(CurrentContext, proxyName, area, controller);
		}
		
		#endregion
		
		#region LinkToFunction
		
		/// <summary>
		/// Returns a link that will trigger a javascript function using the 
		/// onclick handler and return false after the fact.
		/// <code>
		/// &lt;a href="javascript:void(0);" onclick="functionCodeOrName; return false"&gt;innerContent&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <param name="attributes">Attributes to be applied to the html element</param>
		/// <returns></returns>
		public String LinkToFunction(String innerContent, String functionCodeOrName, IDictionary attributes)
		{
			String htmlAtt = GetAttributes(attributes);

			return String.Format("<a href=\"javascript:void(0);\" {2} onclick=\"{0}; return false;\" >{1}</a>", functionCodeOrName, innerContent, htmlAtt );
		}

		/// <summary>
		/// Returns a link that will trigger a javascript function using the 
		/// onclick handler and return false after the fact.
		/// <code>
		/// &lt;a href="javascript:void(0);" onclick="confirm('question') { functionCodeOrName}; return false"&gt;innerContent&lt;/a&gt;
		/// </code>
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <param name="confirm">Confirm question</param>
		/// <param name="attributes">Attributes to be applied to the html element</param>
		/// <returns></returns>
		public String LinkToFunction(String innerContent, String functionCodeOrName, string confirm, IDictionary attributes)
		{
			String htmlAtt = GetAttributes(attributes);

			return String.Format("<a href=\"javascript:void(0);\" {2} onclick=\"if(confirm('" + confirm + "')){{{0}}};return false;\" >{1}</a>", functionCodeOrName, innerContent, htmlAtt);
		}

		/// <summary>
		/// Returns a link that will trigger a javascript function using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <returns></returns>
		public String LinkToFunction(String innerContent, String functionCodeOrName)
		{
			return LinkToFunction(innerContent, functionCodeOrName, new Hashtable());
		}

		/// <summary>
		/// Returns a link that will trigger a javascript function using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="functionCodeOrName">Function definition</param>
		/// <param name="confirm">Confirm question</param>
		/// <returns></returns>
		public String LinkToFunction(String innerContent, String functionCodeOrName, String confirm)
		{
			return LinkToFunction(innerContent, functionCodeOrName, confirm, null);
		}
		
		#endregion

		#region ButtonToFunction
		
		/// <summary>
		/// Returns a button that will trigger a javascript function using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="functionCodeOrName">Function definition or name</param>
		/// <param name="attributes">Attributes to be applied to the input html element</param>
		/// <returns></returns>
		public String ButtonToFunction(String innerContent, String functionCodeOrName, IDictionary attributes) 
		{
			String htmlAtt = GetAttributes(attributes);

			return String.Format("<input type=\"button\" {2} onclick=\"{0}; return false;\" value=\"{1}\" />",
				functionCodeOrName, innerContent, htmlAtt);
		}

		/// <summary>
		/// Returns a button that will trigger a javascript function using the 
		/// onclick handler and return false after the fact.
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="functionCodeOrName">Function definition or name</param>
		/// <returns></returns>
		public String ButtonToFunction(String innerContent, String functionCodeOrName) 
		{
			return ButtonToFunction(innerContent, functionCodeOrName, null);
		}
		
		#endregion
		
		#region ButtonToRemote

		/// <summary>
		/// Creates a button that if clicked will fire an Ajax invocation. 
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="url">The URL of the Ajax action</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>The handcrafted input</returns>
		public String ButtonToRemote(String innerContent, String url, IDictionary options)
		{
			return ButtonToFunction(innerContent, BuildRemoteFunction(url, options));
		}

		/// <summary>
		/// Creates a button that if clicked will fire an Ajax invocation. 
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="url">the target url</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <param name="htmloptions">Attributes to be applied to the html element</param>
		/// <returns>The handcrafted input</returns>
		public String ButtonToRemote(String innerContent, String url, IDictionary options, IDictionary htmloptions)
		{
			return ButtonToFunction(innerContent, BuildRemoteFunction(url, options), htmloptions);
		}
		
		#endregion
		
		#region LinkToRemote

		/// <summary>
		/// Returns a link to a remote action defined by <c>options["url"]</c> 
		/// that is called in the background using 
		/// XMLHttpRequest. The result of that request can then be inserted into a
		/// DOM object whose id can be specified with <c>options["update"]</c>. 
		/// Usually, the result would be a partial prepared by the controller
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="url">Target url</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>The handcrafted element</returns>
		public String LinkToRemote(String innerContent, String url, IDictionary options)
		{
			return LinkToFunction(innerContent, BuildRemoteFunction(url, options));
		}

		/// <summary>
		/// Returns a link to a remote action defined by <c>options["url"]</c> 
		/// that is called in the background using 
		/// XMLHttpRequest. The result of that request can then be inserted into a
		/// DOM object whose id can be specified with <c>options["update"]</c>. 
		/// Usually, the result would be a partial prepared by the controller
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="confirm">the confirm question</param>
		/// <param name="url">Target url</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>The handcrafted element</returns>
		public String LinkToRemote(String innerContent, String confirm, String url, IDictionary options)
		{
			return LinkToFunction(innerContent, BuildRemoteFunction(url, options), confirm, null);
		}

		/// <summary>
		/// Returns a link to a remote action defined by <c>options["url"]</c> 
		/// that is called in the background using 
		/// XMLHttpRequest. The result of that request can then be inserted into a
		/// DOM object whose id can be specified with <c>options["update"]</c>. 
		/// Usually, the result would be a partial prepared by the controller
		/// </summary>
		/// <param name="innerContent">Link content</param>
		/// <param name="url">Target url</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <param name="htmloptions">Attributes to be applied to the html element</param>
		/// <returns>The handcrafted element</returns>
		public String LinkToRemote(String innerContent, String url, IDictionary options, IDictionary htmloptions)
		{
			return LinkToFunction(innerContent, BuildRemoteFunction(url, options), htmloptions);
		}
		
		#endregion

		#region BuildFormRemoteTag
		
		/// <summary>
		/// Returns a form tag that will submit using XMLHttpRequest 
		/// in the background instead of the regular 
		///	reloading POST arrangement. Even though it is
		///	using Javascript to serialize the form elements, the form submission 
		///	will work just like a regular submission as viewed by the 
		///	receiving side (all elements available).
		/// </summary>
		/// <param name="url">Target url</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>The handcrafted element</returns>
		public String BuildFormRemoteTag(String url, IDictionary options)
		{
			return BuildFormRemoteTag( GetOptions(url, options) );
		}

		/// <summary>
		/// Returns a form tag that will submit using XMLHttpRequest 
		/// in the background instead of the regular 
		///	reloading POST arrangement. Even though it is
		///	using Javascript to serialize the form elements, the form submission 
		///	will work just like a regular submission as viewed by the 
		///	receiving side (all elements available).
		/// </summary>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>The handcrafted element</returns>
		public String BuildFormRemoteTag(IDictionary options)
		{
			options["form"] = true;

			String remoteFunc = RemoteFunction(options);

			String formId = options.Contains("formId") ? ("id=\"" + (String) options["formId"] + "\"") : String.Empty;

			return String.Format("<form {1} onsubmit=\"{0}; return false;\" enctype=\"multipart/form-data\" action=\"{2}\" method=\"post\" >", remoteFunc, formId, options["url"]);
		}
		
		#endregion

		#region ObserveField

		/// <summary>
		/// Observes the field with the DOM ID specified by <c>fieldId</c> and makes
		/// an Ajax when its contents have changed.
		/// </summary>
		/// <param name="fieldId">Form field to be observed</param>
		/// <param name="frequency">The frequency (in seconds) at which changes to
		///                       this field will be detected. (required)</param>
		/// <param name="url">url for the action to call
		///                       when the field has changed (required)</param>
		/// <param name="idOfElementToBeUpdated"> Specifies the DOM ID of the element whose 
		///                       innerHTML should be updated with the
		///                       XMLHttpRequest response text.</param>
		/// <param name="with">A Javascript expression specifying the
		///                       parameters for the XMLHttpRequest. This defaults
		///                       to 'value', which in the evaluated context 
		///                       refers to the new field value.</param>
		/// <returns>javascript that activates the observer</returns>
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
		/// Observes the field with the DOM ID specified by <c>fieldId</c> and makes
		/// an Ajax when its contents have changed.
		/// </summary>
		/// <param name="fieldId">Form field to be observed</param>
		/// <param name="frequency">The frequency (in seconds) at which changes to
		///                       this field will be detected. (required)</param>
		/// <param name="url">url for the action to call
		///                       when the field has changed (required)</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript that activates the observer</returns>
		public String ObserveField(String fieldId, int frequency, String url, IDictionary options)
		{
			options["url"] = url;
			options["frequency"] = frequency;
			return BuildObserver("Form.Element.Observer", fieldId, options);
		}
		
		/// <summary>
		/// Observes the field with the DOM ID specified by <c>field</c> and makes
		/// an Ajax call when its contents changes.
		/// <para>
		/// The following entries must exist in the dictionary:
		/// </para>
		/// <list type="bullet">
		/// <item>
		/// <term>field</term>
		/// <description>The DOM field to be observed</description>
		/// </item>
		/// <item>
		/// <term>url</term>
		/// <description>url to to call when the field has changed</description>
		/// </item>
		/// <item>
		/// <term>frequency</term>
		/// <description>The frequency (in seconds) at which changes to this field will be detected</description>
		/// </item>
		/// </list>
		/// <para>
		/// The following are optional entries:
		/// </para>
		/// <list type="bullet">
		/// <item>
		/// <term>update</term>
		/// <description>Specifies the DOM ID of the element whose  
		/// innerHTML should be updated with the 
		/// XMLHttpRequest response text</description>
		/// </item>
		/// <item>
		/// <term>with</term>
		/// <description>A Javascript expression specifying the parameters 
		/// for the XMLHttpRequest. This defaults to 'value', which in the 
		/// evaluated context  refers to the new field value</description>
		/// </item>
		/// </list>
		/// </summary>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript that activates the observer</returns>
		public String ObserveField(IDictionary options)
		{
			return BuildObserver("Form.Element.Observer", (String) options["field"], options);
		}
		
		#endregion
		
		#region ObserveForm

		/// <summary>
		/// Like <see cref="ObserveField(IDictionary)"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField(IDictionary)"/>, except 
		/// the default value of the <tt>:with</tt> option evaluates to the
		/// serialized (request String) value of the form.
		/// Works like the <see cref="ObserveField(IDictionary)"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. Options are the same as <see cref="ObserveField(IDictionary)"/>, except 
		/// the default value of the <c>with</c> option evaluates to the
		/// serialized (request String) value of the entire form.
		/// </summary>
		/// <param name="formId">Form to be observed</param>
		/// <param name="frequency">The frequency (in seconds) at which changes to
		///                       this field will be detected. (required)</param>
		/// <param name="url">url for the action to call
		///                       when the field has changed (required)</param>
		/// <param name="idOfElementToBeUpdated"> Specifies the DOM ID of the element whose 
		///                       innerHTML should be updated with the
		///                       XMLHttpRequest response text.</param>
		/// <param name="with">A Javascript expression specifying the
		///                       parameters for the XMLHttpRequest. This defaults
		///                       to 'value', which in the evaluated context 
		///                       refers to the new field value.</param>
		/// <returns>javascript that activates the observer</returns>
		public String ObserveForm(String formId, int frequency, String url, String idOfElementToBeUpdated, object with)
		{
			IDictionary options = new HybridDictionary();
			options["frequency"] = frequency;
			options["url"] = url;

			if (idOfElementToBeUpdated != null && idOfElementToBeUpdated.Length > 0) options["update"] = idOfElementToBeUpdated;
			if (with != null) options["with"] = ProcessWith(with);

			return ObserveForm(formId, options);
		}

		/// <summary>
		/// Like <see cref="ObserveField(IDictionary)"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField(IDictionary)"/>, except 
		/// the default value of the <c>with</c> option evaluates to the
		/// serialized (request String) value of the entire form.
		/// </summary>
		/// <param name="formId">Form to be observed</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript that activates the observer</returns>
		public String ObserveForm(String formId, IDictionary options)
		{
			return BuildObserver("Form.Observer", formId, options);
		}
		
		/// <summary>
		/// Observes all elements within a form with the DOM 
		/// ID specified by <c>form</c> and makes
		/// an Ajax call when its contents changes.
		/// <para>
		/// The following entries must exist in the dictionary:
		/// </para>
		/// <list type="bullet">
		/// <item>
		/// <term>form</term>
		/// <description>The form element id</description>
		/// </item>
		/// <item>
		/// <term>url</term>
		/// <description>url to to call when the field has changed</description>
		/// </item>
		/// <item>
		/// <term>frequency</term>
		/// <description>The frequency (in seconds) at which changes to this field will be detected</description>
		/// </item>
		/// </list>
		/// <para>
		/// The following are optional entries:
		/// </para>
		/// <list type="bullet">
		/// <item>
		/// <term>update</term>
		/// <description>Specifies the DOM ID of the element whose  
		/// innerHTML should be updated with the 
		/// XMLHttpRequest response text</description>
		/// </item>
		/// <item>
		/// <term>with</term>
		/// <description>A Javascript expression specifying the parameters 
		/// for the XMLHttpRequest. This defaults to 'value', which in the 
		/// evaluated context  refers to the new field value</description>
		/// </item>
		/// </list>
		/// </summary>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript that activates the observer</returns>
		public String ObserveForm(IDictionary options)
		{
			String formId = (String) options["form"];
			
			if (!options.Contains("with"))
			{
				options["with"] = "Form.serialize(" + formId + ")";
			}		
			
			return BuildObserver("Form.Observer", formId, options);
		}
		
		#endregion
		
		#region Periodically Call

		/// <summary>
		/// Periodically invokes the specified <c>url</c>. You can use the options to 
		/// override the default <c>frequency</c> (defaults to 10 seconds).
		/// </summary>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript that activates the timer</returns>
		public String PeriodicallyCallRemote(IDictionary options)
		{
			String url = (String) options["url"];
			
			options.Remove("url");
			
			return PeriodicallyCallRemote(url, options);
		}
		
		/// <summary>
		/// Periodically invokes the specified <c>url</c>. You can use the options to 
		/// override the default <c>frequency</c> (defaults to 10 seconds).
		/// </summary>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <param name="url">url to be invoked periodically</param>
		/// <returns>javascript that activates the timer</returns>
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

			String code = String.Format("new PeriodicalExecuter(function() {{ {0} }}, {1} )", 
				BuildRemoteFunction(url, options), options["frequency"]);

			return String.Format( "<script type='text/javascript'>{0}</script>", code );
		}

		#endregion
		
		#region AutoCompletion
		
		/// <summary>
		/// Renders an input field with Google style autocomplete enabled.
		/// The specified <c>url</c> is used to gather the contents 
		/// for the auto complete panel, so 
		/// and your action should return filtered and sorted results.
		/// <para>
		/// The following entries must exist in the options:
		/// </para>
		/// <list type="bullet">
		/// <item>
		/// <term>input</term>
		/// <description>The text input element id</description>
		/// </item>
		/// <item>
		/// <term>url</term>
		/// <description>url to to call when the field has changed</description>
		/// </item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// it is assumed that the url invoked returns an unordered list.
		/// </remarks>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <param name="tagAttributes">attributes for the input html element</param>
		/// <returns>javascript that activates the timer</returns>
		public String InputTextWithAutoCompletion(IDictionary options, IDictionary tagAttributes)
		{
			String input = (String) options["input"];
			String url = (String) options["url"];
			
			options.Remove("input"); 
			options.Remove("url");
			
			return InputTextWithAutoCompletion(input, url, tagAttributes, options);
		}
		
		/// <summary>
		/// Rendes a input field with Google style autocomplete enabled.
		/// The specified url is used to gather the contents for the auto complete panel, so 
		/// and your action should return filtered and sorted results.
		/// <seealso cref="AutoCompleteInputText"/>
		/// </summary>
		/// <remarks>
		/// it is assumed that the url invoked returns an unordered list.
		/// </remarks>
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
		/// dictionary (key  <c>update</c>), if you don't we assume 
		/// <c>elementId+autocomplete</c>.
		/// </summary>
		/// <remarks>
		/// it is assumed that the url invoked returns an unordered list.
		/// </remarks>
		/// <param name="elementId">The element id (input type=text)</param>
		/// <param name="url">The url to be invoked returning results</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns></returns>
		public String AutoCompleteInputText(String elementId, String url, IDictionary options)
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
		
		#endregion

		#region Supporting methods

		/// <summary>
		/// Returns a function that makes a remote invocation,
		/// using the supplied parameters
		/// </summary>
		/// <param name="url">Target url</param>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript code</returns>
		public String BuildRemoteFunction(String url, IDictionary options)
		{
			if (options == null)
			{
				options = new HybridDictionary();
			}

			options["url"] = url;

			return RemoteFunction(options);
		}

		/// <summary>
		/// Returns a function that makes a remote invocation,
		/// using the supplied parameters
		/// </summary>
		/// <param name="options">the options for the Ajax invocation</param>
		/// <returns>javascript code</returns>
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
					options.Remove("update");
				}
				else
				{
					contents.Append("{");

					bool commaFirst = false;

					if (options.Contains("success"))
					{
						contents.AppendFormat( "success:'{0}'", options["success"] );
						commaFirst = true;
						options.Remove("success");
					}

					if (options.Contains("failure"))
					{
						if (commaFirst)
						{
							contents.Append(",");
						}
						contents.AppendFormat( "failure:'{0}'", options["failure"] );
						options.Remove("failure");
					}

					contents.Append("}, ");
				}
			}

			if (!options.Contains("url")) throw new ArgumentException("url is required");

			contents.Append( GetUrlOption(options) );
			contents.Append( ", " + javascriptOptionsString + ")" );

			if (options.Contains("before"))
			{
				contents = new StringBuilder( String.Format("{0}; {1}", options["before"], contents) );

				options.Remove("before");
			}

			if (options.Contains("after"))
			{
				contents = new StringBuilder( String.Format("{1}; {0}", options["after"], contents) );

				options.Remove("after");
			}

			if (options.Contains("condition"))
			{
				String old = contents.ToString();

				contents = new StringBuilder( 
					String.Format("if ( {0} ) {{ {1}; }}", options["condition"], old) );

				options.Remove("condition");
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

		/// <summary>
		/// Populates the <paramref name="jsOptions"/> by analyzing the
		/// options set on the helper <paramref name="options"/>
		/// </summary>
		/// 
		/// <remarks>
		/// The following options are analyzed
		/// 
		/// <list type="bullet">
		/// <item>
		///		<term>type</term>
		///		<description>boolean - sets the <c>asynchronous</c> </description>
		/// </item>
		/// <item>
		///		<term>method</term>
		///		<description>string - sets the <c>method</c>. Possible values are post/get </description>
		/// </item>
		/// <item>
		///		<term>evalScripts</term>
		///		<description>boolean</description>
		/// </item>
		/// <item>
		///		<term>position</term>
		///		<description>string - sets the place where content is inserted (Top, Bottom, After, Before)</description>
		/// </item>
		/// <item>
		///		<term>form</term>
		///		<description>if present, set the parameters request to send the current form serialized</description>
		/// </item>
		/// <item>
		///		<term>with</term>
		///		<description>if present, set its content as the parameters for the ajax request</description>
		/// </item>
		/// </list>
		/// 
		/// </remarks>
		/// 
		/// <param name="jsOptions">Options that will be used on the js side</param>
		/// <param name="options">Options passed to the helper method</param>
		/// <returns></returns>
		protected String BuildAjaxOptions(IDictionary jsOptions, IDictionary options)
		{
			BuildCallbacks(jsOptions, options);

			jsOptions["asynchronous"] = (!options.Contains("type")).ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);

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
				jsOptions["insertion"] = String.Format("Insertion.{0}", options["position"]);
			}

			if (!options.Contains("with") && options.Contains("form"))
			{
				jsOptions["parameters"] = "Form.serialize(this)";
			}
			else if (options.Contains("with"))
			{
				jsOptions["parameters"] = ProcessWith(options["with"]);
			}

			return JavascriptOptions(jsOptions);
		}

		private string ProcessWith(object with)
		{
			if (with == null) return null;

			if (with is IDictionary)
			{
				return SQuote(CommonUtils.BuildQueryString(ServerUtility, with as IDictionary, false));
			}

			return with.ToString();
		}

		private void BuildCallbacks(IDictionary jsOptions, IDictionary options)
		{
			String[] names = CallbackEnum.GetNames( typeof(CallbackEnum) );
			
			foreach(String name in names)
			{
				if (!options.Contains(name)) continue;

				String callbackFunctionName;

				String function = BuildCallbackFunction( 
					(CallbackEnum) Enum.Parse( typeof(CallbackEnum), name, true),
					options[name].ToString(), out callbackFunctionName);

				if (function == String.Empty) return;

				jsOptions[callbackFunctionName] = function;
			}
		}

		/// <summary>
		/// Builds the callback function.
		/// </summary>
		/// <param name="callback">The callback.</param>
		/// <param name="code">The code.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		protected String BuildCallbackFunction(CallbackEnum callback, String code, out String name)
		{
			name = String.Empty;

			if (callback == CallbackEnum.Uninitialized) return String.Empty;

			if (callback != CallbackEnum.OnFailure && callback != CallbackEnum.OnSuccess)
			{
				name = "on" + callback;
			}
			else if (callback == CallbackEnum.OnFailure)
			{
				name = "onFailure";
			}
			else if (callback == CallbackEnum.OnSuccess)
			{
				name = "onSuccess";
			}

			return String.Format("function(request) {{ {0} }} ", code);
		}

		/// <summary>
		/// Builds the observer.
		/// </summary>
		/// <param name="clazz">The clazz.</param>
		/// <param name="name">The name.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
		protected String BuildObserver(String clazz, String name, IDictionary options)
		{
			if (options.Contains("update") && !options.Contains("with"))
			{
				options["with"] = "'value=' + value";
			}

			String call = RemoteFunction(options);

			StringBuilder js = new StringBuilder();

			js.Append( "<script type=\"text/javascript\">" );
			js.Append( String.Format("new {0}('{1}', {2}, function(element, value) {{ {3} }})", 
				clazz, name, options["frequency"], call) );
			js.Append( "</script>" ); 

			return js.ToString();
		}

		/// <summary>
		/// Gets the options.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="options">The options.</param>
		/// <returns></returns>
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

		/// <summary>
		/// Gets the options.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="idOfElementToBeUpdated">The id of element to be updated.</param>
		/// <param name="with">The with.</param>
		/// <param name="loading">The loading.</param>
		/// <param name="loaded">The loaded.</param>
		/// <param name="complete">The complete.</param>
		/// <param name="interactive">The interactive.</param>
		/// <returns></returns>
		public IDictionary GetOptions(String url, String idOfElementToBeUpdated, 
		                              object with, String loading, String loaded, String complete, String interactive)
		{
			IDictionary options = new HybridDictionary();
	
			options["form"] = true;
			options["url"] = url;
			//	options["method"] = method;
	
			if (with != null) options["with"] = with;
			if (idOfElementToBeUpdated != null && idOfElementToBeUpdated.Length > 0) options["update"] = idOfElementToBeUpdated;
			if (loading != null && loading.Length > 0) options["Loading"] = loading;
			if (loaded != null && loaded.Length > 0) options["Loaded"] = loaded;
			if (complete != null && complete.Length > 0) options["Complete"] = complete;
			if (interactive != null && interactive.Length > 0) options["Interactive"] = interactive;

			return options;
		}

		#endregion
	}
}
