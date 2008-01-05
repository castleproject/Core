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

	/// <summary>
	/// Decorates a public property in a <see cref="ViewComponent"/>
	/// to have the framework automatically bind the value using 
	/// the <see cref="ViewComponent.ComponentParams"/> dictionary. 
	/// </summary>
	/// <remarks>
	/// By default the property name is going to be used as a key to query the params. 
	/// <para>
	/// You can also use the <see cref="ViewComponentParamAttribute.Required"/>
	/// property to define that a parameter is non-optional. 
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>In the code below, the <c>Text</c> parameter will automatically be bound to the <c>header</c> property.
	/// If there is no <c>Text</c> parameter, a <see cref="ViewComponentException"/> will be thrown.</para>
	/// Simailrly, the optional <c>CssClass</c> parameter will be bound to the <c>CssClass</c> property.  No error
	/// occurs if there is no <c>CssClass</c> parameter.
	/// <code><![CDATA[
	/// public class HeaderViewComponent : ViewComponent
	/// {
	///      [ViewComponentParam("Text", Required= true)]
	///      public string header {get; set;}
	///      
	///      [ViewComponentParam]
	///      public string CssClass {get; set;}
	///      // :
	///      // :
	/// }
	/// ]]></code></example>
	/// <seealso cref="ViewComponent"/>
	/// <seealso cref="ViewComponentDetailsAttribute"/>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true), Serializable]
	public class ViewComponentParamAttribute : Attribute
	{
		private string paramName;
		private bool required;
		private object defaultValue;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentParamAttribute"/> class.
		/// </summary>
		public ViewComponentParamAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentParamAttribute"/> class 
		/// allowing you to override the parameter name to be queried on 
		/// the <see cref="ViewComponent.ComponentParams"/> dictionary.
		/// </summary>
		/// <param name="paramName">Overrides the name of the parameter.</param>
		public ViewComponentParamAttribute(string paramName)
		{
			this.paramName = paramName;
		}

		/// <summary>
		/// Gets or sets a value indicating whether a value for this property is required.
		/// </summary>
		/// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
		public bool Required
		{
			get { return required; }
			set { required = value; }
		}

		/// <summary>
		/// Gets the name of the param.
		/// </summary>
		/// <value>The name of the param.</value>
		public string ParamName
		{
			get { return paramName; }
		}

		/// <summary>
		/// Gets or sets the default value for the parameter.
		/// </summary>
		/// <value>The default.</value>
		public object Default
		{
			get { return defaultValue; }
			set { defaultValue = value; }
		}
	}
}