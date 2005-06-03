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

namespace AspectSharp.Core.Matchers
{
	using System;
	using System.Text.RegularExpressions;
	using System.Collections;
	using System.Reflection;

	using AspectSharp.Lang.AST;

	/// <summary>
	/// Summary description for DefaultJoinPointMatcher.
	/// </summary>
	[Serializable]
	public class DefaultJoinPointMatcher : IJoinPointMatcher
	{
		private PointCutDefinitionCollection _pointcuts;

		public DefaultJoinPointMatcher( PointCutDefinitionCollection pointcuts )
		{
			_pointcuts = pointcuts;
		}

		#region IJoinPointMatcher Members

		public virtual PointCutDefinition[] Match(MethodInfo method)
		{
			ArrayList list = new ArrayList();

			foreach(PointCutDefinition pointcut in _pointcuts)
			{
				MethodSignature signature = pointcut.Method;

				if (!FlagsMatchMethodType(method, pointcut))
				{
					continue;
				}

				if (signature != AllMethodSignature.Instance)
				{
					if (!NameMatch(signature, method, pointcut.Flags) || 
						!ReturnTypeMatch(signature, method) || 
						!AccessMatch(signature, method) ||
						!ArgumentsMatch(signature, method))
					{
						continue;
					}
				}

				list.Add( pointcut );
			}

			return (PointCutDefinition[]) list.ToArray( typeof(PointCutDefinition) );
		}

		#endregion

		protected virtual bool NameMatch(MethodSignature signature, MethodInfo method, PointCutFlags flags)
		{
			String sign = signature.MethodName;
			String name = method.Name;

			if (sign.IndexOf('*') != -1)
			{
				return Regex.IsMatch( name, sign );
			}
			else if (( method.IsSpecialName && (((int)(flags & PointCutFlags.Property)) != 0)) || 
				( name.StartsWith("get_") && (((int)(flags & PointCutFlags.PropertyRead)) != 0)) ||
				( name.StartsWith("set_") && (((int)(flags & PointCutFlags.PropertyWrite)) != 0)))
			{
				name = name.Substring(4);
			}

			return name.Equals(sign);
		}

		protected virtual bool ReturnTypeMatch(MethodSignature signature, MethodInfo method)
		{
			if (signature.AllRetTypes)
			{
				return true;
			}

			return TypeMatch(signature.RetType, method.ReturnType);
		}

		protected virtual bool AccessMatch(MethodSignature signature, MethodInfo method)
		{
			if (signature.AllAccess)
				return true;

			return ((signature.Access == "public" && method.IsPublic) ||
				(signature.Access == "private" && method.IsPrivate) ||
				(signature.Access == "internal" && method.IsAssembly));
		}

		protected virtual bool ArgumentsMatch(MethodSignature signature, MethodInfo method)
		{
			if (signature.AllArguments)
			{
				return true;
			}

			String[] arguments = signature.Arguments;
			ParameterInfo[] parameters = method.GetParameters();

			for(int i=0; i < arguments.Length; i++ )
			{
				String argName = arguments[i];

				if (argName.Equals("*"))
				{
					break;
				}
				else if (i == parameters.Length)
				{
					return false;
				}
				else if (i == arguments.Length - 1 && arguments.Length != parameters.Length)
				{
					return false;
				}

				if (!TypeMatch( argName, parameters[i].ParameterType ))
				{
					return false;
				}
			}

			return true;
		}

		protected virtual bool TypeMatch(String argSignature, Type parameterType)
		{
			argSignature = NormalizeTypeName(argSignature);
			String name = parameterType.FullName;

			if (argSignature.IndexOf('*') != -1)
			{
				return Regex.IsMatch( name, argSignature );
			}
			else
			{
				return String.Compare( name, argSignature, true ) == 0;
			}
		}

		protected virtual String NormalizeTypeName(String type)
		{
			String fullTypeName = TypeAliasDictionary.Instance[ type ];
			return fullTypeName != null ? fullTypeName : type;
		}

		protected virtual bool FlagsMatchMethodType(MethodInfo method, PointCutDefinition pointcut)
		{
			if ( !method.IsSpecialName && ((int)(pointcut.Flags & PointCutFlags.Method)) == 0 )
			{
				return false;
			}
	
			if (method.IsSpecialName)
			{
				if ( pointcut.Flags == PointCutFlags.Method )
				{
					return false;
				}
					
				if ( ((int)(pointcut.Flags & PointCutFlags.Property)) == 0 )
				{
					bool isPropertyGet = method.Name.StartsWith("get");

					if ( (!isPropertyGet && ((int)(pointcut.Flags & PointCutFlags.PropertyRead)) != 0) ||
						(isPropertyGet && ((int)(pointcut.Flags & PointCutFlags.PropertyWrite) != 0) ))
					{
						return false;
					}
				}
			}
			
			return true;
		}
	}
}
