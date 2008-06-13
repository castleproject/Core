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
	using System.Collections;
	using System.Globalization;
	using System.Reflection;
	using System.Text;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Configuration;
	using Descriptors;

	/// <summary>
	/// Provides a service which generates a <em>JavaScript</em> block, that
	/// can be used to call Ajax actions on the controller. This JavaScript will
	/// use the <em>Prototype</em> syntax.
	/// </summary>
	public class PrototypeAjaxProxyGenerator : IAjaxProxyGenerator, IMRServiceEnabled
	{
		private static readonly Hashtable ajaxProxyCache = Hashtable.Synchronized(new Hashtable());

		private static readonly Type 
			ARFetchAttType =
				TypeLoadUtil.GetType(
					TypeLoadUtil.GetEffectiveTypeName(
						"Castle.MonoRail.ActiveRecordSupport.ARFetchAttribute, Castle.MonoRail.ActiveRecordSupport"), true);

		private ILogger logger = NullLogger.Instance;
		private IControllerDescriptorProvider controllerDescriptorBuilder;
		private IControllerTree controllerTree;

		/// <summary>
		/// Initializes a new instance of the <see cref="PrototypeAjaxProxyGenerator"/> class.
		/// </summary>
		public PrototypeAjaxProxyGenerator()
		{
		}

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

		#region IServiceEnabledComponent implementation

		/// <summary>
		/// Invoked by the framework in order to give a chance to
		/// obtain other services
		/// </summary>
		/// <param name="provider">The service proviver</param>
		public void Service(IMonoRailServices provider)
		{
			ILoggerFactory loggerFactory = (ILoggerFactory) provider.GetService(typeof(ILoggerFactory));

			if (loggerFactory != null)
			{
				logger = loggerFactory.Create(typeof(PrototypeAjaxProxyGenerator));
			}

			controllerDescriptorBuilder = provider.ControllerDescriptorProvider;
			controllerTree = provider.ControllerTree;
		}

		#endregion

		/// <summary>
		/// Generates an AJAX JavaScript proxy for a given controller.
		/// </summary>
		/// <param name="context">The context of the current request</param>
		/// <param name="proxyName">Name of the javascript proxy object</param>
		/// <param name="controller">Controller which will be target of the proxy</param>
		/// <param name="area">area which the controller belongs to</param>
		public string GenerateJSProxy(IEngineContext context, string proxyName, string area, string controller)
		{
			string nl = Environment.NewLine;
			string cacheKey = (area + "|" + controller).ToLower(CultureInfo.InvariantCulture);
			string result = (String) ajaxProxyCache[cacheKey];

			if (result == null)
			{
				logger.Debug("Ajax Proxy for area: '{0}', controller: '{1}' was not found. Generating a new one", area, controller);
				Type controllerType = controllerTree.GetController(area, controller);

				if (controllerType == null)
				{
					logger.Fatal("Controller not found for the area and controller specified");
					throw new MonoRailException("Controller not found with Area: '{0}', Name: '{1}'", area, controller);
				}

				string baseUrl = context.ApplicationPath + "/";

				if (area != null && area != String.Empty)
				{
					baseUrl += area + "/";
				}

				baseUrl += controller + "/";

				// TODO: develop a smarter function generation, inspecting the return
				// value of the action and generating a proxy that does the same.
				// also, consider a proxy pattern for the Ajax.Updater.

				StringBuilder functions = new StringBuilder(1024);

				functions.Append("{");

				ControllerMetaDescriptor metaDescriptor = controllerDescriptorBuilder.BuildDescriptor(controllerType);

				bool commaNeeded = false;

				foreach(MethodInfo ajaxActionMethod in metaDescriptor.AjaxActions)
				{
					if (!commaNeeded)
					{
						commaNeeded = true;
					}
					else
					{
						functions.Append(',');
					}

					string methodName = ajaxActionMethod.Name;

					AjaxActionAttribute ajaxActionAtt = GetSingleAttribute<AjaxActionAttribute>(ajaxActionMethod, true);
					AccessibleThroughAttribute accessibleThroughAtt = GetSingleAttribute<AccessibleThroughAttribute>(ajaxActionMethod, true);

					string extension = string.IsNullOrEmpty(context.UrlInfo.Extension) ? string.Empty : "." + context.UrlInfo.Extension;
					string url = baseUrl + methodName + extension;

					string functionName = ToCamelCase(ajaxActionAtt.Name ?? methodName);

					functions.AppendFormat(nl + "\t{0}: function(", functionName);

					StringBuilder parameters = new StringBuilder("_=");

					foreach(ParameterInfo pi in ajaxActionMethod.GetParameters())
					{
						string paramName = GetParameterName(pi);

						// by default, just forward the parameter taken by the function as the request parameter value
						string paramValue = paramName;

						// if we have a [JSONBinder] mark on the parameter, we can serialize the parameter using prototype's Object.toJSON().
						JSONBinderAttribute jsonBinderAtt = GetSingleAttribute<JSONBinderAttribute>(pi, false);
						if (jsonBinderAtt != null)
						{
							// toJSON requires Prototype 1.5.1. Users of [JSONBinder] should be aware of that.
							paramName = (string) GetPropertyValue(jsonBinderAtt, "EntryKey");
							paramValue = "Object.toJSON(" + paramValue + ")";
						}

						functions.AppendFormat("{0}, ", paramName);

						// appends " &<paramName>=' + <paramValue> + ' " to the string.
						// the paramValue will run on the client-side, so it can be a parameter name, or a function call like Object.toJSON().
						// parameters.Append("\\x26").Append(paramName).Append("='+").Append(paramValue).Append("+'");
						parameters.Append("&").Append(paramName).Append("='+").Append(paramValue).Append("+'");
					}

					string httpRequestMethod = "get";
					if (accessibleThroughAtt != null)
					{
						httpRequestMethod = accessibleThroughAtt.Verb.ToString().ToLower();
					}

					functions.Append("callback)").Append(nl).Append("\t{").Append(nl);
					functions.AppendFormat("\t\tvar r=new Ajax.Request('{0}', " +
					                       "{{method: '{1}', asynchronous: !!callback, onComplete: callback, parameters: '{2}'}}); " + nl +
					                       "\t\tif(!callback) return r.transport.responseText;" + nl,
					                       url, httpRequestMethod, parameters);
					functions.Append("\t}").Append(nl);
				}

				functions.Append("};").Append(nl);

				ajaxProxyCache[cacheKey] = result = functions.ToString();
			}

			return @"<script type=""text/javascript"">" + nl +
			       "var " + proxyName + " =" + nl + result + "</script>";
		}

		/// <summary>
		/// Gets the name of the parameter.
		/// </summary>
		/// <param name="paramInfo">The parameterInfo.</param>
		/// <returns></returns>
		private string GetParameterName(ParameterInfo paramInfo)
		{
			string paramName = null;

			// change the parameter name, if using [DataBind]
			DataBindAttribute parameterAttribute = GetSingleAttribute<DataBindAttribute>(paramInfo, true);
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

			// the parameter name change for [JsonAttribute] is made from within GenerateJSProxy.

			// use the default parameter name, if none of the parameter binders define a new name
			if (paramName == null || paramName.Length == 0)
			{
				paramName = paramInfo.Name;
			}

			return ToCamelCase(paramName);
		}

		#region Utility Methods

		/// <summary>
		/// Gets the property value.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="propName">Name of the prop.</param>
		/// <returns></returns>
		private static object GetPropertyValue(object obj, string propName)
		{
			if (obj == null)
				return null;

			PropertyInfo propertyInfo = obj.GetType().GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
			return propertyInfo.GetValue(obj, null);
		}

		/// <summary>
		/// Gets the single attribute.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		private T GetSingleAttribute<T>(ICustomAttributeProvider obj, bool inherit) where T : Attribute
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
		private object GetSingleAttribute(ICustomAttributeProvider obj, Type attributeType, bool inherit)
		{
			object[] attributes = obj.GetCustomAttributes(attributeType, inherit);
			return (attributes.Length > 0 ? attributes[0] : null);
		}

		/// <summary>
		/// Toes the camel case.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private string ToCamelCase(string value)
		{
			if (value == null || value.Length == 0) return value;

			return Char.ToLower(value[0], CultureInfo.InvariantCulture)
			       + (value.Length > 0 ? value.Substring(1) : null);
		}

		#endregion
	}
}
