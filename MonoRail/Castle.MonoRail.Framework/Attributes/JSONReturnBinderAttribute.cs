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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Services;

	/// <summary>
	/// Pendent
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = false)]
	public class JSONReturnBinderAttribute : Attribute, IReturnBinder
	{
		private string properties;

		/// <summary>
		/// Gets or sets the properties to be serialized.
		/// </summary>
		/// <value>The properties.</value>
		public string Properties
		{
			get { return properties; }
			set { properties = value; }
		}

		/// <summary>
		/// Binds the specified parameters for the action.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="returnType">Type being returned.</param>
		/// <param name="returnValue">The return value.</param>
		/// <returns></returns>
		public virtual void Bind(IEngineContext context, IController controller, IControllerContext controllerContext,
						 Type returnType, object returnValue)
		{
			// Cancel any view rendering
			controllerContext.SelectedViewName = null;

			SetContentType(context);

			string raw = GetRawValue(returnType, returnValue, context.Services.JSONSerializer);

			context.Response.Output.Write(raw);
		}

		/// <summary>
		/// Sets response content type
		/// </summary>
		/// <param name="context">The context</param>
		protected virtual void SetContentType(IEngineContext context)
		{
			string userAgent = context.Request.Headers["User-Agent"];

			// Ridiculous hack, but necessary. If we set the mime type for mobile clients, they 
			// will treat the response as binary!! 
			if (userAgent != null && userAgent.IndexOf("Windows CE") == -1)
			{
				// Sets the mime type
				context.Response.ContentType = "application/json, text/javascript";
			}
		}

		/// <summary>
		/// Converts target object to its JSON representation
		/// </summary>
		/// <param name="returnType">Type of the target object</param>
		/// <param name="returnValue">The target object</param>
		/// <param name="serializer">The JSON serializer</param>
		/// <returns></returns>
		public virtual string GetRawValue(Type returnType, object returnValue, IJSONSerializer serializer)
		{
			if (returnValue == null)
				return returnType.IsArray ? "[]" : "{}";

			if (properties == null)
				return serializer.Serialize(returnValue);

			Type normalized = returnType.IsArray ? returnType.GetElementType() : returnType;

			return serializer.Serialize(returnValue, new PropertyConverter(normalized, properties));
		}

		class PropertyConverter : IJSONConverter
		{
			private readonly Type targetType;
			private List<PropertyInfo> properties = new List<PropertyInfo>();

			public PropertyConverter(Type targetType, string propertyNames)
			{
				this.targetType = targetType;

				foreach(string propertyName in propertyNames.Split(','))
				{
					PropertyInfo prop = targetType.GetProperty(propertyName);

					if (prop == null)
					{
						throw new MonoRailException("Failed to JSON serialize object. " + 
							"Property " + propertyName + " could not be found for type " + targetType.FullName);
					}

					properties.Add(prop);
				}
			}

			public void Write(IJSONWriter writer, object value)
			{
				writer.WriteStartObject();

				foreach(PropertyInfo info in properties)
				{
					object propVal = info.GetValue(value, null);

					writer.WritePropertyName(info.Name);
					writer.WriteValue(propVal);
				}

				writer.WriteEndObject();
			}

			public bool CanHandle(Type type)
			{
				return type == targetType;
			}

#if DOTNET35
			public object ReadJson(IJSONReader reader, Type objectType)
			{
				throw new NotImplementedException();
			}
#endif
		}
	}
}
