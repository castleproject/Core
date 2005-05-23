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
		internal override void CollectActions()
		{
			MethodInfo[] methods = 
				GetType().GetMethods( BindingFlags.Public|BindingFlags.Instance );
			
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
		}

		protected override void InvokeMethod(MethodInfo method, IRequest request)
		{
			NameValueCollection webParams = request.Params;
			ParameterInfo[] parameters = method.GetParameters();
            IDictionary files = request.Files;

			object[] methodArgs = BuildMethodArguments( parameters, webParams, files );
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

		protected virtual object[] BuildMethodArguments(ParameterInfo[] parameters, NameValueCollection webParams, IDictionary files)
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

					value = webParams.Get( paramName );

					args[i] = ConvertParam(param.ParameterType, files, paramName, value);
				}
			}
			catch(FormatException ex)
			{
				throw new RailsException( 
					String.Format("Could not convert {0} to request type. " + 
						"Argument value is '{1}'", paramName, value), ex );
			}
			catch(Exception ex)
			{
				throw new RailsException( 
					String.Format("Error builing method arguments. " + 
						"Last param analized was {0} with value '{1}'", paramName, value), ex );
			}

			return args;
		}

		private object ConvertParam(Type desiredType, IDictionary files, string paramName, string value)
		{
			if (desiredType == typeof(String))
			{
				return value;
			}
			else if (desiredType == typeof(Guid))
			{
				if (value != null)
				{
					return new Guid( value.ToString() );
				}
				else
				{
					return Guid.Empty; 
				}
			}
			else if (desiredType == typeof(UInt16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt16( value );
			}
			else if (desiredType == typeof(UInt32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt32( value );
			}
			else if (desiredType == typeof(UInt64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToUInt64( value );
			}
			else if (desiredType == typeof(Int16))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt16( value );
			}
			else if (desiredType == typeof(Int32))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt32( value );
			}
			else if (desiredType == typeof(Int64))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToInt64( value );
			}
			else if (desiredType == typeof(Byte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToByte( value );
			}
			else if (desiredType == typeof(SByte))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSByte( value );
			}
			else if (desiredType == typeof(Single))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToSingle( value );
			}
			else if (desiredType == typeof(Double))
			{
				if (value == String.Empty) value = null;
				return System.Convert.ToDouble( value );
			}
			else if (desiredType == typeof(DateTime))
			{
				if (value == String.Empty) value = null;
				return DateTime.Parse(value);
			}
			else if (desiredType == typeof(Boolean))
			{
				// TODO: Add true/on/1 variants
				return value != null;
			}
			else if (desiredType == typeof(HttpPostedFile))
			{
				return files[paramName];
			}
			else if (desiredType == typeof(String[]))
			{
				if (value == null) return null;
				return value.Split(',');
			}
			else if (desiredType == typeof(Int16[]) || desiredType == typeof(Int32[]) || desiredType == typeof(Int64[]) || 
				desiredType == typeof(UInt16[]) || desiredType == typeof(UInt32[]) || desiredType == typeof(UInt64[]) || 
				desiredType == typeof(byte[]) || desiredType == typeof(sbyte[]))
			{
				if (value == null) return null;
				return ConvertToArray(desiredType, value, files, paramName);
			}
			else
			{
				String message = String.Format("Ignoring argument {0} with value {1} " + 
					" as we don't know how to convert from this value to its type", paramName, value);
				Context.Trace(message);
			}

			return null;
		}

		private object ConvertToArray(Type desiredType, string value, IDictionary files, string paramName)
		{
			Type elemType = desiredType.GetElementType();
	
			String[] args = value.Split(',');
	
			Array newArray = Array.CreateInstance(elemType, args.Length);
	
			for(int i=0; i < args.Length; i++)
			{
				newArray.SetValue( ConvertParam(elemType, files, paramName, args[i]), i );
			}
	
			return newArray;
		}
	}
}
