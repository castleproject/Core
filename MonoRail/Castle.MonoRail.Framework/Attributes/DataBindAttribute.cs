// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	public enum ParamStore
	{
		QueryString,
		Form,
		Params
	}
	/// <summary>
	/// The DataBind Attribute is used to indicate that an Action methods parameter 
	/// is to be intercepted and handled by the <see cref="DataBinder"/>.
	/// </summary>
	/// <remarks>
	/// Allowed usage is one per method parameter, and is not inherited.
	/// </remarks>
	[AttributeUsage( AttributeTargets.Parameter, AllowMultiple=false, Inherited=false )]
	public class DataBindAttribute : Attribute
	{
		private String prefix = string.Empty;
		private ParamStore from	= ParamStore.Params;
		private String exclude = String.Empty;
		private int nestedLevel = 3;

		public DataBindAttribute()
		{
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
		/// Gets or sets <see cref="ParamStore"/> used to locate the values used for databinding.
		/// </summary>
		/// <value>The ParamStore type.  Typically either QueryString, Form, or Params.</value>
		public ParamStore From
		{
			get { return from; }
			set { from = value; }
		}

		/// <summary>
		/// Gets or sets the databinding prefix.
		/// </summary>
		/// <value>The databinding prefix.</value>
		public String Prefix
		{
			get { return prefix; }
			set { prefix = value; }
		}

		/// <summary>
		/// Gets or sets the nested level.
		/// </summary>
		/// <value>The nested level.</value>
		public int NestedLevel
		{
			get { return nestedLevel; }
			set { nestedLevel = value; }
		}
	}
}
