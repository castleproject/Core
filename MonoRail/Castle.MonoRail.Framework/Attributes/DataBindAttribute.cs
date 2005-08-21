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
	/// 
	/// </summary>
	[AttributeUsage( AttributeTargets.Parameter, AllowMultiple=false, Inherited=false )]
	public class DataBindAttribute : Attribute
	{
		private String _Prefix		= string.Empty;
		private ParamStore _From	= ParamStore.Params;

		public DataBindAttribute()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public ParamStore From
		{
			get { return _From; }
			set { _From = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		public String Prefix
		{
			get { return _Prefix; }
			set { _Prefix = value; }
		}
	}
}
