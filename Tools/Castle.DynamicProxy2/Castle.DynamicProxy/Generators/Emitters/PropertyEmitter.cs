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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	
	public class PropertyEmitter : IMemberEmitter
	{
		private PropertyBuilder builder;
		private AbstractTypeEmitter parentTypeEmitter;
		private MethodEmitter getMethod;
		private MethodEmitter setMethod;
		// private ParameterInfo[] indexParameters;

		public PropertyEmitter(AbstractTypeEmitter parentTypeEmitter, String name, PropertyAttributes attributes, Type propertyType)
		{
			this.parentTypeEmitter = parentTypeEmitter;
			
			builder = parentTypeEmitter.TypeBuilder.DefineProperty(name, attributes, propertyType, new Type[0]);
		}

		public MethodEmitter GetMethod
		{
			get { return getMethod; }
			set { getMethod = value; }
		}

		public MethodEmitter SetMethod
		{
			get { return setMethod; }
			set { setMethod = value; }
		}

		public MethodEmitter CreateGetMethod()
		{
			return CreateGetMethod(MethodAttributes.Public | 
			                       MethodAttributes.Virtual | 
			                       MethodAttributes.SpecialName);
		}

		public MethodEmitter CreateGetMethod(MethodAttributes attrs, params Type[] parameters)
		{
			if (getMethod != null)
			{
				throw new InvalidOperationException("A getMethod exists");
			}

			if (parameters.Length == 0)
			{
				getMethod = new MethodEmitter(parentTypeEmitter, "get_" + builder.Name, attrs);
			}
			else
			{
				getMethod = new MethodEmitter(parentTypeEmitter, "get_" + builder.Name, 
				                              attrs, 
				                              new ReturnReferenceExpression(ReturnType), 
				                              ArgumentsUtil.ConvertToArgumentReference(parameters));
			}

			return getMethod;
		}

		public MethodEmitter CreateSetMethod()
		{
			return CreateSetMethod(MethodAttributes.Public | 
			                       MethodAttributes.Virtual | 
			                       MethodAttributes.SpecialName);
		}

		public MethodEmitter CreateSetMethod(Type arg)
		{
			return CreateSetMethod(MethodAttributes.Public |
								   MethodAttributes.Virtual |
								   MethodAttributes.SpecialName, arg);
		}

		public MethodEmitter CreateSetMethod(MethodAttributes attrs, params Type[] parameters)
		{
			if (setMethod != null)
			{
				throw new InvalidOperationException("A setMethod exists");
			}

			if (parameters.Length == 0)
			{
				setMethod = new MethodEmitter(parentTypeEmitter, "set_" + builder.Name, attrs);
			}
			else
			{
				setMethod = new MethodEmitter(parentTypeEmitter, "set_" + builder.Name,
											  attrs, new ReturnReferenceExpression(typeof(void)),
											  ArgumentsUtil.ConvertToArgumentReference(parameters));
			}

			return setMethod;
		}

		public MemberInfo Member
		{
			get { return null; }
		}

		public Type ReturnType
		{
			get { return builder.PropertyType; }
		}

		public void Generate()
		{
			if (setMethod != null)
			{
				setMethod.Generate();
				builder.SetSetMethod(setMethod.MethodBuilder);
			}

			if (getMethod != null)
			{
				getMethod.Generate();
				builder.SetGetMethod(getMethod.MethodBuilder);
			}
		}

		public void EnsureValidCodeBlock()
		{
			if (setMethod != null)
			{
				setMethod.EnsureValidCodeBlock();
			}

			if (getMethod != null)
			{
				getMethod.EnsureValidCodeBlock();
			}
		}
	}
}