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

namespace Castle.CastleOnRails.Framework
{
	using System;
	using System.Web;
	using System.Reflection;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Specialization of <see cref="Controller"/> that tries
	/// to match the request params to method arguments.
	/// </summary>
	public abstract class SmartDispatcherController : Controller
	{
		protected override void InvokeMethod(MethodInfo method, IRequest request)
		{
			NameValueCollection webParams = request.Params;
			ParameterInfo[] parameters = method.GetParameters();
            IDictionary files = request.Files;

			object[] methodArgs = BuildMethodArguments( parameters, webParams, files );
            method.Invoke(this, methodArgs);

		}

		protected override MethodInfo SelectMethod(String action, IRequest request)
		{
			NameValueCollection webParams = request.Params;

			Type type = this.GetType();

			MethodInfo[] methods = type.GetMethods( BindingFlags.Public|BindingFlags.Instance );
			
			ArrayList candidates = new ArrayList();

			foreach(MethodInfo method in methods)
			{
				if ( String.Compare(method.Name, action, true) == 0 )
				{
					candidates.Add(method);
				}
			}

			if (candidates.Count == 0)
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

		protected virtual int CalculatePoints(MethodInfo candidate, NameValueCollection webParams)
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

		private object[] BuildMethodArguments(ParameterInfo[] parameters, NameValueCollection webParams, IDictionary files)
		{
			object[] args = new object[parameters.Length];
			String paramName = String.Empty;
			String value = String.Empty;

			try
			{
				for(int i=0; i < args.Length; i++)
				{
					ParameterInfo param = parameters[i];
					paramName = param.Name;
					value = webParams.Get( param.Name );

					if (param.ParameterType == typeof(String))
					{
						args[i] = value;
					}
					else if (param.ParameterType == typeof(Guid))
					{
						if (value != null)
						{
							args[i] = new Guid( value.ToString() );
						}
						else
						{
							args[i] = Guid.Empty; 
						}
					}
					else if (param.ParameterType == typeof(int))
					{
						if (value == String.Empty) value = null;
						args[i] = System.Convert.ToInt32( value );
					}
					else if (param.ParameterType == typeof(long))
					{
						if (value == String.Empty) value = null;
						args[i] = System.Convert.ToInt64( value );
					}
					else if (param.ParameterType == typeof(Single))
					{
						if (value == String.Empty) value = null;
						args[i] = System.Convert.ToSingle( value );
					}
					else if (param.ParameterType == typeof(Double))
					{
						if (value == String.Empty) value = null;
						args[i] = System.Convert.ToDouble( value );
					}
					else if (param.ParameterType == typeof(Boolean))
					{
						args[i] = value != null;
					}
					else if (param.ParameterType == typeof(HttpPostedFile))
					{
						args[i] = files[param.Name];
					}
				}
			}
			catch(FormatException ex)
			{
				throw new RailsException( 
					String.Format("Could not convert {0} to request type. " + 
						"Argument value is '{1}'", paramName, value) );
			}
			catch(Exception ex)
			{
				throw new RailsException( 
					String.Format("Error builing method arguments. " + 
						"Last param analized was {0} with value '{1}'", paramName, value) );
			}

			return args;
		}
	}
}
