// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Components.Binder;
	using Castle.Components.Validator;

	/// <summary>
	/// Specialization of <see cref="Controller"/> that tries
	/// to match the request params to method arguments.
	/// </summary>
	/// <remarks>
	/// You don't even need to always use databinding within
	/// arguments. <see cref="BindObject(ParamStore,Type,string)"/> 
	/// and <see cref="BindObjectInstance(object,string)"/>
	/// provides the same functionality to be used in place.
	/// </remarks>
	public abstract class SmartDispatcherController : Controller
	{
		private IDataBinder binder;
		private TreeBuilder treeBuilder = new TreeBuilder();
		private CompositeNode paramsNode, formNode, queryStringNode;
		private IDictionary<object, ErrorSummary> validationSummaryPerInstance = new Dictionary<object, ErrorSummary>();

		/// <summary>
		/// Represents the errors associated with an instance bound.
		/// </summary>
		protected IDictionary<object, ErrorList> boundInstances = new Dictionary<object, ErrorList>();

		/// <summary>
		/// Initializes a new instance of the <see cref="SmartDispatcherController"/> class.
		/// </summary>
		protected SmartDispatcherController() : this(new DataBinder())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SmartDispatcherController"/> class.
		/// </summary>
		/// <param name="binder">The binder.</param>
		protected SmartDispatcherController(IDataBinder binder)
		{
			this.binder = binder;
		}

		/// <summary>
		/// Gets the binder.
		/// </summary>
		/// <value>The binder.</value>
		public IDataBinder Binder
		{
			get { return binder; }
		}

		/// <summary>
		/// Gets or sets the bound instance errors.
		/// </summary>
		/// <value>The bound instance errors.</value>
		public IDictionary<object, ErrorList> BoundInstanceErrors
		{
			get { return boundInstances; }
			set { boundInstances = value; }
		}

		/// <summary>
		/// Gets the validation summary (key is the object instance)
		/// </summary>
		/// <value>The validation summary per instance.</value>
		public IDictionary<object, ErrorSummary> ValidationSummaryPerInstance
		{
			get { return validationSummaryPerInstance; }
		}

		/// <summary>
		/// Populates the validator error summary.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="binderUsedForBinding">The binder used for binding.</param>
		protected internal void PopulateValidatorErrorSummary(object instance, IDataBinder binderUsedForBinding)
		{
			ValidationSummaryPerInstance[instance] = binderUsedForBinding.GetValidationSummary(instance);
		}

		/// <summary>
		/// Gets the error summary associated with validation errors.
		/// <para>
		/// Will only work for instances populated by the <c>DataBinder</c>
		/// </para>
		/// </summary>
		/// <param name="instance">object instance</param>
		/// <returns>Error summary instance (can be null if the DataBinder wasn't configured to validate)</returns>
		protected ErrorSummary GetErrorSummary(object instance)
		{
			return validationSummaryPerInstance.ContainsKey(instance) ? validationSummaryPerInstance[instance] : null;
		}

		/// <summary>
		/// Returns <c>true</c> if the given instance had 
		/// validation errors during binding.
		/// <para>
		/// Will only work for instances populated by the <c>DataBinder</c>
		/// </para>
		/// </summary>
		/// <param name="instance">object instance</param>
		/// <returns><c>true</c> if the validation had an error</returns>
		protected bool HasValidationError(object instance)
		{
			ErrorSummary summary = GetErrorSummary(instance);

			if (summary == null) return false;

			return summary.ErrorsCount != 0;
		}

		/// <summary>
		/// Gets a list of errors that were thrown during the 
		/// object process, like conversion errors.
		/// </summary>
		/// <param name="instance">The instance that was populated by a binder.</param>
		/// <returns>List of errors</returns>
		protected ErrorList GetDataBindErrors(object instance)
		{
			return boundInstances[instance];
		}

		/// <summary>
		/// Constructs the parameters for the action and invokes it.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="request">The request.</param>
		/// <param name="actionArgs">The action args.</param>
		protected internal override void InvokeMethod(MethodInfo method, IRequest request, IDictionary actionArgs)
		{
			ParameterInfo[] parameters = method.GetParameters();

			object[] methodArgs = BuildMethodArguments(parameters, request, actionArgs);

			method.Invoke(this, methodArgs);
		}

		/// <summary>
		/// Uses a simple heuristic to select the best method -- especially in the 
		/// case of method overloads. 
		/// </summary>
		/// <param name="action">The action name</param>
		/// <param name="actions">The avaliable actions</param>
		/// <param name="request">The request instance</param>
		/// <param name="actionArgs">The custom arguments for the action</param>
		/// <returns></returns>
		protected internal override MethodInfo SelectMethod(String action, IDictionary actions,
		                                                    IRequest request, IDictionary actionArgs)
		{
			object methods = actions[action];

			// should check for single-option as soon as possible (performance improvement)
			if (methods is MethodInfo) return (MethodInfo) methods;

			ArrayList candidates = (ArrayList) methods;

			if (candidates == null) return null;

			return SelectBestCandidate((MethodInfo[]) candidates.ToArray(typeof(MethodInfo)),
			                           request.Params, actionArgs);
		}

		/// <summary>
		/// Selects the best method given the set of entries 
		/// avaliable on <paramref name="webParams"/> and <paramref name="actionArgs"/>
		/// </summary>
		/// <param name="candidates">The candidates.</param>
		/// <param name="webParams">The web params.</param>
		/// <param name="actionArgs">The custom action args.</param>
		/// <returns></returns>
		protected virtual MethodInfo SelectBestCandidate(MethodInfo[] candidates,
		                                                 NameValueCollection webParams,
		                                                 IDictionary actionArgs)
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
				int points = CalculatePoints(candidate, webParams, actionArgs);

				if (lastMaxPoints < points)
				{
					lastMaxPoints = points;
					bestCandidate = candidate;
				}
			}

			return bestCandidate;
		}

		/// <summary>
		/// Gets the name of the request parameter.
		/// </summary>
		/// <param name="param">The param.</param>
		/// <returns></returns>
		protected virtual String GetRequestParameterName(ParameterInfo param)
		{
			return param.Name;
		}

		/// <summary>
		/// Uses a simplest algorithm to compute points for a method 
		/// based on parameters available, which in turn reflects
		/// the best method is the one which the parameters will be 
		/// able to satistfy more arguments
		/// </summary>
		/// <param name="candidate">The method candidate</param>
		/// <param name="webParams">Parameter source</param>
		/// <param name="actionArgs">Extra parameters</param>
		/// <returns></returns>
		protected int CalculatePoints(MethodInfo candidate, NameValueCollection webParams, IDictionary actionArgs)
		{
			int points = 0;
			int matchCount = 0;

			ParameterInfo[] parameters = candidate.GetParameters();

			foreach(ParameterInfo param in parameters)
			{
				//
				// If the param is decorated with an attribute that implements IParameterBinder
				//
				
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

				requestParameterName = GetRequestParameterName(param);
				
				//
				// Otherwise
				//
				
				Type parameterType = param.ParameterType;
				
				if (binder.CanBindParameter(parameterType, requestParameterName, ParamsNode))
				{
					points += 10;
					matchCount++;
				}
				//
				// I'm not sure about the following. Seems to be
				// be fragile regarding the web parameters and the actionArgs array
				//
				else if ((actionArgs != null) && actionArgs.Contains(requestParameterName))
				{
					object actionArg = actionArgs[requestParameterName];
					
					bool exactMatch;
					
					if (binder.Converter.CanConvert(parameterType, actionArg.GetType(), actionArg, out exactMatch))
					{
						points += 10;

						// Give extra weight to exact matches.
						if (exactMatch) points += 5;
						
						matchCount++;
					}
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
		/// <param name="actionArgs">Extra arguments to pass to the action.</param>
		/// <returns>An array with the arguments values</returns>
		protected virtual object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request, IDictionary actionArgs)
		{
			object[] args = new object[parameters.Length];
			String paramName = String.Empty;
			String value = String.Empty;
			
			try
			{
				for(int argIndex = 0; argIndex < args.Length; argIndex++)
				{
					//
					// If the parameter is decorated with an attribute
					// that implements IParameterBinder, it's up to it
					// to convert itself
					//
					
					ParameterInfo param = parameters[argIndex];
					paramName = GetRequestParameterName(param);

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
					
					//
					// Otherwise we handle it
					//

					if (!handled)
					{
						object convertedVal;
						bool conversionSucceeded;
						
						convertedVal = binder.BindParameter(param.ParameterType, paramName, ParamsNode);
						
						if (convertedVal == null && (actionArgs != null) && actionArgs.Contains(paramName))
						{
							object actionArg = actionArgs[paramName];
							
							Type actionArgType = (actionArg != null) ? actionArg.GetType() : param.ParameterType;
							
							convertedVal = binder.Converter.Convert(param.ParameterType, actionArgType, actionArg, out conversionSucceeded);
						}
						
						args[argIndex] = convertedVal;
					}
				}
			}
			catch(FormatException ex)
			{
				throw new RailsException(
					String.Format("Could not convert {0} to request type. " +
						"Argument value is '{1}'", paramName, Params.Get(paramName)), ex);
			}
			catch(Exception ex)
			{
				throw new RailsException(
					String.Format("Error building method arguments. " +
						"Last param analyzed was {0} with value '{1}'", paramName, value), ex);
			}

			return args;
		}

		/// <summary>
		/// Binds the object of the specified type using the given prefix.
		/// </summary>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns></returns>
		protected object BindObject(Type targetType, String prefix)
		{
			return BindObject(ParamStore.Params, targetType, prefix);
		}

		/// <summary>
		/// Binds the object of the specified type using the given prefix.
		/// but only using the entries from the collection specified on the <paramref name="from"/>
		/// </summary>
		/// <param name="from">Restricts the data source of entries.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns></returns>
		protected object BindObject(ParamStore from, Type targetType, String prefix)
		{
			return BindObject(from, targetType, prefix, null, null);
		}

		/// <summary>
		/// Binds the object of the specified type using the given prefix.
		/// but only using the entries from the collection specified on the <paramref name="from"/>
		/// </summary>
		/// <param name="from">From.</param>
		/// <param name="targetType">Type of the target.</param>
		/// <param name="prefix">The prefix.</param>
		/// <param name="excludedProperties">The excluded properties, comma separated list.</param>
		/// <param name="allowedProperties">The allowed properties, comma separated list.</param>
		/// <returns></returns>
		protected object BindObject(ParamStore from, Type targetType, String prefix, String excludedProperties, String allowedProperties)
		{
			CompositeNode node = ObtainParamsNode(from);

			object instance = binder.BindObject(targetType, prefix, excludedProperties, allowedProperties, node);

			boundInstances[instance] = binder.ErrorList;
			PopulateValidatorErrorSummary(instance, binder);

			return instance;
		}

		/// <summary>
		/// Binds the object instance using the specified prefix.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="prefix">The prefix.</param>
		protected void BindObjectInstance(object instance, String prefix)
		{
			BindObjectInstance(instance, ParamStore.Params, prefix);
		}

		/// <summary>
		/// Binds the object instance using the given prefix.
		/// but only using the entries from the collection specified on the <paramref name="from"/>
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="from">From.</param>
		/// <param name="prefix">The prefix.</param>
		protected void BindObjectInstance(object instance, ParamStore from, String prefix)
		{
			CompositeNode node = ObtainParamsNode(from);

			binder.BindObjectInstance(instance, prefix, node);

			boundInstances[instance] = binder.ErrorList;
			PopulateValidatorErrorSummary(instance, binder);
		}

		/// <summary>
		/// Binds the object of the specified type using the given prefix.
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="prefix">The prefix.</param>
		/// <returns></returns>
		protected T BindObject<T>(String prefix)
		{
			return (T) BindObject(typeof(T), prefix);
		}

		/// <summary>
		/// Binds the object of the specified type using the given prefix.
		/// but only using the entries from the collection specified on the <paramref name="from"/>
		/// </summary>
		/// <typeparam name="T">Target type</typeparam>
		/// <param name="from">From.</param>
		/// <param name="prefix">The prefix.</param>
		/// <returns></returns>
		protected T BindObject<T>(ParamStore from, String prefix)
		{
			return (T) BindObject(from, typeof(T), prefix);
		}

		/// <summary>
		/// Binds the object of the specified type using the given prefix.
		/// but only using the entries from the collection specified on the <paramref name="from"/>
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="from">From.</param>
		/// <param name="prefix">The prefix.</param>
		/// <param name="excludedProperties">The excluded properties.</param>
		/// <param name="allowedProperties">The allowed properties.</param>
		/// <returns></returns>
		protected T BindObject<T>(ParamStore from, String prefix, String excludedProperties, String allowedProperties)
		{
			return (T) BindObject(from, typeof(T), prefix, excludedProperties, allowedProperties);
		}
		
		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="Controller.Params"/>
		/// </summary>
		protected virtual internal CompositeNode ParamsNode
		{
			get
			{
				if (paramsNode == null)
				{
					paramsNode = treeBuilder.BuildSourceNode(Params);
					treeBuilder.PopulateTree(paramsNode, HttpContext.Request.Files);
				}
				
				return paramsNode;
			}
		}

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.Form"/>
		/// </summary>
		protected virtual internal CompositeNode FormNode
		{
			get
			{
				if (formNode == null)
				{
					formNode = treeBuilder.BuildSourceNode(Request.Form);
					treeBuilder.PopulateTree(formNode, HttpContext.Request.Files);
				}
				
				return formNode;
			}
		}

		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="IRequest.QueryString"/>
		/// </summary>
		protected virtual internal CompositeNode QueryStringNode
		{
			get
			{
				if (queryStringNode == null)
				{
					queryStringNode = treeBuilder.BuildSourceNode(Request.QueryString);
					treeBuilder.PopulateTree(queryStringNode, HttpContext.Request.Files);
				}
				
				return queryStringNode;
			}
		}

		/// <summary>
		/// This method is for internal use only
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public CompositeNode ObtainParamsNode(ParamStore from)
		{
			switch(from)
			{
				case ParamStore.Form:
					return FormNode;
				case ParamStore.QueryString:
					return QueryStringNode;
				default:
					return ParamsNode;
			}
		}
	}
}
