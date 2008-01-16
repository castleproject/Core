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

namespace Castle.MonoRail.Framework.Filters
{
	using System;
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

		/// <summary>
		/// Sets the filter.
		/// </summary>
		/// <value>The filter.</value>
		public FilterAttribute Filter
		{
			set
			{
				if (!(value is LocalizationFilterAttribute))
				{
					String message = "LocalizationFilter can only be defined by a LocalizationFilterAttribute.";
					throw new ConfigurationErrorsException(message);
				}

				setup = value as LocalizationFilterAttribute;
			}
		}

		#endregion

		#region IFilter Members

		/// <summary>
		/// Executes a sequence of steps to determine the browser location/culture.
		/// </summary>
		/// <param name="exec">When this filter is being invoked</param>
		/// <param name="context">Current context</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns>
		/// 	<c>true</c> if the action should be invoked, otherwise <c>false</c>
		/// </returns>
		public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			try
			{
				String localeId = GetLocaleId(context);

				if (localeId == null && setup.UseBrowser)
					localeId = GetUserLanguage(context.Request);

				if (localeId != null)
				{
					CultureInfo culture = CultureInfo.CreateSpecificCulture(localeId);

					Thread.CurrentThread.CurrentCulture = culture;
					Thread.CurrentThread.CurrentUICulture = culture;
				}
			}
			catch
			{
				if (setup.FailOnError) throw;
			}

			return true;
		}

		#endregion

		#region Get locale id from the store

		/// <summary>
		/// Gets the user language.
		/// </summary>
		/// <param name="request">The request.</param>
		/// <returns></returns>
		private String GetUserLanguage(IRequest request)
		{
			if (request.UserLanguages != null && request.UserLanguages.Length > 0)
				return request.UserLanguages[0];

			return null;
		}

		/// <summary>
		/// Gets the locale id.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		private String GetLocaleId(IEngineContext context)
		{
			switch(setup.Store)
			{
				case RequestStore.Session:
					return context.Session[setup.Key] as String;
				case RequestStore.Cookie:
					return context.Request.ReadCookie(setup.Key);
				case RequestStore.QueryString:
					return context.Request.QueryString[setup.Key];
				case RequestStore.Form:
					return context.Request.Form[setup.Key];
				case RequestStore.Params:
					return context.Request.Params[setup.Key];
			}

			return null;
		}

		#endregion
	}
}
