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
	/// arguments. <see cref="BindObject(Type targetType, String prefix)"/> 
	/// and <see cref="BindObjectInstance(object instance, String prefix)"/>
	/// provides the same functionality to be used in place.
	/// </remarks>
	public abstract class SmartDispatcherController : Controller
	{
		protected IDictionary boundInstances = new HybridDictionary();

		private DataBinder binder;
		private TreeBuilder treeBuilder = new TreeBuilder();
	
		private CompositeNode paramsNode, formNode, queryStringNode;

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

		public IDictionary BoundInstanceErrors
		{
			get { return boundInstances; }
			set { boundInstances = value; }
		}

		protected override void Initialize()
		{
		}

		protected override void InvokeMethod(MethodInfo method, IRequest request, params object[] actionArgs)
		{
			ParameterInfo[] parameters = method.GetParameters();

			object[] methodArgs = BuildMethodArguments(parameters, request, actionArgs);

			method.Invoke(this, methodArgs);
		}

		protected override MethodInfo SelectMethod(String action, IDictionary actions, IRequest request, params object[] actionArgs)
		{
			object methods = actions[action];

			// should check for single-option as soon as possible (performance improvement)
			if (methods is MethodInfo) return (MethodInfo) methods;

			ArrayList candidates = (ArrayList) methods;

			if (candidates == null) return null;

			return SelectBestCandidate((MethodInfo[]) candidates.ToArray(typeof(MethodInfo)),
			                           request.Params, actionArgs);
		}

		protected virtual MethodInfo SelectBestCandidate(MethodInfo[] candidates, NameValueCollection webParams, params object[] actionArgs)
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
		/// <param name="actionArgs">Extra parameters</param>
		/// <returns></returns>
		protected int CalculatePoints(MethodInfo candidate, NameValueCollection webParams, params object[] actionArgs)
		{
			int points = 0;
			int matchCount = 0;
			int actionArgsIndex = 0;

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

				if (requestParameterName == null)
				{
					requestParameterName = GetRequestParameterName(param);
				}
				
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
				else if ((actionArgs != null) && (actionArgsIndex < actionArgs.Length))
				{
					object actionArg = actionArgs[actionArgsIndex];
					
					if (actionArg == null) continue;

					bool exactMatch;
					
					if (binder.Converter.CanConvert(parameterType, actionArg.GetType(), actionArg, out exactMatch))
					{
						points += 10;

						// Give extra weight to exact matches.
						if (exactMatch) points += 5;
						
						matchCount++;
						actionArgsIndex++;	
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
		protected virtual object[] BuildMethodArguments(ParameterInfo[] parameters, IRequest request, params object[] actionArgs)
		{
			object[] args = new object[parameters.Length];
			String paramName = String.Empty;
			String value = String.Empty;

			int actionArgsIndex = 0;
			
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
					
					//
					// Otherwise we handle it
					//

					if (!handled)
					{
						object convertedVal;
						bool conversionSucceeded;
						
						convertedVal = binder.BindParameter(param.ParameterType, paramName, ParamsNode);
						
						if (convertedVal == null && (actionArgs != null) && (actionArgsIndex < actionArgs.Length))
						{
							object actionArg = actionArgs[actionArgsIndex];
							
							if (actionArg == null) continue;
							
							convertedVal = binder.Converter.Convert(param.ParameterType, actionArg.GetType(), actionArg, out conversionSucceeded);

							if (conversionSucceeded) actionArgsIndex++;						
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

		protected object BindObject(Type targetType, String prefix)
		{
			return BindObject(ParamStore.Params, targetType, prefix);
		}
		
		protected object BindObject(ParamStore from, Type targetType, String prefix)
		{
			return BindObject(from, targetType, prefix, null, null);
		}

		protected object BindObject(ParamStore from, Type targetType, String prefix, String excludedProperties, String allowedProperties)
		{
			CompositeNode node = ObtainParamsNode(from);

			object instance = binder.BindObject(targetType, prefix, excludedProperties, allowedProperties, node);

			boundInstances[instance] = binder.ErrorList;

			return instance;
		}
		
		protected void BindObjectInstance(object instance, String prefix)
		{
			BindObjectInstance(instance, ParamStore.Params, prefix);
		}

		protected void BindObjectInstance(object instance, ParamStore from, String prefix)
		{
			CompositeNode node = ObtainParamsNode(from);

			binder.BindObjectInstance(instance, prefix, node);

			boundInstances[instance] = binder.ErrorList;
		}

		protected ErrorList GetDataBindErrors(object instance)
		{
			return boundInstances[instance] as ErrorList;
		}
		
		/// <summary>
		/// Lazy initialized property with a hierarchical 
		/// representation of the flat data on <see cref="Controller.Params"/>
		/// </summary>
		protected internal CompositeNode ParamsNode
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
		protected internal CompositeNode FormNode
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
		protected internal CompositeNode QueryStringNode
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
