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
	using Castle.MonoRail.Framework.Filters;

	/// <summary>
	/// This is an special filter attribute. It is used
	/// to define from where MonoRail should read the localization information
	/// to find out the locale of the client. 
	/// <para>
	/// For example, it can use the browser, or a cookie, an entry in the 
	/// query string (or even in the session)
	/// </para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true), Serializable]
	public class LocalizationFilterAttribute : FilterAttribute
	{
		private bool failOnError = false, useBrowser = true;
		private String key = "locale";
		private RequestStore requestStore = RequestStore.Cookie;

		/// <summary>
		/// Defines that 
		/// you want to use a cookie named 
		/// "locale", but if that fails it falls back
		/// to the client's browser locale.
		/// </summary>
		public LocalizationFilterAttribute() : base(ExecuteEnum.BeforeAction, typeof(LocalizationFilter)) {}

		/// <summary>
		/// Defines a new LocalizationFilter.
		/// </summary>
		/// <param name="store">Location where the localization parameter is stored.</param>
		/// <param name="key">Name of the parameter in the store.</param>
		public LocalizationFilterAttribute(RequestStore store, String key) : this()
		{
			this.key = key;
			requestStore = store;
		}

		#region Properties

		/// <summary>
		/// Key under which the locale value is stored.
		/// </summary>
		public String Key
		{
			get { return key; }
			set { key = value; }
		}

		/// <summary>
		/// Location where the locale value is to be stored.
		/// </summary>
		public RequestStore Store
		{
			get { return requestStore; }
			set { requestStore = value; }
		}

		/// <summary>
		/// True if an exception is to be thrown when a specific
		/// culture appears to be incorrect (can't be created).
		/// </summary>
		public bool FailOnError
		{
			get { return failOnError; }
			set { failOnError = value; }
		}

		/// <summary>
		/// Use client browser defined languages as default.
		/// </summary>
		public bool UseBrowser
		{
			get { return useBrowser; }
			set { useBrowser = value; }
		}

		#endregion		
	}
}