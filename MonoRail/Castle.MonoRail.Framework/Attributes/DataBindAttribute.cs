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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Reflection;

	using Castle.Components.Binder;

	public enum ParamStore
	{
		QueryString,
		Form,
		Params
	}

	/// <summary>
	/// The DataBind Attribute is used to indicate that an Action methods parameter 
	/// is to be intercepted and handled by the <see cref="Castle.Components.Binder.DataBinder"/>.
	/// </summary>
	/// <remarks>
	/// Allowed usage is one per method parameter, and is not inherited.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple=false, Inherited=false)]
	public class DataBindAttribute : Attribute, IParameterBinder
	{
		private ParamStore from = ParamStore.Params;
		private String exclude = String.Empty;
		private String allow = String.Empty;
		private String prefix;

		public DataBindAttribute(String prefix)
		{
			this.prefix = prefix;
		}

		/// <summary>
		/// Gets or sets the property names to exclude.
		/// </summary>
		/// <value>A comma separated list 
		/// of property names to exclude from databinding.</value>
		public String Exclude
		{
			get { return exclude; }
			set { exclude = value; }
		}

		/// <summary>
		/// Gets or sets the property names to allow.
		/// </summary>
		/// <value>A comma separated list 
		/// of property names to allow from databinding.</value>
		public String Allow
		{
			get { return allow; }
			set { allow = value; }
		}
		
		/// <summary>
		/// Gets or sets <see cref="ParamStore"/> used to 
		/// indicate where to get the values from
		/// </summary>
		/// <value>The <see cref="ParamStore"/> type.  
		/// Typically <see cref="ParamStore.Params"/>.</value>
		public ParamStore From
		{
			get { return from; }
			set { from = value; }
		}

		/// <summary>
		/// Gets the databinding prefix.
		/// </summary>
		/// <remarks>
		/// The prefix is a name followed by a 
		/// dot that prefixes the entries names 
		/// on the source http request.
		/// </remarks>
		/// <value>The databinding prefix.</value>
		public String Prefix
		{
			get { return prefix; }
		}

		public int CalculateParamPoints(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			CompositeNode node = controller.ObtainParamsNode(From);

			DataBinder binder = controller.Binder;

			return binder.CanBindObject(parameterInfo.ParameterType, prefix, node) ? 10 : 0;
		}

		public virtual object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			DataBinder binder = controller.Binder;

			CompositeNode node = controller.ObtainParamsNode(From);

			object instance = binder.BindObject(parameterInfo.ParameterType, prefix, exclude, allow, node);

			if (instance != null)
			{
				controller.BoundInstanceErrors[instance] = binder.ErrorList;
			}

			return instance;
		}
	}
}
