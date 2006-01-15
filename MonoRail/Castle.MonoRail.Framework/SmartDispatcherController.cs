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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;
	
	using Castle.Components.Binder;

	/// <summary>
	/// Specialization of <see cref="Controller"/> that tries
	/// to match the request params to method arguments.
	/// </summary>
	/// <remarks>
	/// You don't even need to always use databinding within
	/// arguments. <see cref="BindObject"/> and <see cref="BindObjectInstance"/>
	/// provides the same functionality to be used in place.
	/// </remarks>
	public abstract class SmartDispatcherController : Controller
	{
		protected IDictionary boundInstances = new HybridDictionary();

		private NameValueCollection queryParams;
		private NameValueCollection formParams;
		private NameValueCollection allParams;

		private DataBinder binder;

		public SmartDispatcherController() : this(new DataBinder())
		{
		}

		protected SmartDispatcherController(DataBinder binder)
		{
			this.binder = binder;
		}

		public DataBinder Binder
		{
			get { return binder; }
		}

		protected override void Initialize()
		{
		}

		protected override void InvokeMethod(MethodInfo method, IRequest request)
		{
			// HACK: GetBaseDefinition() is a workaround for DYNPROXY-14
			// see: http://support.castleproject.org/jira/browse/DYNPROXY-14
			ParameterInfo[] parameters = method.GetBaseDefinition().GetParameters();

			object[] methodArgs = BuildMethodArguments(parameters, request);

			method.Invoke(this, methodArgs);
		}

		protected override MethodInfo SelectMethod(String action, IDictionary actions, IRequest request)
		{
			object methods = actions[action];

			// should check for single-option as soon as possible (performance improvement)
			if (methods is MethodInfo) return (MethodInfo) methods;

			ArrayList candidates = (ArrayList) methods;

			if (candidates == null)
				return null;

			return SelectBestCandidate(
				(MethodInfo[]) candidates.ToArray(typeof(MethodInfo)),
				request.Params);
		}

		protected virtual MethodInfo SelectBestCandidate(MethodInfo[] candidates, NameValueCollection webParams)
		{
			if (candidates.Length == 1)
			{
				// There's nothing much to do in this situation
				return candidates[0];
			}

			int lastMaxPoints = int.MinValue;
			MethodInfo bestCandidate = null;

			foreach(MethodInfo candidate in candidates)
			{
				int points = CalculatePoints(candidate, webParams);

				if (lastMaxPoints < points)
				{
					lastMaxPoints = points;
					bestCandidate = candidate;
				}
			}

			return bestCandidate;
		}

		protected int CalculatePoints(MethodInfo candidate, NameValueCollection webParams)
		{
			int points = 0;
			int paramsMatched = 0;

			ParameterInfo[] parameters = candidate.GetParameters();

			foreach(ParameterInfo param in parameters)
			{
				object value = webParams.Get(GetRequestParameterName(param));

				if (value != null)
				{
					points += 10;
					paramsMatched++;
				}
			}

			if (paramsMatched == parameters.Length)
			{
				points += 10;
			}

			return points;
		}

		protected virtual String GetRequestParameterName(ParameterInfo param)
		{
			return param.Name;
		}

		protected virtual object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request)
		{
			object[] args = new object[parameters.Length];
			String paramName = String.Empty;
			String value = String.Empty;
			Type paramType;

			CreateParamCollections(request);

			IDictionary files = request.Files;

			try
			{
				for(int argIndex = 0; argIndex < args.Length; argIndex++)
				{
					ParameterInfo param = parameters[argIndex];
					paramName = param.Name;
					paramType = param.ParameterType;

					if (paramType.IsPrimitive || paramType == typeof(DateTime) || paramType == typeof(String))
					{
						args[argIndex] = ConvertUtils.Convert(param.ParameterType, paramName, allParams, files);
					}
					else
					{
						bool handled = false;

						object[] attributes = param.GetCustomAttributes(false);
						
						foreach(object attr in attributes)
						{
							IParameterBinder paramBinder = attr as IParameterBinder;

							if (paramBinder != null)
							{
								args[argIndex] = paramBinder.Bind(this, param);

								handled = true; break;
							}
						}

						if (!handled)
						{
							args[argIndex] = ConvertUtils.Convert(param.ParameterType, paramName, allParams, files);
						}
					}

					// if complex binding is successful, there's no need for further processing
//					if (BindComplexParameter(param, request, args, argIndex))
//					{
//						continue;
//					}
				}
			}
			catch(FormatException ex)
			{
				throw new RailsException(
					String.Format("Could not convert {0} to request type. " +
						"Argument value is '{1}'", paramName, allParams.Get(paramName)), ex);
			}
			catch(Exception ex)
			{
				throw new RailsException(
					String.Format("Error building method arguments. " +
						"Last param analized was {0} with value '{1}'", paramName, value), ex);
			}

			return args;
		}

		/// <summary>
		/// Complex parameter bindings can be done overriding this method.
		/// It should return <c>true</c> if the binding was completed. If it returns
		/// <c>false</c>, MonoRail will try to convert the parameter in the usual way.
		/// </summary>
		/// <returns>
		/// <c>true</c> if binding completes and the default behaviour can be skipped,
		/// <c>false</c> otherwise.
		/// </returns>
//		protected virtual bool BindComplexParameter(ParameterInfo param, IRequest request, object[] args, int argIndex)
//		{
//			object[] bindAttributes = param.GetCustomAttributes(typeof(DataBindAttribute), false);
//
//			if (bindAttributes.Length != 0)
//			{
//				DataBindAttribute dba = bindAttributes[0] as DataBindAttribute;
//
//				args[argIndex] = BindObject(dba.From, param.ParameterType, dba.Prefix, dba.Exclude, dba.Allow);
//
//				return true;
//			}
//
//			return false;
//		}

		protected object BindObject(ParamStore from, Type targetType, String prefix)
		{
			return BindObject(from, targetType, prefix);
		}

		protected object BindObject(ParamStore from, Type targetType, String prefix, String excludedProperties, String allowedProperties)
		{
			NameValueCollection webParams = ResolveParamsSource(from);

			binder.Prefix = prefix;
			binder.Files = Context.Request.Files;
			binder.ExcludedProperties = excludedProperties;
			binder.AllowedProperties = allowedProperties;

			object instance = binder.BindObject(targetType, new NameValueCollectionAdapter(webParams));

			boundInstances[instance] = binder.Errors;

			return instance;
		}

		protected void BindObjectInstance(object instance, ParamStore from, String prefix)
		{
			NameValueCollection webParams = ResolveParamsSource(from);

			binder.Prefix = prefix;
			binder.Files = Context.Request.Files;
			binder.ExcludedProperties = null;
			binder.AllowedProperties = null;

			binder.BindObjectInstance(instance, new NameValueCollectionAdapter(webParams));

			boundInstances[instance] = binder.Errors;
		}

		protected ErrorList GetDataBindErrors(object instance)
		{
			ArrayList list = boundInstances[instance] as ArrayList;

			return new ErrorList(list);
		}

		protected void CreateParamCollections(IRequest request)
		{
			formParams = request.Form;
			queryParams = request.QueryString;
			allParams = request.Params;
		}

		protected internal NameValueCollection ResolveParamsSource(ParamStore from)
		{
			NameValueCollection webParams = null;
	
			switch(from)
			{
				case ParamStore.Form:
					webParams = formParams;
					break;

				case ParamStore.QueryString:
					webParams = queryParams;
					break;

				default:
					webParams = allParams;
					break;
			}

			return webParams;
		}
	}
}