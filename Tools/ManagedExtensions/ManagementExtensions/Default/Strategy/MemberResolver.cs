// Copyright 2003-2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.ManagementExtensions.Default.Strategy
{
	using System;
	using System.Text;
	using System.Reflection;
	using System.Collections;

	/// <summary>
	/// Summary description for MemberResolver.
	/// </summary>
	public class MemberResolver
	{
		private Hashtable attributes = 
			new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
		
		private Hashtable operations = 
			new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);

		public MemberResolver(ManagementInfo info, Type target)
		{
			ResolveAttributes(info, target);
			ResolveOperations(info, target);
		}

		public MethodInfo[] Methods
		{
			get
			{
				MethodInfo[] methods = new MethodInfo[operations.Count];
				int index = 0;
				foreach(MethodInfo info in operations.Values)
				{
					methods[index++] = info;
				}
				return methods;
			}
		}

		public MethodInfo GetMethod(String methodName)
		{
			return (MethodInfo) operations[methodName];
		}

		public PropertyInfo GetProperty(String propertyName)
		{
			return (PropertyInfo) attributes[propertyName];
		}

		private void ResolveAttributes(ManagementInfo info, Type target)
		{
			foreach(ManagementObject item in info.Attributes)
			{
				PropertyInfo property = target.GetProperty( 
					item.Name, 
					BindingFlags.Public|BindingFlags.Instance );

				attributes.Add( item.Name, property );
			}
		}

		private void ResolveOperations(ManagementInfo info, Type target)
		{
			foreach(ManagementOperation item in info.Operations)
			{
				MethodInfo method = target.GetMethod( 
					item.Name, 
					BindingFlags.Public|BindingFlags.Instance, null, item.Arguments, null );

				operations.Add( BuildOperationName(item.Name, method.GetParameters()), method );
			}
		}

		public static bool Match(ParameterInfo[] parameters, Type[] signature)
		{
			if (parameters.Length == 0 && signature.Length == 0)
			{
				return true;
			}

			// TODO: Check for argument of type 'params'

			for(int i=0; i < parameters.Length; i++)
			{
				ParameterInfo info = parameters[i];

				int pos = info.Position;

				if (pos < signature.Length)
				{
					if ( !info.ParameterType.IsAssignableFrom( signature[pos] ) )
					{
						return false;
					}
				}
				else
				{
					return false;
				}
			}

			return true;
		}

		public static String BuildOperationName(String name, ParameterInfo[] args)
		{
			String result = String.Format("{0}[{1}]", name, GetArraySig(args));
			return result;
		}

		public static String BuildOperationName(String name, Type[] args)
		{
			String result = String.Format("{0}[{1}]", name, GetArraySig(args));
			return result;
		}

		private static String GetArraySig(ParameterInfo[] args)
		{
			if (args == null)
			{
				return String.Empty;
			}

			StringBuilder sb = new StringBuilder();
			
			foreach(ParameterInfo parameter in args)
			{
				if (sb.Length != 0)
				{
					sb.Append(",");
				}
				sb.Append(parameter.ParameterType);
			}

			return sb.ToString();
		}

		private static String GetArraySig(Type[] args)
		{
			if (args == null)
			{
				return String.Empty;
			}

			StringBuilder sb = new StringBuilder();
			
			foreach(Type parameter in args)
			{
				if (sb.Length != 0)
				{
					sb.Append(",");
				}
				sb.Append(parameter);
			}

			return sb.ToString();
		}
	}
}