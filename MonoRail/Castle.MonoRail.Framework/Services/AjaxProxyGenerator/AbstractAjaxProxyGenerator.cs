namespace Castle.MonoRail.Framework.Services.AjaxProxyGenerator
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using Configuration;
	using Core.Logging;
	using Descriptors;
	using Helpers;

	/// <summary>
	/// 
	/// </summary>
	public abstract class AbstractAjaxProxyGenerator : IMRServiceEnabled, IAjaxProxyGenerator
	{
		private readonly Hashtable ajaxProxyCache = Hashtable.Synchronized(new Hashtable());

		private static readonly Type
			ARFetchAttType =
				TypeLoadUtil.GetType(
					TypeLoadUtil.GetEffectiveTypeName(
						"Castle.MonoRail.ActiveRecordSupport.ARFetchAttribute, Castle.MonoRail.ActiveRecordSupport"), true);

		private IControllerDescriptorProvider controllerDescriptorBuilder;
		private IControllerTree controllerTree;
		private ILogger logger = NullLogger.Instance;

		/// <summary>
		/// Gets or sets the controller descriptor builder.
		/// </summary>
		/// <value>The controller descriptor builder.</value>
		public IControllerDescriptorProvider ControllerDescriptorBuilder
		{
			get { return controllerDescriptorBuilder; }
			set { controllerDescriptorBuilder = value; }
		}

		/// <summary>
		/// Gets or sets the controller tree.
		/// </summary>
		/// <value>The controller tree.</value>
		public IControllerTree ControllerTree
		{
			get { return controllerTree; }
			set { controllerTree = value; }
		}

		#region IAjaxProxyGenerator Members

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// </summary>
		/// <param name="context">The context of the current request</param>
		/// <param name="proxyName">Name of the javascript proxy object</param>
		/// <param name="controller">Controller which will be target of the proxy</param>
		/// <param name="area">area which the controller belongs to</param>
		public string GenerateJSProxy(IEngineContext context, string proxyName, string area, string controller)
		{
			string cacheKey = (area + "|" + controller).ToLower(CultureInfo.InvariantCulture);
			var result = (String) ajaxProxyCache[cacheKey];

			if (result != null)
			{
				return AbstractHelper.ScriptBlock(result);
			}

			logger.Debug("Ajax Proxy for area: '{0}', controller: '{1}' was not found. Generating a new one", area, controller);

			Type controllerType = controllerTree.GetController(area, controller);

			if (controllerType == null)
			{
				logger.Fatal("Controller not found for the area and controller specified");
				throw new MonoRailException("Controller not found with Area: '{0}', Name: '{1}'", area, controller);
			}

			string baseUrl = GetBaseUrl(area, controller, context);

			// TODO: develop a smarter function generation, inspecting the return
			// value of the action and generating a proxy that does the same.
			// also, consider a proxy pattern for the Ajax.Updater.
			ControllerMetaDescriptor metaDescriptor = controllerDescriptorBuilder.BuildDescriptor(controllerType);

			var functions = new StringBuilder();
			functions.AppendLine("{");

			for(int i = 0; i < metaDescriptor.AjaxActions.Count; i++)
			{
				MethodInfo ajaxActionMethod = metaDescriptor.AjaxActions[i];
				var ajaxActionAtt = GetSingleAttribute<AjaxActionAttribute>(ajaxActionMethod, true);

				string httpRequestMethod = GetHTTPRequestMethod(ajaxActionMethod);
				string extension = GetURLExtension(context);
				string url = baseUrl + ajaxActionMethod.Name + extension;
				string functionName = ToCamelCase(ajaxActionAtt.Name ?? ajaxActionMethod.Name);

				ScriptFunctionParameter[] scriptFunctionParameters = GetScriptFunctionParameters(ajaxActionMethod.GetParameters());

				functions.Append(
					GenerateJavascriptFunction(url, functionName, httpRequestMethod, scriptFunctionParameters)
					);

				// js functions are seperated by a comma
				if (metaDescriptor.AjaxActions.Count > 0 && i < metaDescriptor.AjaxActions.Count - 1) 
					functions.AppendLine(",");
			}

			functions.AppendLine("};");
			ajaxProxyCache[cacheKey] = "var " + proxyName + " =" + Environment.NewLine + functions;

			return GenerateJSProxy(context, proxyName, area, controller);
		}

		#endregion

		/// <summary>
		/// Gets the script function parameters.
		/// </summary>
		/// <param name="parameters">The parameters.</param>
		/// <returns></returns>
		private static ScriptFunctionParameter[] GetScriptFunctionParameters(IEnumerable<ParameterInfo> parameters)
		{
			var scriptParameters = new List<ScriptFunctionParameter>();

			foreach(ParameterInfo parameterInfo in parameters)
			{
				string clientSideName = GetParameterName(parameterInfo);
				string serverSideName = clientSideName;

				bool needsJSONEncoding = false;

				// if we have a [JSONBinder] mark on the parameter, 
				// we can serialize the parameter using the implementors internal toJSON function.
				var jsonBinderAtt = GetSingleAttribute<JSONBinderAttribute>(parameterInfo, false);
				if (jsonBinderAtt != null)
				{
					serverSideName = (string) GetPropertyValue(jsonBinderAtt, "EntryKey") ?? clientSideName;
					needsJSONEncoding = true;
				}

				scriptParameters.Add(new ScriptFunctionParameter(clientSideName, serverSideName, needsJSONEncoding));
			}

			return scriptParameters.ToArray();
		}

		/// <summary>
		/// Generates the javascript proxy function.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="functionName">Name of the function.</param>
		/// <param name="httpRequestMethod">The HTTP request method.</param>
		/// <param name="scriptFunctionParameters">The script function parameters.</param>
		/// <returns></returns>
		protected abstract string GenerateJavascriptFunction(string url, string functionName, string httpRequestMethod,
		                                                     ScriptFunctionParameter[] scriptFunctionParameters);

		/// <summary>
		/// Gets the URL extension.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private static string GetURLExtension(IEngineContext context)
		{
			return string.IsNullOrEmpty(context.UrlInfo.Extension)
			       	? string.Empty
			       	: "." + context.UrlInfo.Extension;
		}

		/// <summary>
		/// Gets the HTTP request method.
		/// </summary>
		/// <param name="ajaxActionMethod">The ajax action method.</param>
		/// <returns></returns>
		private static string GetHTTPRequestMethod(ICustomAttributeProvider ajaxActionMethod)
		{
			var accessibleThroughAtt = GetSingleAttribute<AccessibleThroughAttribute>(ajaxActionMethod, true);
			return accessibleThroughAtt != null ? accessibleThroughAtt.Verb.ToString().ToLower() : "get";
		}

		/// <summary>
		/// Gets the single attribute.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		private static T GetSingleAttribute<T>(ICustomAttributeProvider obj, bool inherit) where T : Attribute
		{
			Type attributeType = typeof(T);
			return (T) GetSingleAttribute(obj, attributeType, inherit);
		}

		/// <summary>
		/// Gets the single attribute.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="attributeType">Type of the attribute.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		private static object GetSingleAttribute(ICustomAttributeProvider obj, Type attributeType, bool inherit)
		{
			object[] attributes = obj.GetCustomAttributes(attributeType, inherit);
			return (attributes.Length > 0 ? attributes[0] : null);
		}

		/// <summary>
		/// Toes the camel case.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private static string ToCamelCase(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return value;
			}

			return Char.ToLower(value[0], CultureInfo.InvariantCulture)
			       + (value.Length > 0 ? value.Substring(1) : null);
		}

		/// <summary>
		/// Gets the base URL.
		/// </summary>
		/// <param name="area">The area.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private static string GetBaseUrl(string area, string controller, IEngineContext context)
		{
			string baseUrl = context.ApplicationPath + "/";
			if (!string.IsNullOrEmpty(area))
			{
				baseUrl += area + "/";
			}
			baseUrl += controller + "/";
			return baseUrl;
		}

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="propName">Name of the prop.</param>
		/// <returns></returns>
		private static object GetPropertyValue(object obj, string propName)
		{
			if (obj == null)
			{
				return null;
			}

			PropertyInfo propertyInfo = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
			return propertyInfo.GetValue(obj, null);
		}

		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		/// <param name="paramInfo">The parameterInfo.</param>
		/// <returns></returns>
		private static string GetParameterName(ParameterInfo paramInfo)
		{
			string paramName = null;

			// change the parameter name, if using [DataBind]
			var parameterAttribute = GetSingleAttribute<DataBindAttribute>(paramInfo, true);
			if (parameterAttribute != null)
			{
				paramName = parameterAttribute.Prefix;
			}

			if (ARFetchAttType != null)
			{
				// change the parameter name, if using [ARFetch]
				object parameterAttr = GetSingleAttribute(paramInfo, ARFetchAttType, true);
				if (parameterAttr != null)
				{
					paramName = Convert.ToString(GetPropertyValue(parameterAttr, "RequestParameterName"));
				}
			}

			// the parameter name change for [JsonAttribute] is made from within GetScriptFunctionParameters.

			// use the default parameter name, if none of the parameter binders define a new name
			if (string.IsNullOrEmpty(paramName))
			{
				paramName = paramInfo.Name;
			}

			return ToCamelCase(paramName);
		}

		#region Nested type: ScriptFunctionParameter

		/// <summary>
		/// Script parameters
		/// </summary>
		protected class ScriptFunctionParameter
		{
			private readonly string _clientSideParameterName;
			private readonly bool _needsJSONEncoding;
			private readonly string _serverSideParameterName;

			/// <summary>
			/// Initializes a new instance of the <see cref="ScriptFunctionParameter"/> class.
			/// </summary>
			/// <param name="clientSideName">Name of the client side.</param>
			/// <param name="serverSideName">Name of the server side.</param>
			/// <param name="needsJSONEncoding">if set to <c>true</c> [needs JSON encoding].</param>
			public ScriptFunctionParameter(string clientSideName, string serverSideName, bool needsJSONEncoding)
			{
				_clientSideParameterName = clientSideName;
				_serverSideParameterName = serverSideName;
				_needsJSONEncoding = needsJSONEncoding;
			}

			/// <summary>
			/// Gets the name of the server side parameter.
			/// </summary>
			/// <value>The name of the server side parameter.</value>
			public string ServerSideParameterName
			{
				get { return _serverSideParameterName; }
			}

			/// <summary>
			/// Gets the name of the client side parameter.
			/// </summary>
			/// <value>The name of the client side parameter.</value>
			public string ClientSideParameterName
			{
				get { return _clientSideParameterName; }
			}

			/// <summary>
			/// Gets a value indicating whether the parameter needs JSON encoding.
			/// </summary>
			/// <value><c>true</c> if [needs JSON encoding]; otherwise, <c>false</c>.</value>
			public bool NeedsJSONEncoding
			{
				get { return _needsJSONEncoding; }
			}
		}

		#endregion

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IMonoRailServices provider)
		{
			var loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(GetType());
			}

			controllerDescriptorBuilder = provider.ControllerDescriptorProvider;
			controllerTree = provider.ControllerTree;
		}

		#endregion
	}
}