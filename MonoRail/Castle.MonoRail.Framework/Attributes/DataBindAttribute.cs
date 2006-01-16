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
	using System.Collections.Specialized;
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
	[AttributeUsage( AttributeTargets.Parameter, AllowMultiple=false, Inherited=false )]
	public class DataBindAttribute : Attribute, IParameterBinder
	{
		private ParamStore _from = ParamStore.Params;
		private String _exclude = String.Empty;
		private String _allow = String.Empty;
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
			get { return _exclude; }
			set { _exclude = value; }
		}

		/// <summary>
		/// Gets or sets the property names to allow.
		/// </summary>
		/// <value>A comma separated list 
		/// of property names to allow from databinding.</value>
		public String Allow
		{
			get { return _allow; }
			set { _allow = value; }
		}
		
		/// <summary>
		/// Gets or sets <see cref="ParamStore"/> used to 
		/// indicate where to get the values to databinding.
		/// </summary>
		/// <value>The <see cref="ParamStore"/> type.  
		/// Typically <see cref="ParamStore.Params"/>.</value>
		public ParamStore From
		{
			get { return _from; }
			set { _from = value; }
		}

		/// <summary>
		/// Gets the databinding prefix.
		/// </summary>
		/// <value>The databinding prefix.</value>
		public String Prefix
		{
			get { return prefix; }
		}

		public virtual object Bind(SmartDispatcherController controller, ParameterInfo parameterInfo)
		{
			DataBinder binder = controller.Binder;

			NameValueCollection coll = ResolveParams(controller);

			ConfigureBinder(binder, controller);

			return binder.BindObject(parameterInfo.ParameterType, prefix, _exclude, _allow, new NameValueCollectionAdapter(coll));
		}

		protected void ConfigureBinder(DataBinder binder, SmartDispatcherController controller)
		{
			binder.Files = controller.Context.Request.Files;
		}

		protected NameValueCollection ResolveParams(SmartDispatcherController controller)
		{
			return controller.ResolveParamsSource(From);
		}
	}
}
