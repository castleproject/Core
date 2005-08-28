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

	using Castle.MonoRail.Framework.Filters;

	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true), Serializable]
	public class LocalizationFilterAttribute : FilterAttribute
	{
		private bool _FailOnError = false, _UseBrowser = true;
		private string _Key			= "locale";
		private RequestStore _Store = RequestStore.Cookie;
		
		public LocalizationFilterAttribute() : base( ExecuteEnum.Before, typeof(LocalizationFilter) )
		{
		}

		/// <summary>
		/// Defines a new LocalizationFilter.
		/// </summary>
		/// <param name="store">Location where the localization parameter is stored.</param>
		/// <param name="key">Name of the parameter in the store.</param>
		public LocalizationFilterAttribute( RequestStore store, string key ) : this()
		{
			_Key	= key;
			_Store	= store;
		}

        #region Properties

		/// <summary>
		/// Key under which the locale value is stored.
		/// </summary>
		public string Key
		{
			get { return _Key; }
			set { _Key = value; }
		}

		/// <summary>
		/// Location where the locale value is to be stored.
		/// </summary>
		public RequestStore Store
		{
			get { return _Store; }
			set { _Store = value; }
		}

		/// <summary>
		/// True if an exception is to be thrown when a specific
		/// culture appears to be incorrect (can't be created).
		/// </summary>
		public bool FailOnError
		{
			get { return _FailOnError; }
			set { _FailOnError = value; }
		}

		/// <summary>
		/// Use client browser defined languages as default.
		/// </summary>
		public bool UseBrowser
		{
			get { return _UseBrowser; }
			set { _UseBrowser = value; }
		}

		#endregion		
	}
}
