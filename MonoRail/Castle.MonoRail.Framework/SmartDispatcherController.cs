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

		private IBindingDataSourceNode queryStringNode;
		private IBindingDataSourceNode formNode;
		private IBindingDataSourceNode paramsNode;

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

		public IBindingDataSourceNode QueryStringNode
		{
			get
			{
				if (queryStringNode == null)
				{
					queryStringNode = new NameValueCollectionAdapter(Request.QueryString);
				}
				return queryStringNode;
			}
		}

		public IBindingDataSourceNode FormNode
		{
			get
			{
				if (formNode == null)
				{
					formNode = new NameValueCollectionAdapter(Request.Form);
				}
				return formNode;
			}
		}

		public IBindingDataSourceNode ParamsNode
		{
			get
			{
				if (paramsNode == null)
				{
					paramsNode = new NameValueCollectionAdapter(Request.Params);
				}
				return paramsNode;
			}
		}

		protected override void Initialize()
		{
		}

		protected override void InvokeMethod(MethodInfo method, IRequest request)
		{
			ParameterInfo[] parameters = method.GetParameters();

			object[] methodArgs = BuildMethodArguments(parameters, request);

			method.Invoke(this, methodArgs);
		}

		protected override MethodInfo SelectMethod(String action, IDictionary actions, IRequest request)
		{
			object methods = actions[action];

			// should check for single-option as soon as possible (performance improvement)
			if (methods is MethodInfo) return (MethodInfo) methods;

			ArrayList candidates = (ArrayList) methods;

			if (candidates == null) return null;

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
			int matchCount = 0;

			ParameterInfo[] parameters = candidate.GetParameters();

			foreach(ParameterInfo param in parameters)
			{
				object[] attributes = param.GetCustomAttributes(false);

				String requestParameterName = null;

				bool calculated = false;

				foreach(object attr in attributes)
				{
					IParameterBinder actionParam = attr as IParameterBinder;
					
					if (actionParam == null) continue;

					points += actionParam.CalculateParamPoints(this, param);
					calculated = true;
				}

				if (calculated) continue;

				if (requestParameterName == null)
					requestParameterName = GetRequestParameterName(param);

				object value = webParams.Get(requestParameterName);

				if (value != null)
				{
					points += 10;
					matchCount++;
				}
			}

			// the bonus should be nice only for disambiguation.
			// otherwise, unmatched-parameterless-actions will always have
			// the same weight as matched-single-parameter-actions.
			if (matchCount == parameters.Length)
			{
				points += 5;
			}

			return points;
		}

		protected virtual String GetRequestParameterName(ParameterInfo param)
		{
			return param.Name;
		}

		/// <summary>
		/// Returns an array that hopefully fills the arguments of the selected action.
		/// </summary>
		/// <remarks>
		/// Each parameter is inspected and we try to obtain an implementation of
		/// <see cref="IParameterBinder"/> from the attributes the parameter have (if any).
		/// If an implementation is found, it's used to fill the value for that parameter.
		/// Otherwise we use simple conversion to obtain the value.
		/// </remarks>
		/// <param name="parameters">Parameters to obtain the values to</param>
		/// <param name="request">The current request, which is the source to obtain the data</param>
		/// <returns>An array with the arguments values</returns>
		protected virtual object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request)
		{
			object[] args = new object[parameters.Length];
			String paramName = String.Empty;
			String value = String.Empty;

			CreateParamCollections(request);

			IDictionary files = request.Files;

			try
			{
				for(int argIndex = 0; argIndex < args.Length; argIndex++)
				{
					ParameterInfo param = parameters[argIndex];
					paramName = param.Name;

					bool handled = false;

					object[] attributes = param.GetCustomAttributes(false);

					foreach(object attr in attributes)
					{
						IParameterBinder paramBinder = attr as IParameterBinder;

						if (paramBinder != null)
						{
							args[argIndex] = paramBinder.Bind(this, param);

							handled = true;
							break;
						}
					}

					if (!handled)
					{
						bool conversionSucceeded;

						object convertedVal = ConvertUtils.Convert(
							param.ParameterType, paramName, allParams, files, out conversionSucceeded);

						if (conversionSucceeded)
						{
							args[argIndex] = convertedVal;
						}
						else
						{
							// Should we log, cry out loud, throw exception or what?
						}
					}
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
						"Last param analyzed was {0} with value '{1}'", paramName, value), ex);
			}

			return args;
		}

		protected object BindObject(ParamStore from, Type targetType, String prefix)
		{
			return BindObject(from, targetType, prefix, null, null);
		}

		protected object BindObject(ParamStore from, Type targetType, String prefix, String excludedProperties, String allowedProperties)
		{
			NameValueCollection webParams = ResolveParamsSource(from);

			binder.Files = Context.Request.Files;

			object instance = binder.BindObject(targetType, prefix, excludedProperties, allowedProperties, new NameValueCollectionAdapter(webParams));

			boundInstances[instance] = binder.ErrorList;

			return instance;
		}

		protected void BindObjectInstance(object instance, ParamStore from, String prefix)
		{
			NameValueCollection webParams = ResolveParamsSource(from);

			binder.Files = Context.Request.Files;

			binder.BindObjectInstance(instance, prefix, new NameValueCollectionAdapter(webParams));

			boundInstances[instance] = binder.ErrorList;
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