// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Controllers
{
	using System;
	using System.Web;
	using Castle.MonoRail.Framework.Helpers;
    using Castle.MonoRail.Framework.Attributes;
    using Castle.MonoRail.Framework.Filters;

	/// <summary>
	/// Buit in <see cref="Controller"/> containing the files requireds by helpers and other 
	/// parts of MonoRail.
	/// </summary>
	[ControllerDetails(Area="MonoRail")]
	[PersistFlashFilter]
	[LocalizationFilter(Store = RequestStore.QueryString, Key = "locale")]
	[Resource("Behaviour","Castle.MonoRail.Framework.Controllers.Behaviour", CultureName="neutral")]
	[Resource("Ajax","Castle.MonoRail.Framework.Controllers.Ajax", CultureName="neutral")]
	[Resource("Effects2", "Castle.MonoRail.Framework.Controllers.Effects2", CultureName="neutral")]
	[Resource("EffectsFat", "Castle.MonoRail.Framework.Controllers.EffectsFat", CultureName="neutral")]
	[Resource("Validation", "Castle.MonoRail.Framework.Controllers.Validation", CultureName="neutral")]
	[Resource("FormHelper", "Castle.MonoRail.Framework.Controllers.FormHelper", CultureName="neutral")]
	public sealed class FilesController : Controller
	{
		/// <summary>
		/// Script used by <see cref="AjaxHelper"/>.
		/// </summary>
		[Cache(HttpCacheability.Public, Duration=86400)] // 1 day
		public void AjaxScripts()
		{	
			RenderJavascriptFile( "Ajax", "jsfunctions" );
		}

		/// <summary>
		/// Script used by <see cref="AjaxHelper"/>.
		/// </summary>
		public void BehaviourScripts()
		{
			RenderJavascriptFile( "Behaviour", "jsfunctions" );
		}
		
		/// <summary>
		/// Script used by <see cref="EffectsFatHelper"/>.
		/// </summary>
		public void EffectsFatScripts()
		{
			RenderJavascriptFile( "EffectsFat", "fatfunctions" );
		}

		/// <summary>
		/// Script used by <see cref="Effects2Helper"/>.
		/// </summary>
		public void Effects2()
		{
			RenderJavascriptFile( "Effects2", "functions" );
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateConfig()
		{
			RenderJavascriptFile( "Validation", "fValidateConfig" );
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateCore()
		{
			RenderJavascriptFile( "Validation", "fValidateCore" );
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
		public void ValidateValidators()
		{
			RenderJavascriptFile( "Validation", "fValidateValidators" );
		}

		/// <summary>
		/// Script used by <see cref="ValidationHelper"/>.
		/// </summary>
        [Resource("ValidationLang", "Castle.MonoRail.Framework.Controllers.ValidationLang")]
		public void ValidateLang()
		{
			RenderJavascriptFile( "ValidationLang", "fValidateLang" );
		}

		/// <summary>
		/// Script used by <see cref="AjaxHelper"/>.
		/// </summary>
		public void FormHelperScript()
		{
			RenderJavascriptFile("FormHelper", "jsfunctions");
		}

		private void RenderJavascriptFile( String resourceName, String resourceKey )
		{
            Response.ContentType = "text/javascript";
            string fileContent = GetResourceValue(resourceName, resourceKey);
            RenderText(fileContent);
		}

		private String GetResourceValue(String resName, String resKey)
		{
			return (String) (Resources[resName])[resKey];
		}
	}
}
