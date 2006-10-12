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
	using System.Text;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Internal;

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
	/// <item><term>Prototype</term>
	/// <description>Simplify ajax programming, among other goodies 
	/// See also <a href="http://prototype.conio.net/"/>
	/// </description></item>
	/// <item><term>Behaviour</term>
	/// <description>Uses css selectors to bind javascript code to DOM elements 
	/// See also <a href="http://bennolan.com/behaviour/"/>
	/// </description></item>
	/// </list>
	/// </remarks>
	public class AjaxHelper : AbstractHelper, IServiceEnabledComponent
	{
		private static Hashtable ajaxProxyCache = Hashtable.Synchronized(new Hashtable());
		
		/// <summary>
		/// The logger instance
		/// </summary>
		private ILogger logger = NullLogger.Instance;
		
		/// <summary>
		/// Used by <c>GenerateJSProxy</c> overloads.
		/// </summary>
		private IControllerFactory controllerFactory;
		
		/// <summary>
		/// Used by <c>GenerateJSProxy</c> overloads.
		/// </summary>
		private IControllerDescriptorProvider controllerDescriptorBuilder;

		private bool behaviourCommaNeeded;

		#region IServiceEnabledComponent implementation
		
		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IServiceProvider provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));
			
			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(AjaxHelper));
			}
			
			controllerFactory = (IControllerFactory) 
				provider.GetService(typeof(IControllerFactory));
			
			controllerDescriptorBuilder = (IControllerDescriptorProvider) 
				provider.GetService(typeof(IControllerDescriptorProvider));
		}

		#endregion

		/// <summary>
		/// Renders a Javascript library inside a single script tag.
		/// </summary>
		/// <returns></returns>
		public String InstallScripts()
		{
			return String.Format("<script type=\"text/javascript\" src=\"{0}.{1}\"></script>", 
				Controller.Context.ApplicationPath + "/MonoRail/Files/AjaxScripts", 
				Controller.Context.UrlInfo.Extension);
		}

        [Obsolete("Please use the preferred InstallScripts function.")]
        public String GetJavascriptFunctions()
        {
            return InstallScripts();
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
		
		#region Behaviour library related
		
		/// <summary>
		/// Renders a script block invoking <c>Behaviour.apply()</c>
		/// </summary>
		public String ReApply()
		{
			return "<script type=\"text/javascript\">Behaviour.apply();</script>";
		}
		
		/// <summary>
		/// Renders a script block invoking <c>Behaviour.addLoadEvent(loadFunctionName);</c>
		/// </summary>
		/// <param name="loadFunctionName">The name of the js function to be invoked when the body is loaded</param>
		public String AddLoadEvent(String loadFunctionName)
		{
			return "<script type=\"text/javascript\">" + Environment.NewLine + 
				"Behaviour.addLoadEvent(" + loadFunctionName + ");" + Environment.NewLine + 
				"</script>";
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

		#endregion

		#region GenerateJSProxy overloads
		
		/// <summary>
		/// Generates an AJAX JavaScript proxy for the current controller.
		/// </summary>
		public String GenerateJSProxy(string proxyName)
		{
			return GenerateJSProxy(proxyName, Controller.AreaName, Controller.Name);
		}

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// <para>
		/// TODO: Better documentation
		/// </para>
		/// </summary>
		public String GenerateJSProxy(string proxyName, string controller)
		{
			return GenerateJSProxy(proxyName, null, controller);
		}

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// <para>
		/// TODO: Better documentation
		/// </para>
		/// </summary>
		public String GenerateJSProxy(string proxyName, string area, string controller)
		{
			String cacheKey = (area + "|" + controller).ToLower();
			String result = (String) ajaxProxyCache[cacheKey];

			if (result == null)
			{
				Controller controllerInstance = controllerFactory.CreateController(new UrlInfo("/", area, controller, "", ""));
				
				if (controllerInstance == null)
				{
					throw new RailsException("Controller not found with Area: '{0}', Name: '{1}'", area, controller);
				}
				
				String baseUrl = Controller.Context.ApplicationPath + "/";
				
				if (area != null && area != String.Empty)
				{
					baseUrl += area + "/";
				}
				
				baseUrl += controller + "/";
				
				// TODO: develop a smarter function generation, inspecting the return
				// value of the action and generating a proxy that does the same.
				// also, think on a proxy pattern for the Ajax.Updater.
				
				StringBuilder functions = new StringBuilder(1024);
				
				functions.Append("{ " + Environment.NewLine);
				
				ControllerMetaDescriptor metaDescriptor = 
					controllerDescriptorBuilder.BuildDescriptor(controllerInstance);
				
				bool commaNeeded = false;
				
				foreach(MethodInfo ajaxActionMethod in metaDescriptor.AjaxActions)
				{
					if (!commaNeeded) commaNeeded = true; else functions.Append(',');
					
					String methodName = ajaxActionMethod.Name;
					
					object[] attributes = ajaxActionMethod.GetCustomAttributes(typeof(AjaxActionAttribute), true);
					
					AjaxActionAttribute ajaxActionAtt = (AjaxActionAttribute) attributes[0];
					
					StringBuilder parameters = new StringBuilder("_=");
					String url = baseUrl + methodName + "." + Controller.Context.UrlInfo.Extension;
					String functionName = ajaxActionAtt.Name != null ? ajaxActionAtt.Name : methodName;
					
					functionName = Char.ToLower(functionName[0]) + (functionName.Length > 0 ? functionName.Substring(1) : null);

					functions.AppendFormat(Environment.NewLine + "\t{0}: " + Environment.NewLine + "\tfunction(", functionName);
					
					foreach(ParameterInfo pi in ajaxActionMethod.GetParameters())
					{
						String paramName = pi.Name;
						paramName = Char.ToLower(paramName[0]) + (paramName.Length > 0 ? paramName.Substring(1) : null);
						functions.AppendFormat("{0}, ", paramName);
						parameters.AppendFormat("\\x26{0}='+{0}+'", paramName);
					}
					
					functions.Append("callback)").Append(Environment.NewLine).Append("\t{");
					functions.Append(Environment.NewLine).AppendFormat("\t\tvar r=new Ajax.Request('{0}', " + 
						"{{parameters: '{1}', asynchronous: !!callback, onComplete: callback}}); " + 
						Environment.NewLine + 
						"\t\tif(!callback) return r.transport.responseText;", url, parameters.ToString());
					functions.Append("}").Append(Environment.NewLine);
				}
				
				functions.Length -= 1;
				functions.Append("}").Append(Environment.NewLine);

				ajaxProxyCache[cacheKey] = result = functions.ToString();
			}
			
			return @"<script type=""text/javascript"">var " + 
			       proxyName + " = " + Environment.NewLine + 
			       result + "</script>";
		}
		
		#endregion
		
		#region LinkToFunction
		
		/// <summary>
		/// Returns a link that'll trigger a javascript +function+ using the 
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
		
		#endregion

		#region ButtonToFunction
		
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
		
		#endregion
		
		#region ButtonToRemote

		/// <summary>
		/// Creates a button that if clicked will fire an Ajax invocation. 
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="url">The URL of the Ajax action</param>
		/// <param name="options">The options for the Ajax call</param>
		/// <returns>The handcrafted input</returns>
		public String ButtonToRemote(String innerContent, String url, IDictionary options)
		{
			return ButtonToFunction(innerContent, BuildRemoteFunction(url, options));
		}

		/// <summary>
		/// Creates a button that if clicked will fire an Ajax invocation. 
		/// </summary>
		/// <param name="innerContent">Button legend</param>
		/// <param name="url">the url</param>
		/// <param name="options">the options for the ajax invocations</param>
		/// <param name="htmloptions">Attributes to be applied to the html element</param>
		/// <returns>The handcrafted input</returns>
		public String ButtonToRemote(String innerContent, String url, IDictionary options, IDictionary htmloptions)
		{
			return ButtonToFunction(innerContent, BuildRemoteFunction(url, options), htmloptions);
		}
		
		#endregion
		
		#region LinkToRemote

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
		
		#endregion

		#region BuildFormRemoteTag
		
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
		
		#endregion

		#region ObserveField

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
		/// Like <see cref="ObserveField(IDictionary)"/>, but operates on an entire form identified by the
		/// DOM ID <c>formId</c>. options are the same as <see cref="ObserveField(IDictionary)"/>, except 
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
		/// Periodically invokes the specified url. You can use the options to 
		/// override the default frequency (defaults to 10 seconds)
		/// </summary>
		public String PeriodicallyCallRemote(IDictionary options)
		{
			String url = (String) options["url"];
			
			options.Remove("url");
			
			return PeriodicallyCallRemote(url, options);
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

			String code = String.Format("new PeriodicalExecuter(function() {{ {0} }}, {1} )", 
				BuildRemoteFunction(url, options), options["frequency"]);

			return String.Format( "<script>{0}</script>", code );
		}

		#endregion
		
		#region AutoCompletion
		
		/// <summary>
		/// Rendes a input field with Google style autocomplete enabled.
		/// The specified url is used to gather the contents for the auto complete panel, so 
		/// and your action should return filtered and sorted results.
		/// <para>
		/// The following entries must exist in the dictionary:
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
		public String InputTextWithAutoCompletion(IDictionary parameters, IDictionary tagAttributes)
		{
			String input = (String) parameters["input"];
			String url = (String) parameters["url"];
			
			parameters.Remove("input"); 
			parameters.Remove("url");
			
			return InputTextWithAutoCompletion(input, url, tagAttributes, parameters);
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
		
		#endregion

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
				jsOptions["insertion"] = String.Format("Insertion.{0}", options["position"]);
			}

			if (!options.Contains("with") && options.Contains("form"))
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

			if (callback != CallbackEnum.OnFailure && callback != CallbackEnum.OnSuccess)
			{
				name = "on" + callback.ToString();
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

		#endregion
	}
}
