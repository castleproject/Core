namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	class GenericUtil
	{
		public static void PopulateGenericArguments(AbstractTypeEmitter parentEmitter, Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			foreach(GenericTypeParameterBuilder genType in parentEmitter.GenericTypeParams)
			{
				name2GenericType.Add(genType.Name, genType);
			}
		}

		public static GenericTypeParameterBuilder[] DefineGenericArguments(Type[] genericArguments, 
		                                                                   MethodBuilder builder, 
		                                                                   Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			GenericTypeParameterBuilder[] genericTypeParams = null;
			
			String[] argumentNames = new String[genericArguments.Length];

			for (int i = 0; i < argumentNames.Length; i++)
			{
				argumentNames[i] = genericArguments[i].Name;
			}

			if (argumentNames.Length != 0)
			{
				genericTypeParams = builder.DefineGenericParameters(argumentNames);

				for (int i = 0; i < genericTypeParams.Length; i++)
				{
					try
					{
						GenericParameterAttributes attributes = genericArguments[i].GenericParameterAttributes;
						Type[] types = genericArguments[i].GetGenericParameterConstraints();

						genericTypeParams[i].SetGenericParameterAttributes(attributes);

						Type[] interfacesConstraints = Array.FindAll(types, delegate(Type type)
							{
								return type.IsInterface;
							});

						Type baseClassConstraint = Array.Find(types, delegate(Type type)
							{
								return type.IsClass;
							});

						if (interfacesConstraints.Length != 0)
						{
							genericTypeParams[i].SetInterfaceConstraints(interfacesConstraints);
						}

						if (baseClassConstraint != null)
						{
							genericTypeParams[i].SetBaseTypeConstraint(baseClassConstraint);
						}
					}
					catch (NotSupportedException)
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

		public static Type[] ExtractParametersTypes(ParameterInfo[] baseMethodParameters, 
		                                            Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			Type[] newParameters = new Type[baseMethodParameters.Length];

			for (int i = 0; i < baseMethodParameters.Length; i++)
			{
				ParameterInfo param = baseMethodParameters[i];
				Type paramType = param.ParameterType;

				newParameters[i] = ExtractCorrectType(paramType, name2GenericType);
			}

			return newParameters;
		}

		public static Type ExtractCorrectType(Type paramType,
											  Dictionary<string, GenericTypeParameterBuilder> name2GenericType)
		{
			if (paramType.IsArray)
			{
				Type underlyingType = paramType.GetElementType();

				if (underlyingType.IsGenericParameter)
				{
					GenericTypeParameterBuilder genericType = name2GenericType[underlyingType.Name];

					// newParameters[i] = genericType.MakeArrayType(param.ParameterType.GetArrayRank());
					return genericType.MakeArrayType();
				}
			}

			if (paramType.IsGenericParameter)
			{
				return name2GenericType[paramType.Name];
			}

			return paramType;
		}
	}
}
