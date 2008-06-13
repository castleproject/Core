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
	using System.Reflection;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;

	/// <summary>
	/// Enables binding of JSON formatted values on POCO objects.
	/// </summary>
	/// <example>
	/// <para>
	/// The following demonstrates how to bind a JSON querystring value representing a Car object instance 
	/// to a POCO Car object instance:
	/// </para>
	/// The querystring:
	/// <code>
	/// car={Wheels=4,Year=2007,Model='Cheap'}
	/// </code>
	/// And you want to bind those values to a instance of yours Car class, which looks like this:
	/// <code>
	/// public class Car
	///	{
	///		private int wheels, year;
	///		private string model;
	///
	///		public int Wheels
	///		{
	///			get { return wheels; }
	///			set { wheels = value; }
	///		}
	///
	///		public int Year
	///		{
	///			get { return year; }
	///			set { year = value; }
	///		}
	///
	///		public string Model
	///		{
	///			get { return model; }
	///			set { model = value; }
	///		}
	///	}
	/// </code>
	/// <para>Using the <see cref="JSONBinderAttribute"/> and the <see cref="SmartDispatcherController"/>, all you have to 
	/// do is to mark the method parameter with the attribute, like the following example:</para>
	/// <code>
	/// public void MyAction([JSONBinder("car")] Car car)
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public class JSONBinderAttribute : Attribute, IParameterBinder
	{
		private string entryKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="JSONBinderAttribute"/> class.
		/// </summary>
		public JSONBinderAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JSONBinderAttribute"/> class.
		/// </summary>
		/// <param name="entryKey">The entry key, which is the form or 
		/// querystring key that identifies the JSON persisted content</param>
		public JSONBinderAttribute(string entryKey)
		{
			if (entryKey == null) throw new ArgumentNullException("entryKey");
			
			this.entryKey = entryKey;
		}

		/// <summary>
		/// Gets the entry key.
		/// </summary>
		/// <remarks>
		/// The entry key, which is the form or querystring key that identifies the JSON persisted content.
		/// </remarks>
		/// <value>The entry key.</value>
		public string EntryKey {
			get { return entryKey; }
		}

		/// <summary>
		/// Calculates the param points. Implementors should return value equals or greater than
		/// zero indicating whether the parameter can be bound successfully. The greater the value (points)
		/// the more successful the implementation indicates to the framework
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info.</param>
		/// <returns></returns>
		public int CalculateParamPoints(IEngineContext context, IController controller, 
			IControllerContext controllerContext, ParameterInfo parameterInfo)
		{
			EnsureValidEntryKey(parameterInfo.Name);

			return context.Request.Params[entryKey] != null ? 1 : 0; 
		}

		/// <summary>
		/// Binds the specified parameter for the action.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <param name="parameterInfo">The parameter info.</param>
		/// <returns>
		/// A instance based on the JSON values present in the <c>EntryKey</c> or the parameter name.
		/// </returns>
		public object Bind(IEngineContext context, IController controller, IControllerContext controllerContext, ParameterInfo parameterInfo)
		{
			EnsureValidEntryKey(parameterInfo.Name);

			string entryValue = context.Request.Params[entryKey];

			IJSONSerializer serializer = context.Services.JSONSerializer;

			return serializer.Deserialize(entryValue, parameterInfo.ParameterType);
		}

		private void EnsureValidEntryKey(string name)
		{
			if (entryKey == null)
			{
				entryKey = name;
			}
		}
	}
}