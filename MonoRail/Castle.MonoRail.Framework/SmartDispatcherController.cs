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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.MonoRail.Framework.Internal;

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
		private IDictionary boundInstances = new ListDictionary();

		private NameValueCollection queryParams;
		private NameValueCollection formParams;
		private NameValueCollection allParams;

		protected DataBinder binder;

		public SmartDispatcherController()
		{
		}

		public DataBinder Binder
		{
			get { return binder; }
		}

		protected override void Initialize()
		{
			binder = new DataBinder();
		}

		protected internal override void CollectActions()
		{
			MethodInfo[] methods = GetType().GetMethods( BindingFlags.Public|BindingFlags.Instance );
			
			// HACK: workaround for DYNPROXY-14
			// see: http://support.castleproject.org/jira/browse/DYNPROXY-14
			for (int i=0; i < methods.Length; i++)
				methods[i] = methods[i].GetBaseDefinition();
			
			foreach(MethodInfo m in methods)
			{
				if (_actions.Contains(m.Name))
				{
					ArrayList list = _actions[m.Name] as ArrayList;

					if (list == null)
					{
						list = new ArrayList();
						list.Add(_actions[m.Name]);

						_actions[m.Name] = list;
					}

					list.Add(m);
				}
				else
				{
					_actions[m.Name] = m;
				}
			}

			base.ScreenCommonPublicMethods(_actions);
		}

		protected override void InternalSend(String action)
		{
			base.InternalSend( action );
		}

		protected override void InvokeMethod(MethodInfo method, IRequest request)
		{
			ParameterInfo[] parameters = method.GetParameters();

			object[] methodArgs = BuildMethodArguments( parameters, request);
            method.Invoke(this, methodArgs);
		}

		protected override MethodInfo SelectMethod(String action, IDictionary actions, IRequest request)
		{
			object methods = actions[action];

			// should check for single-option as soon as possible (performance improvement)
			if (methods is MethodInfo)
				return (MethodInfo) methods;
			
			ArrayList candidates = (ArrayList) methods;
			
			if (candidates == null)
				return null;

			return SelectBestCandidate( 
				(MethodInfo[]) candidates.ToArray( typeof(MethodInfo) ), 
				request.Params );
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
				int points = CalculatePoints( candidate, webParams );
				
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
				object value = webParams.Get( param.Name );

				if (value != null )
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

		protected virtual object[] BuildMethodArguments( ParameterInfo[] parameters, IRequest request )
		{
			object[] args = new object[parameters.Length];
			String paramName = String.Empty;
			String value = String.Empty;

			CreateParamCollections(request);

			IDictionary files = request.Files;

			try
			{
				for(int i=0; i < args.Length; i++)
				{
					ParameterInfo param	= parameters[i];
					paramName = param.Name;

					// if complex binding is successful, there's no need for further processing
					if (BindComplexParameter(param, request, args, i))
						continue;
					
					args[i] = ConvertUtils.Convert( param.ParameterType, allParams.GetValues( paramName ), param.Name, files, allParams );
				}
			}
			catch(FormatException ex)
			{
				throw new RailsException( 
					String.Format("Could not convert {0} to request type. " + 
						"Argument value is '{1}'", paramName, allParams.Get( paramName ) ), ex );
			}
			catch(Exception ex)
			{
				throw new RailsException( 
					String.Format("Error building method arguments. " + 
						"Last param analized was {0} with value '{1}'", paramName, value), ex );
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
		protected virtual bool BindComplexParameter(ParameterInfo param, IRequest request, object[] args, int i)
		{
			object[] bindAttributes	= param.GetCustomAttributes( typeof(DataBindAttribute), false );
			
			if ( bindAttributes.Length > 0 )
			{
				DataBindAttribute dba = bindAttributes[0] as DataBindAttribute;

				args[i] = BindObject( dba.From, param.ParameterType, dba.Prefix, dba.NestedLevel, dba.Exclude, dba.Allow );
				
				return true;
			}

			return false;
		}

		protected object BindObject(ParamStore from, Type paramType, String prefix, int nestedLevel, String excludedProperties, String allowedProperties)
		{
			NameValueCollection webParams = null;

			switch ( from )
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

			ArrayList errorList = new ArrayList();
			
			object instance = binder.BindObject( paramType, prefix, webParams, Context.Request.Files, errorList, nestedLevel, excludedProperties, allowedProperties );
						
			boundInstances[instance] = errorList;

			return instance;
		}

		protected object BindObjectInstance(object instance, ParamStore from, String prefix)
		{
			NameValueCollection webParams = null;

			switch ( from )
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

			ArrayList errorList = new ArrayList();
			
			binder.BindObjectInstance( instance, prefix, webParams, Context.Request.Files, errorList );
						
			boundInstances[instance] = errorList;

			return instance;
		}

		protected ErrorList GetDataBindErrors( object instance )
		{
			ArrayList list = boundInstances[ instance ] as ArrayList;

			return new ErrorList( list );
		}

		protected void CreateParamCollections( IRequest request )
		{
			formParams		= request.Form;
			queryParams		= request.QueryString;
			allParams		= request.Params;
		}
	}
}
