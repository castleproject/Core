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

namespace Castle.MicroKernel.SubSystems.Conversion
{
	using System;
	using System.Collections;

	using Castle.Model;
	using Castle.Model.Configuration;

	/// <summary>
	/// Composition of all available conversion managers
	/// </summary>
	[Serializable]
	public class DefaultConversionManager : AbstractSubSystem, IConversionManager, ITypeConverterContext
	{
		private IList converters;
		private IList standAloneConverters;
		private Stack modelStack = new Stack();

		public DefaultConversionManager()
		{
			converters = new ArrayList();
			standAloneConverters = new ArrayList();

			InitDefaultConverters();
		}

		protected virtual void InitDefaultConverters()
		{
			Add( new PrimitiveConverter() );
			Add( new TypeNameConverter() );
			Add( new EnumConverter() );
			Add( new ListConverter() );
			Add( new DictionaryConverter() );
			Add( new ArrayConverter() ); 
			Add( new ComponentConverter() ); 
		}

		#region IConversionManager Members

		public void Add(ITypeConverter converter)
		{
			converter.Context = this;

			converters.Add(converter);

			if (!(converter is IKernelDependentConverter))
			{
				standAloneConverters.Add(converter);
			}
		}

		public bool IsSupportedAndPrimitiveType(Type type)
		{
			foreach(ITypeConverter converter in standAloneConverters)
			{
				if (converter.CanHandleType(type)) return true;
			}

			return false;
		}

		#endregion

		#region ITypeConverter Members

		public ITypeConverterContext Context
		{
			get { return this; }
			set { throw new NotImplementedException(); }
		}

		public bool CanHandleType(Type type)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(type)) return true;
			}

			return false;
		}

		public object PerformConversion(String value, Type targetType)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(targetType)) 
					return converter.PerformConversion(value, targetType);
			}

			String message = String.Format("No converter registered to handle the type {0}", 
				targetType.FullName);

			throw new ConverterException(message);
		}

		public object PerformConversion(IConfiguration configuration, Type targetType)
		{
			foreach(ITypeConverter converter in converters)
			{
				if (converter.CanHandleType(targetType)) 
					return converter.PerformConversion(configuration, targetType);
			}

			String message = String.Format("No converter registered to handle the type {0}", 
				targetType.FullName);

			throw new ConverterException(message);
		}

		#endregion

		#region ITypeConverterContext Members

		IKernel ITypeConverterContext.Kernel
		{
			get { return base.Kernel; }
		}

		public void PushModel(ComponentModel model)
		{
			modelStack.Push(model);
		}

		public void PopModel()
		{
			modelStack.Pop();
		}

		public ComponentModel CurrentModel
		{
			get { if (modelStack.Count == 0) return null; 
				  else return (ComponentModel) modelStack.Peek(); }
		}

		public ITypeConverter Composition
		{
			get { return this; }
		}

		#endregion
	}
}
