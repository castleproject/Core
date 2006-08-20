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

namespace Castle.DynamicProxy.Builder.CodeBuilder
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Builder.CodeBuilder.SimpleAST;
	using Castle.DynamicProxy.Builder.CodeBuilder.Utils;

	/// <summary>
	/// Summary description for EasyProperty.
	/// </summary>
	[CLSCompliant(false)]
	public class EasyProperty : IEasyMember
	{
		private PropertyBuilder _builder;
		private AbstractEasyType _maintype;
		private EasyMethod _getMethod;
		private EasyMethod _setMethod;
		private ParameterInfo[] _indexParameters;

		public EasyProperty(AbstractEasyType maintype, String name, Type returnType)
		{
			_maintype = maintype;
			_builder = maintype.TypeBuilder.DefineProperty(
				name, PropertyAttributes.None, returnType, new Type[0]);
		}

		public String Name
		{
			get { return _builder.Name; }
		}

		public EasyMethod CreateGetMethod()
		{
			return CreateGetMethod(
				MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName);
		}

		public EasyMethod CreateGetMethod(MethodAttributes attrs, params Type[] parameters)
		{
			if (_getMethod != null)
			{
				return _getMethod;
			}

			_getMethod = new EasyMethod(_maintype, "get_" + _builder.Name,
			                            attrs,
			                            new ReturnReferenceExpression(ReturnType),
			                            ArgumentsUtil.ConvertToArgumentReference(parameters));

			return _getMethod;
		}

		public EasyMethod CreateSetMethod(Type arg)
		{
			return CreateSetMethod(
				MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.SpecialName, arg);
		}

		public EasyMethod CreateSetMethod(MethodAttributes attrs, params Type[] parameters)
		{
			if (_setMethod != null)
			{
				return _setMethod;
			}

			_setMethod = new EasyMethod(_maintype, "set_" + _builder.Name,
			                            attrs,
			                            new ReturnReferenceExpression(typeof(void)),
			                            ArgumentsUtil.ConvertToArgumentReference(parameters));

			return _setMethod;
		}

		#region IEasyMember Members

		public void Generate()
		{
			if (_setMethod != null)
			{
				_setMethod.Generate();
				_builder.SetSetMethod(_setMethod.MethodBuilder);
			}

			if (_getMethod != null)
			{
				_getMethod.Generate();
				_builder.SetGetMethod(_getMethod.MethodBuilder);
			}
		}

		public void EnsureValidCodeBlock()
		{
			if (_setMethod != null)
			{
				_setMethod.EnsureValidCodeBlock();
			}

			if (_getMethod != null)
			{
				_getMethod.EnsureValidCodeBlock();
			}
		}

		public MethodBase Member
		{
			get { return null; }
		}

		public Type ReturnType
		{
			get { return _builder.PropertyType; }
		}

		public ParameterInfo[] IndexParameters
		{
			get { return _indexParameters; }
			set { _indexParameters = value; }
		}

		#endregion
	}
}