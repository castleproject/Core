// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel.Assemble
{
	using System;
	using System.Reflection;

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Model;

	/// <summary>
	/// 
	/// </summary>
	public delegate void ResolveTypeHandler( 
		IComponentModel model, 
		Type typeRequest, String argumentOrPropertyName, 
		object key, out object value );

	/// <summary>
	/// Summary description for Assembler.
	/// </summary>
	public abstract class Assembler
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="typeSpecified"></param>
		/// <param name="argumentOrPropertyName"></param>
		/// <returns></returns>
		public static bool IsKnown( Type typeSpecified, String argumentOrPropertyName )
		{
			if (IsLogger( typeSpecified ) || 
				IsContext( typeSpecified ) || 
				IsConfiguration( typeSpecified ))
			{
				return true;
			}

			// TODO: Support injection of Context entries / Configuration
			// directly into Constructor arguments or setmethods.

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="resolver"></param>
		/// <returns></returns>
		public static object[] BuildConstructorArguments( 
			IComponentModel model, object key, ResolveTypeHandler resolver )
		{
			AssertUtil.ArgumentNotNull( model, "model" );
			AssertUtil.ArgumentNotNull( resolver, "resolver" );

			ParameterInfo[] parameters = model.ConstructionModel.SelectedConstructor.GetParameters();
			object[] args = new object[ parameters.Length ];

			for(int i=0; i < args.Length; i++)
			{
				ParameterInfo parameter = parameters[i];
				Type service = parameter.ParameterType;
				String argumentName = parameter.Name;
				
				object value = Resolve( model, service, argumentName, key, resolver );
				
				args[ parameter.Position ] = value;
			}

			return args;			
		}

		public static void AssembleProperties( object instance, IComponentModel model, 
			object key, ResolveTypeHandler resolver )
		{
			AssertUtil.ArgumentNotNull( model, "model" );
			AssertUtil.ArgumentNotNull( resolver, "resolver" );

			foreach( PropertyInfo property in model.ConstructionModel.SelectedProperties )
			{
				Type service = property.PropertyType;
				String propertyName = property.Name;

				object value = Resolve( model, service, propertyName, key, resolver );

				if (value != null)
				{
					property.SetValue( instance, value, null );
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="type"></param>
		/// <param name="argumentOrPropertyName"></param>
		/// <param name="resolver"></param>
		/// <returns></returns>
		private static object Resolve( 
			IComponentModel model, Type type, 
			String argumentOrPropertyName, object key, ResolveTypeHandler resolver  )
		{
			if (IsLogger( type ))
			{
				return model.Logger;
			}
			if (IsContext( type ))
			{
				return model.Context;
			}
			if (IsConfiguration( type ))
			{
				return model.Configuration;
			}

			object value = null;

			resolver( model, type, argumentOrPropertyName, key, out value );

			return value;
		}

		private static bool IsLogger( Type typeSpecified )
		{
			return typeSpecified == typeof(ILogger);
		}

		private static bool IsContext( Type typeSpecified )
		{
			return typeSpecified == typeof(IContext);
		}

		private static bool IsConfiguration( Type typeSpecified )
		{
			return typeSpecified == typeof(IConfiguration);
		}
	}
}
