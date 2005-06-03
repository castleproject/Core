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

namespace AspectSharp.Lang.AST
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for MethodSignature.
	/// </summary>
	[Serializable]
	public class MethodSignature
	{
		protected static readonly String ALL_MARK = "*";

		private string _access = string.Empty;
		private String _retType = String.Empty;
		private String _methodName = String.Empty;
		private ArrayList _arguments = new ArrayList();
		private bool _allAccess = false;
		private bool _allRetTypes = false;
		private bool _allArguments = false;

		protected MethodSignature()
		{
		}

		public MethodSignature(String retType, String methodName)
			: this("*", retType, methodName)
		{
		}

		public MethodSignature(String access, String retType, String methodName)
		{
			if (ALL_MARK.EndsWith(access))
			{
				_allAccess = true;
			}
			if (ALL_MARK.Equals(retType))
			{
				_allRetTypes = true;
			}
			_access = String.Intern(access.ToLower());
			_retType = String.Intern(retType.ToLower());
			_methodName = String.Intern(methodName);
		}

		public void AddArgumentType(String typeName)
		{
			if ( _arguments.Count == 0 && ALL_MARK.Equals(typeName))
			{
				_allArguments = true;
			}
			else
			{
				_allArguments = false;
			}
			_arguments.Add(String.Intern(typeName.ToLower()));
		}

		public string Access
		{
			get 
			{ 
				return _access;
			}
		}

		public String RetType
		{
			get { return _retType; }
		}

		public String MethodName
		{
			get { return _methodName; }
		}

		public String[] Arguments
		{
			get { return (String[]) _arguments.ToArray(typeof (String)); }
		}

		public bool AllAccess
		{
			get { return _allAccess; }
		}

		public bool AllRetTypes
		{
			get { return _allRetTypes; }
		}

		public bool AllArguments
		{
			get { return _allArguments; }
		}

		public override bool Equals(object obj)
		{
			MethodSignature otherMethod = obj as MethodSignature;

			if (otherMethod == null)
			{
				return false;
			}

			if (otherMethod.AllRetTypes == AllRetTypes &&
				otherMethod.AllArguments == AllArguments &&
				otherMethod.ArgumentsHashCode() == ArgumentsHashCode() &&
				otherMethod.MethodName.Equals(MethodName) &&
				otherMethod.RetType.Equals(RetType) &&
				otherMethod.Access.Equals(Access))
			{
				return true;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return ArgumentsHashCode() ^ MethodName.GetHashCode() ^ RetType.GetHashCode() ^ Access.GetHashCode();
		}

		public override String ToString()
		{
			return String.Format("({0} {1} {2}{3})", Access, RetType, MethodName, BuildArguments());
		}

		private int ArgumentsHashCode()
		{
			int argsHash = _arguments.Count + 1;
			foreach(object item in _arguments)
			{
				argsHash = argsHash ^ item.GetHashCode();
			}
			return argsHash;
		}

		private String BuildArguments()
		{
			if (_allArguments)
			{
				return "(" + ALL_MARK + ")";
			}
			else if (_arguments.Count == 0)
			{
				return "()";
			}
			else
			{
				// I know this is optimized et al.
				// TODO: Refactor

				String signature = "("; bool comma = false;

				foreach(object item in _arguments)
				{
					if (comma) signature += ", "; else comma = true;
					signature += item.ToString();
				}

				signature += ")";

				return signature;
			}
		}
	}
}