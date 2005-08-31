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

namespace Castle.MonoRail.Framework.Filters
{
	using System.Configuration;
	using System.Globalization;
	using System.Threading;

	/// <summary>
	/// Enum to identify where a value is stored.
	/// </summary>
	public enum RequestStore
	{
		/// <summary>
		/// Value is stored in the Session object.
		/// </summary>
		Session = 1,
		/// <summary>
		/// Value is stored in a cookie object.
		/// </summary>
		Cookie = 2,
		/// <summary>
		/// Value is stored in the querystring.
		/// </summary>
		QueryString = 3,
		/// <summary>
		/// Value is stored in the form collection.
		/// </summary>
		Form = 4,
		/// <summary>
		/// Value is stored in either query string or form collection.
		/// </summary>
		Params = 5
	}

	/// <summary>
	/// The LocalizationFilter can be used to determine the culture to use
	/// for resources and UI.
	/// </summary>
	public class LocalizationFilter : IFilter, IFilterAttributeAware
	{
		private LocalizationFilterAttribute setup;

		#region IFilterAttributeAware Members

		public FilterAttribute FilterAttribute
		{
			set
			{
				if ( !(value is LocalizationFilterAttribute) )
				{
					throw new ConfigurationException( "LocalizationFilter can only be defined by a LocalizationFilterAttribute." );
				}

				setup = value as LocalizationFilterAttribute;
			}
		}

		#endregion

		#region IFilter Members

		public bool Perform( ExecuteEnum exec, IRailsEngineContext context, Controller controller )
		{
			try
			{
				string localeId	= GetLocaleId( context );

				if ( localeId == null && setup.UseBrowser )
				{
					localeId = GetUserLanguage( context.Request );
				}
				
				if ( localeId != null )
				{
					CultureInfo culture = CultureInfo.CreateSpecificCulture( localeId );

					Thread.CurrentThread.CurrentCulture = culture;
					Thread.CurrentThread.CurrentUICulture = culture;
				}
			}
			catch
			{
				if ( setup.FailOnError ) throw;
			}
			
			return true;
		}

        #endregion

		#region Get locale id from the store

		private string GetUserLanguage( IRequest request )
		{
			if ( request.UserLanguages != null && request.UserLanguages.Length > 0 )
			{
				return request.UserLanguages[0];
			}

			return null;	
		}

		private string GetLocaleId( IRailsEngineContext context )
		{
			switch ( setup.Store )
			{
				case RequestStore.Session:
					return context.Session[ setup.Key ] as string;
				case RequestStore.Cookie:
					return context.Request.ReadCookie( setup.Key );
				case RequestStore.QueryString:
					return context.Request.QueryString[ setup.Key ];
				case RequestStore.Form:
					return context.Request.Form[ setup.Key ];
				case RequestStore.Params:
					return context.Request.Params[ setup.Key ];
			}

			return null;
		}

		#endregion
	}
}
