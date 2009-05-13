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
// limitations under the License.

namespace Castle.Components.Binder
{
	using System;
	using System.Collections.Generic;

	public class DefaultConverter : MarshalByRefObject, IConverter
	{
		private readonly List<IConverter> converters = new List<IConverter>();

		public DefaultConverter()
		{
			Initialize();
		}
		private void Initialize()
		{
			if(converters.Count==0)
			{
				AddConverter(new StringConverter());
				AddConverter(new ExactMatchConverter());
				AddConverter(new ArrayConverter(this));
				AddConverter(new EnumConverter());
				AddConverter(new DecimalConverter());
				AddConverter(new PrimitiveConverter());
				AddConverter(new NullableConverter(this));
				AddConverter(new GuidConverter());
				AddConverter(new DateTimeConverter());
				AddConverter(new HttpPostedFileConverter());
				AddConverter(new GenericListConverter(this));
				AddConverter(new TypeConverterAdapter());

			}
		}

		public virtual void AddConverter(IConverter converter)
		{
			converters.Add(converter);
		}
		private IConverter FindConverter(Type desiredType,object input)
		{
			bool exactMatch;
			var converter = converters.Find(c => c.CanConvert(desiredType, null, input, out exactMatch));
			if(converter==null)
			{
				throw new BindingException("Unable to find converter for '{0}'", desiredType);
			}
			return converter;
			
		}
		public virtual bool CanConvert(Type desiredType, Type inputType, object input, out bool exactMatch)
		{
			exactMatch = false;
			foreach (var converter in converters)
			{
				if(converter.CanConvert(desiredType,inputType,input,out exactMatch))
				{
					return true;
				}
			}
			return false;
		}

		public virtual object Convert(Type desiredType, Type inputType, object input, out bool conversionSucceeded)
		{
			try
			{
				conversionSucceeded = (input != null);
				var converterImpl = FindConverter(desiredType, input);
				return converterImpl.Convert(desiredType, null, input, out conversionSucceeded);
			}
			catch (BindingException)
			{
				throw;
			}
			catch (Exception inner)
			{
				conversionSucceeded = false;

				ThrowInformativeException(desiredType, input, inner);
				return null;
			}

		}

		/// <summary>
		/// Convert the input param into the desired type
		/// </summary>
		/// <param name="desiredType">Type of the desired</param>
		/// <param name="input">The input</param>
		/// <param name="conversionSucceeded">if <c>false</c> the return value must be ignored</param>
		/// <remarks>
		/// There are 3 possible cases when trying to convert:
		/// 1) Input data for conversion missing (input is null or an empty String)
		///		Returns default conversion value (based on desired type) and set <c>conversionSucceeded = false</c>
		/// 2) Has input data but cannot convert to particular type
		///		Throw exception and set <c>conversionSucceeded = false</c>
		/// 3) Has input data and can convert to particular type
		/// 	 Return input converted to desired type and set <c>conversionSucceeded = true</c>
		/// </remarks>
		public object Convert(Type desiredType, object input, out bool conversionSucceeded)
		{
			if (desiredType.IsInstanceOfType(input))
			{
				conversionSucceeded = true;
				return input;
			}
			return Convert(desiredType, null, input, out conversionSucceeded);
		}
		private static void ThrowInformativeException(Type desiredType, object input, Exception inner)
		{
			String message = String.Format("Conversion error: " +
			                               "Could not convert parameter with value '{0}' to expected type {1}", input,
			                               desiredType);

			throw new BindingException(message, inner);
		}

	}
}
