// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
// limitations under the License.using System;

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using System.Reflection;
	using Castle.Core.Configuration;
	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Conversion;

	[Serializable]
	public class DefaultComplexConverter : AbstractTypeConverter
	{
		private IConversionManager _conversionManager = null;

		#region ITypeConverter Member

		public override bool CanHandleType(Type type)
		{
			return !type.IsPrimitive;
		}

		public override object PerformConversion(IConfiguration configuration, Type targetType)
		{
			object instance = CreateInstance(targetType, configuration);
			ConvertPropertyValues(instance, targetType, configuration);

			return instance;
		}

		public override object PerformConversion(string value, Type targetType)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary>
		/// Creates the target type instance.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="configuration">The configuration.</param>
		/// <returns></returns>
		private object CreateInstance(Type type, IConfiguration configuration)
		{
			type = ObtainImplementation(type, configuration);	

			ConstructorInfo constructor = ChooseConstructor(type);

			object[] args = null;
			if (constructor != null)
			{
				args = ConvertConstructorParameters(constructor, configuration);
			}

			object instance = Activator.CreateInstance(type, args);
			return instance;
		}

		private Type ObtainImplementation(Type type, IConfiguration configuration)
		{
			String typeNode = configuration.Attributes["type"];
	
			if (String.IsNullOrEmpty(typeNode))
			{
				if (type.IsInterface)
				{
					throw new ConverterException("A type attribute must be specified for interfaces");
				}

				return type;
			}

			Type implType = (Type) Context.Composition.PerformConversion(typeNode, typeof(Type));

			if (!type.IsAssignableFrom(implType))
			{
				String message = String.Format("Type {0} is not assignable to {1}",
				                               implType.FullName, type.FullName);

				throw new ConverterException(message);
			}

			return implType;
		}

		/// <summary>
		/// Chooses the first non default constructor. Throws an exception if more than 
		/// one non default constructor is found
		/// </summary>
		/// <param name="type"></param>
		/// <returns>The chosen constructor, or <c>null</c> if none was found</returns>
		private ConstructorInfo ChooseConstructor(Type type)
		{
			ConstructorInfo chosen = null;
			ConstructorInfo[] constructors = type.GetConstructors();
			foreach(ConstructorInfo candidate in constructors)
			{
				if (candidate.GetParameters().Length == 0)
					continue;
				if (chosen != null)
					throw new ConverterException("Classes with more than one non-default constructor are not supported.");
				chosen = candidate;
			}

			return chosen;
		}

		/// <summary>
		/// Converts the constructor parameters.
		/// </summary>
		/// <param name="constructor">The constructor.</param>
		/// <param name="configuration">The configuration.</param>
		/// <returns></returns>
		private object[] ConvertConstructorParameters(ConstructorInfo constructor, IConfiguration configuration)
		{
			IConversionManager conversionManager = this.ConversionManager;

			ParameterInfo[] parameters = constructor.GetParameters();
			object[] parameterValues = new object[parameters.Length];

			for(int i = 0, n = parameters.Length; i < n; ++i)
			{
				ParameterInfo parameter = parameters[i];

				IConfiguration paramConfig = FindChildIgnoreCase(configuration, parameter.Name);
				if (paramConfig == null)
					throw new ConverterException("Child '" + parameter.Name + "' missing in " + configuration.Name + " element.");

				Type paramType = parameter.ParameterType;
				if (!this.ConversionManager.CanHandleType(paramType))
					throw new ConverterException("No converter found for child '" + parameter.Name + "' in " + configuration.Name +
					                             " element (type: " + paramType.Name + ").");


				parameterValues[i] = ConvertChildParameter(paramConfig, paramType);
			}

			return parameterValues;
		}

		/// <summary>
		/// Converts the property values.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="type">The type.</param>
		/// <param name="configuration">The configuration.</param>
		private void ConvertPropertyValues(object instance, Type type, IConfiguration configuration)
		{
			IConversionManager conversionManager = this.ConversionManager;

			foreach(IConfiguration propConfig in configuration.Children)
			{
				PropertyInfo property =
					type.GetProperty(propConfig.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
				if (property == null || !property.CanWrite)
					continue;

				Type propType = property.PropertyType;
				if (!this.ConversionManager.CanHandleType(propType))
					throw new ConverterException("No converter found for child '" + property.Name + "' in " + configuration.Name +
					                             " element (type: " + propType.Name + ").");

				object val = ConvertChildParameter(propConfig, propType);
				property.SetValue(instance, val, null);
			}
		}


		private object ConvertChildParameter(IConfiguration config, Type type)
		{
			if (config.Value == null && config.Children.Count != 0)
			{
				return this.Context.Composition.PerformConversion(config, type);
			}
			else
			{
				return this.Context.Composition.PerformConversion(config.Value, type);
			}
		}

		/// <summary>
		/// Gets the conversion manager.
		/// </summary>
		/// <value>The conversion manager.</value>
		private IConversionManager ConversionManager
		{
			get
			{
				if (_conversionManager == null)
				{
					_conversionManager = (IConversionManager)
					                     this.Context.Kernel.GetSubSystem(SubSystemConstants.ConversionManagerKey);
				}
				return _conversionManager;
			}
		}

		/// <summary>
		/// Finds the child (case insensitive).
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="name">The name.</param>
		/// <returns></returns>
		private IConfiguration FindChildIgnoreCase(IConfiguration config, string name)
		{
			foreach(IConfiguration child in config.Children)
			{
				if (string.Compare(child.Name, name, true) == 0)
					return child;
			}

			return null;
		}
	}
}