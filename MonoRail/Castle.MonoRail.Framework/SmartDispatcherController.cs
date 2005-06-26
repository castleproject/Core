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
	using Castle.MonoRail.Framework.Attributes;

	/// <summary>
	/// Specialization of <see cref="Controller"/> that tries
	/// to match the request params to method arguments.
	/// </summary>
	public abstract class SmartDispatcherController : Controller
	{
		private IDictionary boundInstances = new ListDictionary();

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

			// Release any bound instances
			foreach ( object o in boundInstances.Keys )
				_instanceFactory.Release( o, Context );
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
				throw new ControllerException( String.Format("No action for '{0}' found", action) );
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

			// Get case insensitive versions of the collections
			NameValueCollection allParams, formParams, queryParams, webParams = request.Params;
			CreateParamCollections( request, out allParams, out formParams, out queryParams );

			IDictionary files = request.Files;

			try
			{
				DataBinder binder = new DataBinder( _instanceFactory, Context );

				for(int i=0; i < args.Length; i++)
				{
					ParameterInfo param		= parameters[i];
					paramName				= param.Name;

					object[] bindAttributes	= param.GetCustomAttributes( typeof(DataBindAttribute), true );
					
					if ( bindAttributes.Length > 0 )
					{
						DataBindAttribute dba = bindAttributes[0] as DataBindAttribute;
	
						switch ( dba.From )
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
						args[i]	= binder.BindObject( param.ParameterType, dba.Prefix, webParams, files, errorList );
						
						boundInstances.Add( args[i], errorList );
					}
					else
					{
						args[i] = binder.Convert( param.ParameterType, allParams.GetValues( paramName ), param.Name, files );
					}
				}
			}
			catch(FormatException ex)
			{
				throw new RailsException( 
					String.Format("Could not convert {0} to request type. " + 
						"Argument value is '{1}'", paramName, webParams.Get( paramName ) ), ex );
			}
			catch(Exception ex)
			{
				throw new RailsException( 
					String.Format("Error building method arguments. " + 
						"Last param analized was {0} with value '{1}'", paramName, value), ex );
			}

			return args;
		}

		protected virtual ErrorList GetDataBindErrors( object instance )
		{
			ArrayList list = boundInstances[ instance ] as ArrayList;

			return new ErrorList( list );
		}

		protected virtual void CreateParamCollections( IRequest request, out NameValueCollection allParams, 
														out NameValueCollection formParams, out NameValueCollection queryParams )
		{
			formParams		= request.Form;
			queryParams		= request.QueryString;
			allParams		= request.Params;
		}
	}
}
