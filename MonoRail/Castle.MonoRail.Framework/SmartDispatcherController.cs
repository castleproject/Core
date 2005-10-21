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

	/// <summary>
	/// Specialization of <see cref="Controller"/> that tries
	/// to match the request params to method arguments.
	/// </summary>
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
			binder = new DataBinder( Context );
		}

		protected internal override void CollectActions()
		{
			MethodInfo[] methods = GetType().GetMethods( BindingFlags.Public|BindingFlags.Instance );
			
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
			NameValueCollection webParams = request.Params;

			object methods = actions[action];

			ArrayList candidates = methods as ArrayList;

			if (candidates == null && methods != null)
			{
				candidates = new ArrayList();
				candidates.Add(methods);
			}

			if (candidates == null)
			{
				return null;
			}

			return SelectBestCandidate( 
				(MethodInfo[]) candidates.ToArray( typeof(MethodInfo) ), 
				webParams );
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

			ParameterInfo[] parameters = candidate.GetParameters();

			if (parameters.Length == webParams.Count)
			{
				points = 10;
			}

			foreach(ParameterInfo param in parameters)
			{
				object value = webParams.Get( param.Name );
				if (value != null ) points += 10;
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

					object[] bindAttributes	= param.GetCustomAttributes( typeof(DataBindAttribute), false );
					
					if ( bindAttributes.Length > 0 )
					{
						DataBindAttribute dba = bindAttributes[0] as DataBindAttribute;
	
						args[i] = BindObject( dba.From, param.ParameterType, dba.Prefix, dba.NestedLevel, dba.Exclude );
					}
					else
					{
						args[i] = DataBinder.Convert( param.ParameterType, allParams.GetValues( paramName ), param.Name, files, Context );
					}
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

		protected object BindObject(ParamStore from, Type paramType, String prefix, int nestedLevel, String excludedProperties)
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
			
			object instance = binder.BindObject( paramType, prefix, webParams, Context.Request.Files, errorList, nestedLevel, excludedProperties );
						
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
