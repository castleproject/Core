// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	internal delegate GenericTypeParameterBuilder[] ApplyGenArgs(String[] argumentNames);

	internal class GenericUtil
	{
		public static void PopulateGenericArguments(
			AbstractTypeEmitter parentEmitter,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			if (parentEmitter.GenericTypeParams == null) return;

			foreach(GenericTypeParameterBuilder genType in parentEmitter.GenericTypeParams)
			{
				name2GenericType.Add(genType.Name, genType);
			}
		}

		public static GenericTypeParameterBuilder[] DefineGenericArguments(
			Type[] genericArguments, TypeBuilder builder, 
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			return
				DefineGenericArguments(genericArguments, name2GenericType,
				                       delegate(String[] args) { return builder.DefineGenericParameters(args); });
		}

		public static GenericTypeParameterBuilder[] DefineGenericArguments(
			Type[] genericArguments,
			MethodBuilder builder,
			Dictionary<String, GenericTypeParameterBuilder>
				name2GenericType)
		{
			return
				DefineGenericArguments(genericArguments, name2GenericType,
				                       delegate(String[] args) { return builder.DefineGenericParameters(args); });
		}

		private static GenericTypeParameterBuilder[] DefineGenericArguments(
			Type[] genericArguments,
			Dictionary<String, GenericTypeParameterBuilder>
				name2GenericType,
			ApplyGenArgs gen)
		{
			GenericTypeParameterBuilder[] genericTypeParams = null;

			String[] argumentNames = new String[genericArguments.Length];

			for(int i = 0; i < argumentNames.Length; i++)
			{
				argumentNames[i] = genericArguments[i].Name;
			}

			if (argumentNames.Length != 0)
			{
				genericTypeParams = gen(argumentNames);

				for(int i = 0; i < genericTypeParams.Length; i++)
				{
					try
					{
						GenericParameterAttributes attributes = genericArguments[i].GenericParameterAttributes;
						Type[] types = genericArguments[i].GetGenericParameterConstraints();

						genericTypeParams[i].SetGenericParameterAttributes(attributes);

						Type[] interfacesConstraints = Array.FindAll(types, delegate(Type type) { return type.IsInterface; });

						Type baseClassConstraint = Array.Find(types, delegate(Type type) { return type.IsClass; });

						if (interfacesConstraints.Length != 0)
						{
							for(int j = 0; j < interfacesConstraints.Length; ++j)
							{
								interfacesConstraints[j] =
									SubstituteGenericArguments(interfacesConstraints[j], genericArguments, genericTypeParams);
							}
							genericTypeParams[i].SetInterfaceConstraints(interfacesConstraints);
						}

						if (baseClassConstraint != null)
						{
							baseClassConstraint = SubstituteGenericArguments(baseClassConstraint, genericArguments, genericTypeParams);
							genericTypeParams[i].SetBaseTypeConstraint(baseClassConstraint);
						}
					}
					catch(NotSupportedException)
					{
						// Doesnt matter

						genericTypeParams[i].SetGenericParameterAttributes(GenericParameterAttributes.None);
					}

					if (name2GenericType.ContainsKey(argumentNames[i]))
					{
						name2GenericType.Remove(argumentNames[i]);
					}

					name2GenericType.Add(argumentNames[i], genericTypeParams[i]);
				}
			}

			return genericTypeParams;
		}

		private static Type SubstituteGenericArguments(
			Type type, Type[] argumentsToSubstitute, GenericTypeParameterBuilder[] substitutes)
		{
			if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				type = type.GetGenericTypeDefinition();

				for(int i = 0; i < genericArguments.Length; ++i)
				{
					int substitutionIndex = Array.IndexOf(argumentsToSubstitute, genericArguments[i]);
					if (substitutionIndex != -1)
					{
						genericArguments[i] = substitutes[substitutionIndex];
					}
				}
				return type.MakeGenericType(genericArguments);
			}
			else
			{
				return type;
			}
		}

		public static Type[] ExtractParametersTypes(
			ParameterInfo[] baseMethodParameters,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			Type[] newParameters = new Type[baseMethodParameters.Length];

			for(int i = 0; i < baseMethodParameters.Length; i++)
			{
				ParameterInfo param = baseMethodParameters[i];
				Type paramType = param.ParameterType;

				newParameters[i] = ExtractCorrectType(paramType, name2GenericType);
			}

			return newParameters;
		}

		public static Type ExtractCorrectType(
			Type paramType,
			Dictionary<string, GenericTypeParameterBuilder> name2GenericType)
		{
			if (paramType.IsArray)
			{
				int rank = paramType.GetArrayRank();

				Type underlyingType = paramType.GetElementType();

				if (underlyingType.IsGenericParameter)
				{
					GenericTypeParameterBuilder genericType;
					if (name2GenericType.TryGetValue(underlyingType.Name, out genericType) == false)
						return paramType;
 
					if (rank == 1)
					{
						return genericType.MakeArrayType();
					}
					else
					{
						return genericType.MakeArrayType(rank);
					}
				}
				else
				{
					if (rank == 1)
					{
						return underlyingType.MakeArrayType();
					}
					else
					{
						return underlyingType.MakeArrayType(rank);
					}
				}
			}

			if (paramType.IsGenericParameter)
			{
				GenericTypeParameterBuilder value;
				if (name2GenericType.TryGetValue(paramType.Name, out value))
					return value;
			}

			return paramType;
		}

		public static Type[] ExtractParameterTypes(ParameterInfo[] baseMethodParameters)
		{
			Type[] newParameters = new Type[baseMethodParameters.Length];

			for(int i = 0; i < baseMethodParameters.Length; i++)
			{
				ParameterInfo param = baseMethodParameters[i];
				Type paramType = param.ParameterType;

				newParameters[i] = paramType;
			}

			return newParameters;
		}
	}
}